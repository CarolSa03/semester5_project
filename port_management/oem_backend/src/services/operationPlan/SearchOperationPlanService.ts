import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IOperationPlanRepo} from "../../domain/operationPlan/IOperationPlanRepo";
import {OperationPlanMap} from "../../mappers/OperationPlanMap";
import {IOperationPlanDTO} from "../../dto/IOperationPlanDTO";

@Service()
export default class SearchOperationPlanService {
    constructor(
        @Inject('OperationPlanRepo') private operationPlansRepo: IOperationPlanRepo
    ) {}

    public async getOperationPlans(): Promise<Result<IOperationPlanDTO[]>> {
        try {
            const operationPlans = await this.operationPlansRepo.findAll();

            // If no plans, return empty array
            if (!operationPlans || operationPlans.length === 0) {
                return Result.ok<IOperationPlanDTO[]>([]);
            }

            // Diagnostic: log basic info about the returned collection
            try {
                console.debug(`SearchOperationPlanService.getOperationPlans: repo returned count=${Array.isArray(operationPlans) ? operationPlans.length : 'non-array'}`);
                if (Array.isArray(operationPlans)) {
                    const invalidIndexes: number[] = [];
                    operationPlans.forEach((p, idx) => { if (!p || !(p as any).id) invalidIndexes.push(idx); });
                    if (invalidIndexes.length > 0) {
                        console.warn(`SearchOperationPlanService.getOperationPlans: found invalid items at indexes=${invalidIndexes.join(',')}`);
                    }
                }
            } catch (logErr) {
                console.warn('Failed to log operationPlans diagnostic:', logErr);
            }

            // Filter out any null/undefined items just in case
            const filtered = (Array.isArray(operationPlans) ? operationPlans : [operationPlans]).filter(p => p && (p as any).id);

            // Safe mapping: wrap in try/catch to prevent a single bad item from crashing
            let operationPlansDTOResult: IOperationPlanDTO[] = [];
            try {
                operationPlansDTOResult = filtered.map(operationPlan => {
                    if (!operationPlan) return null as any;
                    return OperationPlanMap.toDTO(operationPlan) as IOperationPlanDTO;
                }).filter(x => x != null) as IOperationPlanDTO[];
            } catch (mapErr) {
                console.error('Error mapping operation plans to DTOs:', mapErr);
                // Return empty array to caller instead of throwing
                return Result.ok<IOperationPlanDTO[]>([]);
            }

            return Result.ok<IOperationPlanDTO[]>(operationPlansDTOResult)
        } catch (e) {
            throw e;
        }
    }

    public async getOperationPlansByDateRange(startDate: string, endDate: string): Promise<Result<IOperationPlanDTO[]>> {
        try {
            const start = new Date(startDate);
            const end = new Date(endDate);
            const operationPlans = await this.operationPlansRepo.findByDateRange(start, end);

            if (!operationPlans || operationPlans.length === 0) {
                return Result.ok<IOperationPlanDTO[]>([]);
            }

            const filtered = operationPlans.filter(p => p && (p as any).id);
            const operationPlansDTOResult = filtered.map(operationPlan => OperationPlanMap.toDTO(operationPlan) as IOperationPlanDTO);
            return Result.ok<IOperationPlanDTO[]>(operationPlansDTOResult)
        } catch (e) {
            throw e;
        }
    }

    public async getOperationPlansByVesselId(vesselId: string): Promise<Result<IOperationPlanDTO[]>> {
        try {
            const operationPlans = await this.operationPlansRepo.findByVVN(vesselId);

            if (!operationPlans || (Array.isArray(operationPlans) && operationPlans.length === 0)) {
                return Result.ok<IOperationPlanDTO[]>([]);
            }

            // If repository returned a single plan (non-array), normalize to array
            const plansArray = Array.isArray(operationPlans) ? operationPlans : [operationPlans];
            const filtered = plansArray.filter(p => p && (p as any).id);
            const operationPlansDTOResult = filtered.map(operationPlan => OperationPlanMap.toDTO(operationPlan) as IOperationPlanDTO);
            return Result.ok<IOperationPlanDTO[]>(operationPlansDTOResult);
        } catch (e) {
            throw e;
        }
    }

    public async getOperationPlansById(operationPlansId: string): Promise<Result<IOperationPlanDTO>> {
        try {
            const operationPlans = await this.operationPlansRepo.findById(operationPlansId);

            if (operationPlans === null) {
                return Result.fail<IOperationPlanDTO>("OperationPlans not found");
            } else {
                const operationPlansDTOResult = OperationPlanMap.toDTO(operationPlans) as IOperationPlanDTO;
                return Result.ok<IOperationPlanDTO>(operationPlansDTOResult)
            }
        } catch (e) {
            throw e;
        }
    }

    public async getOperationPlanById(operationPlanId: string): Promise<Result<IOperationPlanDTO>> {
        try {
            const operationPlans = await this.operationPlansRepo.findById(operationPlanId);
            if (operationPlans === null) {
                return Result.fail<IOperationPlanDTO>("OperationPlan not found");
            } else {
                const operationPlanDTOResult = OperationPlanMap.toDTO(operationPlans) as IOperationPlanDTO;
                return Result.ok<IOperationPlanDTO>(operationPlanDTOResult)
            }
        } catch (e) {
            throw e;
        }
    }

    public async getAllocation(resourceId: string, start: string, end: string) {

        const id = resourceId || "VVN-TEST-01";

        return [
            {
                operationId: "OP-001",
                resourceId: id,
                status: "IN_PROGRESS",
                description: "Mooring / Docking",
                
                startTime: "2026-01-06T08:00:00Z",
                endTime:   "2026-01-06T12:00:00Z",
                start:     "2026-01-06T08:00:00Z",
                end:       "2026-01-06T12:00:00Z",
                realStartTime: "2026-01-06T08:00:00Z",
                realEndTime:   "2026-01-06T12:00:00Z"
            },
            {
                operationId: "OP-002",
                resourceId: id,
                status: "IN_PROGRESS",
                description: "Unloading Containers (Bay A)",

                startTime: "2026-01-06T09:30:00Z",
                endTime:   "2026-01-06T12:30:00Z",
                start:     "2026-01-06T09:30:00Z",
                end:       "2026-01-06T12:30:00Z",
                realStartTime: "2026-01-06T09:30:00Z",
                realEndTime:   "2026-01-06T12:30:00Z"
            },
            {
                operationId: "OP-003",
                resourceId: id,
                status: "IN_PROGRESS",
                description: "Refueling Operation",

                startTime: "2026-01-06T14:00:00Z",
                endTime:   "2026-01-06T15:30:00Z",
                start:     "2026-01-06T14:00:00Z",
                end:       "2026-01-06T15:30:00Z",
                realStartTime: "2026-01-06T14:00:00Z",
                realEndTime:   "2026-01-06T15:30:00Z"
            }
        ];
    }
}
