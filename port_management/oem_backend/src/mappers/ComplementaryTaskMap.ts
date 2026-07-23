import { ComplementaryTask } from "../domain/complementaryTask/ComplementaryTask";
import { ComplementaryTaskCategory } from "../domain/complementaryTask/ComplementaryTaskCategory";
import { IComplementaryTaskDTO } from "../dto/complementaryTask/IComplementaryTaskDTO";
import { UniqueEntityID } from "../core/domain/UniqueEntityID";
import { TimeRange } from "../domain/shared/TimeRange.vo";
import { TaskStatus } from "../domain/complementaryTask/enums/TaskStatus.enum";

/**
 * Mapper para Complementary Tasks
 * US 4.1.15
 */
export class ComplementaryTaskMap {

    /**
     * Converte domain entity para DTO
     */
    public static toDTO(
        task: ComplementaryTask,
        category: ComplementaryTaskCategory
    ): IComplementaryTaskDTO {
        return {
            id: task.id.toString(),
            categoryCode: category.code,
            categoryName: category.name,
            vveId: task.vveId,
            responsible: task.responsible,
            startTime: task.timeWindow.getStartTime().toISOString(),
            endTime: task.timeWindow.getEndTime().toISOString(),
            status: task.status,
            isBlocking: task.isBlocking,
            description: task.description,
            createdBy: task.createdBy,
            createdAt: task.createdAt.toISOString()
        };
    }

    /**
     * Converte dados de persistência para domain entity
     */
    public static toDomain(raw: any): ComplementaryTask {
        const timeWindow = TimeRange.create(
            new Date(raw.startTime),
            new Date(raw.endTime)
        );

        const taskOrError = ComplementaryTask.create(
            {
                categoryId: new UniqueEntityID(raw.categoryId),
                vveId: raw.vveId,
                responsible: raw.responsible,
                timeWindow: timeWindow,
                isBlocking: raw.isBlocking,
                description: raw.description,
                createdBy: raw.createdBy
            },
            new UniqueEntityID(raw.domainId)
        );

        if (taskOrError.isFailure) {
            throw new Error(taskOrError.error.toString());
        }

        const task = taskOrError.getValue();

        // Restaurar status e createdAt da persistência
        (task as any).props.status = raw.status as TaskStatus;
        (task as any).props.createdAt = new Date(raw.createdAt);

        return task;
    }

    /**
     * Converte domain entity para formato de persistência
     */
    public static toPersistence(task: ComplementaryTask): any {
        return {
            domainId: task.id.toString(),
            categoryId: task.categoryId.toString(),
            vveId: task.vveId,
            responsible: task.responsible,
            startTime: task.timeWindow.getStartTime(),
            endTime: task.timeWindow.getEndTime(),
            status: task.status,
            isBlocking: task.isBlocking,
            description: task.description,
            createdBy: task.createdBy,
            createdAt: task.createdAt
        };
    }
}
