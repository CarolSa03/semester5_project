/**
 * DTO para criar uma nova tarefa complementar
 * US 4.1.15
 */
export interface ICreateComplementaryTaskDTO {
    categoryCode: string;      // Código da categoria (ex: "SEC-001", "MAINT-002")
    vveId: string;             // ID da Vessel Visit Execution
    responsible: string;       // Nome ou ID do responsável
    startTime: string;         // Data/hora início (ISO 8601)
    endTime: string;           // Data/hora fim (ISO 8601)
    isBlocking: boolean;       // Se bloqueia operações de carga
    description?: string;      // Descrição adicional opcional
    createdBy: string;         // ID do utilizador que cria a tarefa
}
