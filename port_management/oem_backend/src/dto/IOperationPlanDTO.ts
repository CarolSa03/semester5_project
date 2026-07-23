export interface IPlannedOperationDTO {
    // Campos novos obrigatórios
    operationId: string;
    resourceId: string;

    // Campos antigos/opcionais
    dockId?: string;

    vesselId: string;
    startTime: number;
    endTime: number;
    durationMinutes: number;
    assignedCranes: string[];
    delay: number;
    status?: string;
}

export interface IOperationPlanDTO {
    id: string;
    vvnId: string;
    date: string;
    metrics: {
        algorithm: string;
        totalDelay: number;
        computationTime: number;
    };
    // Array usa a nova interface
    schedule: IPlannedOperationDTO[];
}