export interface IOperationPlanPersistence {
    domainId: string;
    vvnId: string;
    date: Date;
    algorithm: string;

    // Métricas
    totalDelay: number;
    computationTime: number;

    // Auditoria e Estado
    status: string;
    lastModified?: Date;
    lastModifiedBy?: string;
    modificationReason?: string;

    // Lista de Operações (Estrutura plana para BD)
    operations: Array<{
        type: string;
        status: string;
        resourceId: string;
        startTime: Date;
        endTime: Date;
    }>;
}