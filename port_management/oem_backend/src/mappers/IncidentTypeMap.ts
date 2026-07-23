import {Mapper} from "../core/infra/Mapper";
import {IncidentType} from "../domain/incidentType/IncidentType";
import {IIncidentTypeDTO} from "../dto/incidentType/IIncidentTypeDTO";
import {UniqueEntityID} from "../core/domain/UniqueEntityID";
import {IncidentTypeSeverity} from "../domain/incidentType/enums/IncidentTypeSeverity.enum";

export class IncidentTypeMap extends Mapper<IncidentType> {

    public static toDTO(incident: IncidentType): IIncidentTypeDTO {
        return {
            id: incident.id.toString(),
            code: incident.code,
            name: incident.name,
            description: incident.description,
            severity: incident.severity,
            parentId: incident.parentId
        } as IIncidentTypeDTO;
    }

    public static toPersistence(incident: IncidentType): any {
        return {
            domainId: incident.id.toString(),
            code: incident.code,
            name: incident.name,
            description: incident.description,
            severity: incident.severity,
            parentId: incident.parentId,
            active: incident.isActive
        };
    }

    public static toDomain(raw: any): IncidentType {
        const incidentOrError = IncidentType.create({
            code: raw.code,
            name: raw.name,
            description: raw.description,
            severity: raw.severity as IncidentTypeSeverity,
            parentId: raw.parentId
        }, new UniqueEntityID(raw.domainId));

        return incidentOrError.getValue();
    }
}