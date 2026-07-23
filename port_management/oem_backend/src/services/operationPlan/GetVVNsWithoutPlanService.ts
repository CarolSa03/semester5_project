import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IOperationPlanRepo} from "../../domain/operationPlan/IOperationPlanRepo";
import {IPortModuleService} from "../IServices/IPortModuleService";
import {IMissingPlanDTO} from "../../dto/IMissingPlanDTO";

@Service()
export default class GetVVNsWithoutPlanService {
    constructor(
        @Inject('OperationPlanRepo') private planRepo: IOperationPlanRepo,
        @Inject('PortAdapter') private portService: IPortModuleService
    ) {}

    public async execute(dateStr?: string, startDateStr?: string, endDateStr?: string): Promise<Result<IMissingPlanDTO[]>> {
        try {
            // 1. Parse dates if provided
            let date: Date | undefined;
            let startDate: Date | undefined;
            let endDate: Date | undefined;

            if (dateStr) {
                date = new Date(dateStr);
                if (isNaN(date.getTime())) {
                    return Result.fail<IMissingPlanDTO[]>("Invalid date format");
                }
            }

            if (startDateStr && endDateStr) {
                startDate = new Date(startDateStr);
                endDate = new Date(endDateStr);
                if (isNaN(startDate.getTime()) || isNaN(endDate.getTime())) {
                    return Result.fail<IMissingPlanDTO[]>("Invalid date range format");
                }
                if (startDate > endDate) {
                    return Result.fail<IMissingPlanDTO[]>("Start date must be before end date");
                }
            }

            // 2. Fetch approved VVNs from Port Module (MOCK DATA)
            const vvns = await this.portService.fetchApprovedVVNs(date, startDate, endDate);

            if (vvns.length === 0) {
                return Result.ok<IMissingPlanDTO[]>([]);
            }

            // 3. Extract VVN IDs
            const vvnIds = vvns.map(v => v.vvnId);

            // 4. Find which VVNs don't have plans
            const missingVvnIds = await this.planRepo.findVVNsWithoutPlan(vvnIds);

            // 5. Build response DTOs
            const missingPlans: IMissingPlanDTO[] = vvns
                .filter(v => missingVvnIds.includes(v.vvnId))
                .map(v => ({
                    vvnId: v.vvnId,
                    vesselName: v.vesselName,
                    expectedArrival: v.expectedArrival?.toISOString(),
                    detectedAt: new Date().toISOString()
                }));

            return Result.ok<IMissingPlanDTO[]>(missingPlans);
        } catch (e) {
            return Result.fail<IMissingPlanDTO[]>(`Error fetching VVNs without plans: ${e}`);
        }
    }
}
