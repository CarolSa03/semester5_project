export interface IIncidentPersistence {
    domainId: string;
    incidentTypeId: string;
    description: string;
    severity: string;
    startTime: Date;
    endTime?: Date;
    affectedVvnIds: string[];
    createdBy: string;
}