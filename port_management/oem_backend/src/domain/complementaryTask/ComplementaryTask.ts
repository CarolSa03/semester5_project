import { AggregateRoot } from "../../core/domain/AggregateRoot";
import { UniqueEntityID } from "../../core/domain/UniqueEntityID";
import { Result } from "../../core/logic/Result";
import { TimeRange } from "../shared/TimeRange.vo";
import { TaskStatus } from "./enums/TaskStatus.enum";

interface ComplementaryTaskProps {
    categoryId: UniqueEntityID;  // Referência à categoria
    vveId: string;                // Referência à VVE (Vessel Visit Execution)
    responsible: string;          // Responsável pela tarefa (nome ou ID)
    timeWindow: TimeRange;        // Início e fim da tarefa
    status: TaskStatus;           // PENDING, IN_PROGRESS, COMPLETED, CANCELLED
    isBlocking: boolean;          // Se bloqueia operações de carga (crítico!)
    description?: string;         // Descrição adicional
    createdBy: string;            // ID do utilizador que criou
    createdAt: Date;              // Timestamp de criação
}

/**
 * Complementary Task Aggregate Root
 * US 4.1.15 - Tarefas complementares que podem bloquear operações de carga
 */
export class ComplementaryTask extends AggregateRoot<ComplementaryTaskProps> {

    // Getters públicos (essenciais para services e mappers)
    get id(): UniqueEntityID {
        return this._id;
    }

    get categoryId(): UniqueEntityID {
        return this.props.categoryId;
    }

    get vveId(): string {
        return this.props.vveId;
    }

    get responsible(): string {
        return this.props.responsible;
    }

    get timeWindow(): TimeRange {
        return this.props.timeWindow;
    }

    get status(): TaskStatus {
        return this.props.status;
    }

    get isBlocking(): boolean {
        return this.props.isBlocking;
    }

    get description(): string | undefined {
        return this.props.description;
    }

    get createdBy(): string {
        return this.props.createdBy;
    }

    get createdAt(): Date {
        return this.props.createdAt;
    }

    private constructor(props: ComplementaryTaskProps, id?: UniqueEntityID) {
        super(props, id);
    }

    /**
     * Factory method para criar nova tarefa complementar
     */
    public static create(
        props: {
            categoryId: UniqueEntityID;
            vveId: string;
            responsible: string;
            timeWindow: TimeRange;
            isBlocking: boolean;
            description?: string;
            createdBy: string;
        },
        id?: UniqueEntityID
    ): Result<ComplementaryTask> {

        // Validações de negócio
        if (!props.categoryId) {
            return Result.fail("Category ID is required");
        }

        if (!props.vveId || props.vveId.trim().length === 0) {
            return Result.fail("VVE ID is required");
        }

        if (!props.responsible || props.responsible.trim().length === 0) {
            return Result.fail("Responsible person is required");
        }

        if (!props.timeWindow) {
            return Result.fail("Time window is required");
        }

        if (!props.createdBy || props.createdBy.trim().length === 0) {
            return Result.fail("Creator ID is required");
        }

        const task = new ComplementaryTask(
            {
                ...props,
                status: TaskStatus.PENDING, // Status inicial
                createdAt: new Date()
            },
            id
        );

        return Result.ok(task);
    }

    /**
     * Marca a tarefa como "em progresso"
     */
    public startTask(): Result<void> {
        if (this.props.status !== TaskStatus.PENDING) {
            return Result.fail(`Cannot start task with status ${this.props.status}`);
        }

        this.props.status = TaskStatus.IN_PROGRESS;
        return Result.ok();
    }

    /**
     * Marca a tarefa como concluída
     */
    public markAsCompleted(): Result<void> {
        if (this.props.status === TaskStatus.COMPLETED) {
            return Result.fail("Task is already completed");
        }

        if (this.props.status === TaskStatus.CANCELLED) {
            return Result.fail("Cannot complete a cancelled task");
        }

        this.props.status = TaskStatus.COMPLETED;
        return Result.ok();
    }

    /**
     * Cancela a tarefa
     */
    public cancel(): Result<void> {
        if (this.props.status === TaskStatus.COMPLETED) {
            return Result.fail("Cannot cancel a completed task");
        }

        this.props.status = TaskStatus.CANCELLED;
        return Result.ok();
    }

    /**
     * Atualiza a janela de tempo da tarefa
     */
    public updateTimeWindow(newWindow: TimeRange): Result<void> {
        if (this.props.status === TaskStatus.COMPLETED) {
            return Result.fail("Cannot update time window of completed task");
        }

        this.props.timeWindow = newWindow;
        return Result.ok();
    }

    /**
     * Verifica se a tarefa está ativa e bloqueante
     * MÉTODO CRÍTICO para US 4.1.15 - suspensão de operações de carga
     */
    public isActiveAndBlocking(): boolean {
        return this.props.isBlocking &&
            (this.props.status === TaskStatus.PENDING ||
                this.props.status === TaskStatus.IN_PROGRESS);
    }
}
