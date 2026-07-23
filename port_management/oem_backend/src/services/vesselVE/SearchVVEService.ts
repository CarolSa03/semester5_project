// src1/services/vesselVE/SearchVVEService.ts
import {Inject, Service} from 'typedi';
import {IVesselVisitExecutionRepo} from '../../domain/vesselVE/IVesselVisitExecutionRepo';
import {IPortModuleService} from '../IServices/IPortModuleService';
import {IOperationPlanRepo} from '../../domain/operationPlan/IOperationPlanRepo';
import VVEMetricsService from './VVEMetricsService';
import {IVVESearchFiltersDTO} from '../../dto/vesselVE/IVVESearchFiltersDTO';
import {IVVESearchResultDTO, IExecutedOperationResultDTO} from '../../dto/vesselVE/IVVESearchResultDTO';
import {Result} from '../../core/logic/Result';

@Service()
export default class SearchVVEService {
    constructor(
        @Inject('VesselVisitExecutionRepo') private vveRepo: IVesselVisitExecutionRepo,
        @Inject('PortAdapter') private portAdapter: IPortModuleService,
        @Inject('OperationPlanRepo') private planRepo: IOperationPlanRepo,
        @Inject(() => VVEMetricsService) private metricsService: VVEMetricsService
    ) {}

    public async execute(filters: IVVESearchFiltersDTO): Promise<Result<IVVESearchResultDTO[]>> {
        try {
            let startDate = filters.startDate ? new Date(filters.startDate) : undefined;
            let endDate = filters.endDate ? new Date(filters.endDate) : undefined;

            const allVVNs = await this.portAdapter.fetchApprovedVVNs(undefined, startDate, endDate);
            const localExecutions = await this.vveRepo.findByFilters({});
            const executionMap = new Map(localExecutions.map(e => [e.vvnId, e]));

            const results: IVVESearchResultDTO[] = [];

            for (const vvn of allVVNs) {
                if (filters.vesselId && vvn.vesselId !== filters.vesselId) continue;

                const vve = executionMap.get(vvn.vvnId);
                const currentStatus = vve ? vve.status : 'PENDING';

                if (filters.status && filters.status !== currentStatus) continue;

                let operationPlan = null;
                try {
                    operationPlan = await this.planRepo.findByVVN(vvn.vvnId);
                } catch (e) { /* ignore */ }

                let resultDTO: IVVESearchResultDTO;

                if (vve) {
                    // --- CENÁRIO: Em Curso / Completo ---
                    const metrics = this.metricsService.calculateMetrics(
                        vve,
                        { eta: vvn.expectedArrival, etd: vvn.expectedDeparture, vesselName: vvn.vesselName },
                        operationPlan
                    );

                    const executedOpsDTO: IExecutedOperationResultDTO[] = vve.executedOperations.map(op => ({
                        operationId: op.operationId.toString(),
                        status: op.status,
                        realResource: op.realResource,
                        realStartTime: op.realTimeWindow.getStartTime().toISOString(),
                        realEndTime: op.realTimeWindow.getEndTime().toISOString()
                    }));

                    resultDTO = {
                        id: vvn.vvnId, // <--- ID Preenchido
                        vvnId: vvn.vvnId,
                        vesselName: vvn.vesselName,
                        vesselId: vvn.vesselId,
                        status: vve.status,
                        arrivalDate: vve.arrivalDate ? vve.arrivalDate.toISOString() : null,
                        departureDate: vve.departureDate ? vve.departureDate.toISOString() : null,
                        expectedArrival: vvn.expectedArrival ? vvn.expectedArrival.toISOString() : null,
                        expectedDeparture: vvn.expectedDeparture ? vvn.expectedDeparture.toISOString() : null,
                        assignedDock: vvn.assignedDock,
                        metrics: metrics,
                        executedOperations: executedOpsDTO
                    };

                } else {
                    // --- CENÁRIO: Pendente ---
                    resultDTO = {
                        id: vvn.vvnId, // <--- ID Preenchido
                        vvnId: vvn.vvnId,
                        vesselName: vvn.vesselName,
                        vesselId: vvn.vesselId,
                        status: 'PENDING',
                        arrivalDate: null,
                        departureDate: null,
                        expectedArrival: vvn.expectedArrival ? vvn.expectedArrival.toISOString() : null,
                        expectedDeparture: vvn.expectedDeparture ? vvn.expectedDeparture.toISOString() : null,
                        assignedDock: vvn.assignedDock,
                        // Métricas Vazias (Estrutura Correta)
                        metrics: {
                            turnaroundTime: null,
                            berthOccupancyTime: null,
                            waitingTimeForBerthing: null,
                            arrivalDelay: 0,
                            departureDelay: 0,
                            operationDelays: 0,
                            totalOperations: operationPlan ? operationPlan.operations.length : 0,
                            completedOperations: 0,
                            delayedOperations: 0,
                            progressPercentage: 0
                        },
                        executedOperations: []
                    };
                }
                results.push(resultDTO);
            }

            return Result.ok<IVVESearchResultDTO[]>(results);

        } catch (error: any) {
            console.error('❌ [SearchVVEService] Error:', error);
            return Result.fail<IVVESearchResultDTO[]>(`Failed to search VVEs: ${error.message}`);
        }
    }
}