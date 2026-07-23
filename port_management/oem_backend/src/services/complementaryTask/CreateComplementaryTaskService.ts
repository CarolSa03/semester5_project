import { Inject, Service } from "typedi";
import { IComplementaryTaskRepo } from "../../domain/complementaryTask/IComplementaryTaskRepo";
import { IComplementaryTaskCategoryRepo } from "../../domain/complementaryTask/IComplementaryTaskCategoryRepo";
import { IVesselVisitExecutionRepo } from "../../domain/vesselVE/IVesselVisitExecutionRepo";
import { ComplementaryTask } from "../../domain/complementaryTask/ComplementaryTask";
import { ICreateComplementaryTaskDTO } from "../../dto/complementaryTask/ICreateComplementaryTaskDTO";
import { IComplementaryTaskDTO } from "../../dto/complementaryTask/IComplementaryTaskDTO";
import { ComplementaryTaskMap } from "../../mappers/ComplementaryTaskMap";
import { Result } from "../../core/logic/Result";
import { TimeRange } from "../../domain/shared/TimeRange.vo";
import { ValidationError } from "../../core/logic/ValidateError";

/**
 * Service para criar tarefas complementares
 * US 4.1.15
 */
@Service()
export class CreateComplementaryTaskService {
    constructor(
        @Inject("ComplementaryTaskRepo")
        private taskRepo: IComplementaryTaskRepo,

        @Inject("ComplementaryTaskCategoryRepo")
        private categoryRepo: IComplementaryTaskCategoryRepo,

        @Inject("VesselVisitExecutionRepo")
        private vveRepo: IVesselVisitExecutionRepo
    ) { }

    public async execute(dto: ICreateComplementaryTaskDTO): Promise<Result<IComplementaryTaskDTO>> {
        try {
            // 1. Validar que a categoria existe
            const category = await this.categoryRepo.findByCode(dto.categoryCode);
            if (!category) {
                return Result.fail<IComplementaryTaskDTO>(
                    `Category with code ${dto.categoryCode} not found`
                );
            }

            // 2. Validar que a VVE existe
            const vve = await this.vveRepo.findByVVN(dto.vveId);
            if (!vve) {
                return Result.fail<IComplementaryTaskDTO>(
                    `Vessel Visit Execution ${dto.vveId} not found`
                );
            }

            // 3. Validar e criar janela de tempo
            const startTime = new Date(dto.startTime);
            const endTime = new Date(dto.endTime);

            if (isNaN(startTime.getTime()) || isNaN(endTime.getTime())) {
                return Result.fail<IComplementaryTaskDTO>("Invalid date format");
            }

            let timeWindow: TimeRange;
            try {
                timeWindow = TimeRange.create(startTime, endTime);
            } catch (error) {
                if (error instanceof ValidationError) {
                    return Result.fail<IComplementaryTaskDTO>(error.message);
                }
                throw error;
            }

            // 4. Criar a tarefa complementar
            const taskOrError = ComplementaryTask.create({
                categoryId: category.id,
                vveId: dto.vveId,
                responsible: dto.responsible,
                timeWindow: timeWindow,
                isBlocking: dto.isBlocking,
                description: dto.description,
                createdBy: dto.createdBy
            });

            if (taskOrError.isFailure) {
                return Result.fail<IComplementaryTaskDTO>(taskOrError.error);
            }

            const task = taskOrError.getValue();

            // 5. Salvar no repositório
            await this.taskRepo.save(task);

            // 6. Retornar DTO
            const taskDTO = ComplementaryTaskMap.toDTO(task, category);
            return Result.ok<IComplementaryTaskDTO>(taskDTO);

        } catch (error: unknown) {
            return Result.fail<IComplementaryTaskDTO>(
                `Error creating complementary task: ${error}`
            );
        }
    }
}
