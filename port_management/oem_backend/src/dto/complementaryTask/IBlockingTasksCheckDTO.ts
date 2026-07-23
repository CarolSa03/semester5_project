import { IComplementaryTaskDTO } from "./IComplementaryTaskDTO";

/**
 * DTO para verificação de tarefas bloqueantes
 * US 4.1.15 - Lógica crítica de suspensão de operações
 */
export interface IBlockingTasksCheckDTO {
    vveId: string;
    hasBlockingTasks: boolean;           // Se existem tarefas bloqueantes ativas
    suspendCargoOperations: boolean;     // Se operações de carga devem ser suspensas
    blockingTasks: IComplementaryTaskDTO[];  // Lista das tarefas bloqueantes
    message: string;                     // Mensagem explicativa
}
