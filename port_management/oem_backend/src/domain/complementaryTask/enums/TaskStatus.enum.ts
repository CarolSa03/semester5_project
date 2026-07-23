/**
 * Estados possíveis de uma tarefa complementar
 * US 4.1.15 - Complementary Tasks
 */
export enum TaskStatus {
    PENDING = "PENDING",           // Tarefa criada mas não iniciada
    IN_PROGRESS = "IN_PROGRESS",   // Tarefa em execução
    COMPLETED = "COMPLETED",       // Tarefa concluída com sucesso
    CANCELLED = "CANCELLED"        // Tarefa cancelada
}
