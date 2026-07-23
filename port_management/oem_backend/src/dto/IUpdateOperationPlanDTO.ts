export interface IUpdateOperationPlanDTO {
    operations?: Array<{
        operationId: string;
        // Os campos são opcionais para permitir atualizações parciais
        resourceId?: string;
        startTime?: string; // ISO String
        endTime?: string;   // ISO String
        status?: string;
    }>;
    status?: string;
    author: string;
    reason: string;
}

export interface IUpdateOperationPlanResponseDTO {
    id: string;
    vvnId: string;
    date: string;
    algorithm: string;
    status: string;
    operations: Array<any>;
    lastModified: string;
    lastModifiedBy: string;
    modificationReason: string;
    warnings?: string[]; // Campo para devolver alertas não-bloqueantes (se necessário)
}