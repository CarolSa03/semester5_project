export interface IRegisterExecutionDTO {
    vvnId: string;           // ID da visita (Vessel Visit Notification)
    operationId: string;     // ID da operação (partilhado entre plano e execução)
    realStartTime: string;   // Data/hora real de início (ISO 8601 string)
    realEndTime: string;     // Data/hora real de fim (ISO 8601 string)
    realResource: string;    // ID do recurso (grua/staff) efetivamente usado
    status: string;          // Esperado: 'COMPLETED' ou 'DELAYED'
    completedBy: string;     // ID do utilizador que registou a execução
}