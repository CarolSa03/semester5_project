export interface IIncidentTypeDTO {
    id: string;
    code: string;
    name: string;
    description: string;
    severity: string;
    parentId?: string; // Opcional no JSON
}