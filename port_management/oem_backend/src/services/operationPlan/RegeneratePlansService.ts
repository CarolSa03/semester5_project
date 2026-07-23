import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IOperationPlanRepo} from "../../domain/operationPlan/IOperationPlanRepo";
import {IPortModuleService} from "../IServices/IPortModuleService";
import {IPlanningExternalService} from "../IServices/IPlanningExternalService";
import {IRegeneratePlansDTO} from "../../dto/IRegeneratePlansDTO";
import {IRegenerationMetadataDTO} from "../../dto/IRegenerationMetadataDTO";
import {OperationPlan} from "../../domain/operationPlan/OperationPlan";
import {PlannedOperation} from "../../domain/operationPlan/PlannedOperation";
import {TimeRange} from "../../domain/shared/TimeRange.vo";
import {OperationType} from "../../domain/operationPlan/enums/OperationType.enum";

@Service()
export default class RegeneratePlansService {
    constructor(
        @Inject('OperationPlanRepo') private planRepo: IOperationPlanRepo,
        @Inject('PortAdapter') private portService: IPortModuleService,
        @Inject('PlanningAdapter') private planningService: IPlanningExternalService
    ) {}

    public async execute(dto: IRegeneratePlansDTO): Promise<Result<IRegenerationMetadataDTO>> {
        try {
            // 1. Validar data
            const targetDate = new Date(dto.date);
            if (isNaN(targetDate.getTime())) {
                return Result.fail<IRegenerationMetadataDTO>("Invalid date format");
            }

            // 2. Validar confirmação
            if (!dto.overwriteExisting) {
                return Result.fail<IRegenerationMetadataDTO>(
                    "Regeneration requires explicit confirmation (overwriteExisting: true)"
                );
            }

            // 3. Apagar planos existentes
            const deletedCount = await this.planRepo.deleteByDate(targetDate);
            console.log(`✅ Deleted ${deletedCount} existing plans for ${dto.date}`);

            // 4. Fetch VVNs (Mock)
            const vvns = await this.portService.fetchApprovedVVNs(targetDate);

            if (vvns.length === 0) {
                return Result.ok<IRegenerationMetadataDTO>({
                    regeneratedPlans: 0,
                    overwrittenPlans: deletedCount,
                    newPlans: 0,
                    algorithm: dto.algorithm,
                    author: dto.author,
                    timestamp: new Date().toISOString()
                });
            }

            // 5. Gerar novos planos
            const planningResult = await this.planningService.fetchSchedule(targetDate, dto.algorithm);

            if (!planningResult || planningResult.isFailure) {
                return Result.fail<IRegenerationMetadataDTO>(
                    "Planning module failed to generate schedule: " + planningResult.error
                );
            }

            // 6. Criar e gravar planos
            let savedCount = 0;
            const planDTO = planningResult.getValue();

            // Nota: O PlanningAdapter devolve um plano único com várias operações.
            // A lógica aqui assume que criamos UM plano por VVN.
            // Se o scheduleItem vier misturado, teríamos de agrupar.
            // Assumindo estrutura simples do Adapter corrigido:

            for (const scheduleItem of planDTO.schedule) {
                const baseDate = new Date(targetDate);
                const startDateTime = new Date(baseDate.getTime() + (scheduleItem.startTime * 60000));
                const endDateTime = new Date(baseDate.getTime() + (scheduleItem.endTime * 60000));

                // CORREÇÃO: Usar resourceId em vez de dockId
                const resourceToUse = scheduleItem.resourceId || "UNKNOWN-RESOURCE";

                const operation = PlannedOperation.create(
                    OperationType.LOADING,
                    TimeRange.create(startDateTime, endDateTime),
                    resourceToUse // Agora passamos a string garantida
                );

                const planOrError = OperationPlan.create({
                    vvnId: scheduleItem.vesselId,
                    date: targetDate,
                    algorithm: dto.algorithm,
                    totalDelay: 0,
                    computationTime: 0,
                    operations: [operation]
                });

                if (planOrError.isSuccess) {
                    const plan = planOrError.getValue();
                    plan.updatePlan({}, dto.author, dto.reason);
                    await this.planRepo.save(plan);
                    savedCount++;
                }
            }

            console.log(`✅ Created ${savedCount} new plans`);

            return Result.ok<IRegenerationMetadataDTO>({
                regeneratedPlans: savedCount,
                overwrittenPlans: deletedCount,
                newPlans: Math.max(0, savedCount - deletedCount),
                algorithm: dto.algorithm,
                author: dto.author,
                timestamp: new Date().toISOString()
            });
        } catch (e) {
            return Result.fail<IRegenerationMetadataDTO>(`Error regenerating plans: ${e}`);
        }
    }
}