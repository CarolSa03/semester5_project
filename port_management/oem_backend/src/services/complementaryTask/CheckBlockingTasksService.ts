import { Inject, Service } from "typedi";
import { IComplementaryTaskRepo } from "../../domain/complementaryTask/IComplementaryTaskRepo";
import { IComplementaryTaskCategoryRepo } from "../../domain/complementaryTask/IComplementaryTaskCategoryRepo";
import { IBlockingTasksCheckDTO } from "../../dto/complementaryTask/IBlockingTasksCheckDTO";
import { ComplementaryTaskMap } from "../../mappers/ComplementaryTaskMap";
import { Result } from "../../core/logic/Result";

/**
 * Service para verificar se existem tarefas bloqueantes ativas
 * US 4.1.15 - Lógica crítica para suspender operações de carga
 */
@Service()
export class CheckBlockingTasksService {
    constructor(
        @Inject("ComplementaryTaskRepo")
        private taskRepo: IComplementaryTaskRepo,

        @Inject("ComplementaryTaskCategoryRepo")
        private categoryRepo: IComplementaryTaskCategoryRepo
    ) { }

    public async execute(vveId: string): Promise<Result<IBlockingTasksCheckDTO>> {
        try {
            // 1. Buscar tarefas bloqueantes ativas
            const blockingTasks = await this.taskRepo.findBlockingTasksByVVE(vveId);

            // 2. Se existem tarefas bloqueantes, buscar categorias para montar DTOs
            const taskDTOs = await Promise.all(
                blockingTasks.map(async (task) => {
                    const category = await this.categoryRepo.findById(
                        task.categoryId.toString()
                    );
                    if (!category) {
                        throw new Error(`Category not found for task ${task.id}`);
                    }
                    return ComplementaryTaskMap.toDTO(task, category);
                })
            );

            // 3. Determinar se operações devem ser suspensas
            const hasBlockingTasks = blockingTasks.length > 0;
            const suspendCargoOperations = hasBlockingTasks;

            // 4. Montar mensagem explicativa
            let message = "No blocking tasks found. Cargo operations can proceed.";
            if (hasBlockingTasks) {
                message = `${blockingTasks.length} blocking task(s) active. Cargo operations SUSPENDED.`;
            }

            // 5. Retornar resultado
            const result: IBlockingTasksCheckDTO = {
                vveId,
                hasBlockingTasks,
                suspendCargoOperations,
                blockingTasks: taskDTOs,
                message
            };

            return Result.ok<IBlockingTasksCheckDTO>(result);

        } catch (error: unknown) {
            return Result.fail<IBlockingTasksCheckDTO>(
                `Error checking blocking tasks: ${error}`
            );
        }
    }
}
