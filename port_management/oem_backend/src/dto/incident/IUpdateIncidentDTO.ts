import {IncidentTypeSeverity} from "../../domain/incidentType/enums/IncidentTypeSeverity.enum";

export interface IUpdateIncidentDTO {
    id: string; // ID do incidente a atualizar

    description?: string;
    severity?: IncidentTypeSeverity;

    // Para resolver o incidente, envia-se a data de fim
    endTime?: string;

    // Para atualizar a lista de navios afetados
    affectedVvnIds?: string[];
}