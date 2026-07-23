/**
 * DTO para atualizar status de uma tarefa
 * US 4.1.15
 */
export interface IUpdateTaskStatusDTO {
    status: "IN_PROGRESS" | "COMPLETED" | "CANCELLED";
}
