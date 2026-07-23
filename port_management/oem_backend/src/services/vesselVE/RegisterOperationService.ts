import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IVesselVisitExecutionRepo} from "../../domain/vesselVE/IVesselVisitExecutionRepo";
import {IOperationPlanRepo} from "../../domain/operationPlan/IOperationPlanRepo";
import {IRegisterExecutionDTO} from "../../dto/vesselVE/IRegisterExecutionDTO";
import {ExecutedOperation} from "../../domain/vesselVE/ExecutedOperation";
import {OperationId} from "../../domain/operationPlan/value-objects/OperationId.vo";
import {TimeRange} from "../../domain/shared/TimeRange.vo";
import {OperationStatus} from "../../domain/operationPlan/enums/OperationStatus.enum";
import {VesselVisitExecution} from "../../domain/vesselVE/VesselVisitExecution";

@Service()
export default class RegisterOperationService {
    constructor(
        @Inject('VesselVisitExecutionRepo') private vveRepo: IVesselVisitExecutionRepo,
        @Inject('OperationPlanRepo') private planRepo: IOperationPlanRepo
    ) {}

    public async execute(dto: IRegisterExecutionDTO): Promise<Result<void>> {
        try {
            // =========================================================
            // PASSO 1: Obter ou Criar a VVE (Vessel Visit Execution)
            // =========================================================
            let vve = await this.vveRepo.findByVVN(dto.vvnId);

            if (!vve) {
                // Se não existir, criamos (fail-safe para garantir que a US funciona
                // mesmo se a US 4.1.7 não tiver sido executada antes)
                const vveOrError = VesselVisitExecution.create({ vvnId: dto.vvnId });
                if (vveOrError.isFailure) {
                    return Result.fail<void>(`Failed to create VVE: ${vveOrError.error}`);
                }
                vve = vveOrError.getValue();
            }

            // =========================================================
            // PASSO 2: Validar e Preparar Dados
            // =========================================================
            const start = new Date(dto.realStartTime);
            const end = new Date(dto.realEndTime);

            if (isNaN(start.getTime()) || isNaN(end.getTime())) {
                return Result.fail<void>("Invalid date format provided.");
            }

            // Garantir que timeRange é válido (inicio < fim)
            if (end < start) {
                return Result.fail<void>("End time cannot be before start time.");
            }

            const operationId = OperationId.create(dto.operationId);
            const timeRange = TimeRange.create(start, end);

            // Validar Status (converte string para Enum)
            const statusKey = dto.status as keyof typeof OperationStatus;
            const status = OperationStatus[statusKey];

            if (!status) {
                return Result.fail<void>(`Invalid status '${dto.status}'. Must be COMPLETED or DELAYED.`);
            }

            // =========================================================
            // PASSO 3: Criar e Registar a Execução na VVE
            // =========================================================
            const executedOp = ExecutedOperation.create({
                operationId: operationId,
                realTimeWindow: timeRange,
                realResource: dto.realResource,
                status: status,
                completedBy: dto.completedBy
            });

            vve.registerOperation(executedOp);
            await this.vveRepo.save(vve);

            // =========================================================
            // PASSO 4: Sincronizar com o Plano de Operação (Core da US)
            // =========================================================
            const plan = await this.planRepo.findByVVN(dto.vvnId);

            if (plan) {
                // Usamos o metodo do domínio para atualizar o estado
                const updateResult = plan.updateOperationStatus(dto.operationId, status, dto.completedBy);

                if (updateResult.isSuccess) {
                    await this.planRepo.save(plan);
                } else {
                    // Logamos o aviso mas não falhamos o request (soft consistency)
                    console.warn(`[Sync Warning] Could not update plan for Op ${dto.operationId}: ${updateResult.error}`);
                }
            } else {
                console.warn(`[Sync Warning] No Operation Plan found for VVN ${dto.vvnId}`);
            }

            return Result.ok<void>();

        } catch (e) {
            console.error(e);
            return Result.fail<void>(e instanceof Error ? e.message : "Unexpected error in RegisterOperationService");
        }
    }
}