/**
 * DTO completo de uma tarefa complementar
 * US 4.1.15
 */
export interface IComplementaryTaskDTO {
    id: string;
    categoryCode: string;
    categoryName: string;      // Nome da categoria (ex: "Security Check")
    vveId: string;
    responsible: string;
    startTime: string;         // ISO 8601
    endTime: string;           // ISO 8601
    status: string;            // "PENDING" | "IN_PROGRESS" | "COMPLETED" | "CANCELLED"
    isBlocking: boolean;
    description?: string;
    createdBy: string;
    createdAt: string;         // ISO 8601
}
