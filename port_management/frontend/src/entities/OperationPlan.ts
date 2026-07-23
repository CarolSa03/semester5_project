export interface IOperationPlan {
    id: string;
    vvnId: string;
    date: string;
    metrics: {
        algorithm: string;
        totalDelay: number;
        computationTime: number;
        problemSize?: {
            vessels: number;
            resources: number;
            staff: number;
        };
    };
    schedule: IPlannedOperation[];
    warnings?: string[];
}

export interface IPlannedOperation {
    operationId?: string; // Adicionado para suportar update
    vesselId: string;

    // CORREÇÃO: Tornar explícito o que é o quê
    dockId?: string;       // Opcional, pois a operação define quem trabalha (Resource), o Dock é do VVN
    resourceId: string;    // Obrigatório: É a Grua (STS-001, etc.)

    startTime: number;
    endTime: number;
    durationMinutes: number;
    assignedCranes: string[]; // Pode manter para compatibilidade visual ou remover a favor de resourceId
    delay: number;

    // Campos auxiliares de estado
    type?: string;
    status?: string;
}

export interface IGenerateOperationPlanRequest {
    vvnId: string;
    date: string;
    algorithm: 'genetic' | 'astar' | 'Genetic' | 'AStar';
}

export interface IUpdateOperationPlanRequest {
    status?: 'ACTIVE' | 'ARCHIVED' | 'SUPERSEDED';
    operations?: IPlannedOperation[];
    author: string;
    reason: string;
}
