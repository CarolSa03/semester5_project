export interface IIncidentDTO {
    id: string;
    incidentTypeId: string;
    description: string;
    severity: string;
    startTime: string;
    endTime?: string;
    duration?: number;
    active: boolean;
    affectedVvnIds: string[];
    createdBy: string;
}