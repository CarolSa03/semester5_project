import { ComplementaryTask } from "./ComplementaryTask";

/**
 * Interface do repositório de tarefas complementares
 * US 4.1.15
 */
export interface IComplementaryTaskRepo {
    /**
     * Salva ou atualiza uma tarefa
     */
    save(task: ComplementaryTask): Promise<ComplementaryTask>;

    /**
     * Verifica se a tarefa já existe
     */
    exists(task: ComplementaryTask): Promise<boolean>;

    /**
     * Busca tarefa por ID
     */
    findById(id: string): Promise<ComplementaryTask | null>;

    /**
     * Busca todas as tarefas de uma VVE
     */
    findByVVE(vveId: string): Promise<ComplementaryTask[]>;

    /**
     * Busca tarefas bloqueantes ativas de uma VVE
     * (isBlocking=true e status=PENDING ou IN_PROGRESS)
     */
    findBlockingTasksByVVE(vveId: string): Promise<ComplementaryTask[]>;

    /**
     * Busca tarefas ativas num intervalo de tempo
     */
    findActiveTasksInTimeRange(
        vveId: string,
        start: Date,
        end: Date
    ): Promise<ComplementaryTask[]>;

    /**
     * Remove uma tarefa (soft delete)
     */
    delete(task: ComplementaryTask): Promise<void>;
}
