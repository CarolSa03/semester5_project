import { Service, Inject } from 'typedi';
import { Result } from "../../core/logic/Result";
import { IOperationPlanDTO } from "../../dto/IOperationPlanDTO";
import { IPlanningExternalService } from "../IServices/IPlanningExternalService";
import { IOperationPlanRepo } from "../../domain/operationPlan/IOperationPlanRepo";
import { OperationPlanMap } from "../../mappers/OperationPlanMap";

@Service()
export default class CreateOperationPlanService {
    constructor(
        @Inject('OperationPlanRepo') private operationPlanRepo: IOperationPlanRepo,
        @Inject('PlanningAdapter') private planningService: IPlanningExternalService
    ) {}

    // Aceita objeto completo
    public async execute(dto: { vvnId: string, date: Date, algorithm?: string }): Promise<Result<IOperationPlanDTO>> {
        try {
            console.log(`⚙️ [Service] A pedir plano para ${dto.vvnId} na data ${dto.date}`);

            // IMPORTANTE: Passar dto.vvnId como 3º argumento
            const planningResult = await this.planningService.fetchSchedule(dto.date, dto.algorithm, dto.vvnId);

            if (planningResult.isFailure) {
                return Result.fail<IOperationPlanDTO>(planningResult.error);
            }

            const planDTO = planningResult.getValue();

            // Mapeamento
            const operationPlan = OperationPlanMap.toDomain(planDTO);

            // SE ISTO FOR NULL, LANÇA O ERRO QUE ESTÁS A VER
            if (!operationPlan) {
                console.error("❌ [Service] OperationPlanMap.toDomain retornou null!");
                return Result.fail<IOperationPlanDTO>("Failed to map planning result to domain");
            }

            await this.operationPlanRepo.save(operationPlan);
            const resultDTO = OperationPlanMap.toDTO(operationPlan);

            return Result.ok<IOperationPlanDTO>(resultDTO);

        } catch (e) {
            console.error("❌ [Service] Exceção:", e);
            return Result.fail<IOperationPlanDTO>((e as Error).message);
        }
    }
}