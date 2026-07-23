import {Mapper} from "../core/infra/Mapper";
import {Incident} from "../domain/incident/Incident";
import {IIncidentDTO} from "../dto/incident/IIncidentDTO";
import {UniqueEntityID} from "../core/domain/UniqueEntityID"; // Importar UniqueEntityID

export class IncidentMap extends Mapper<Incident> {
    public static toDTO(incident: Incident): IIncidentDTO {
        return {
            id: incident.id.toString(),
            incidentTypeId: incident.incidentTypeId,
            description: incident.description,
            severity: incident.severity,
            startTime: incident.startTime.toISOString(),
            endTime: incident.endTime ? incident.endTime.toISOString() : undefined,
            duration: incident.durationMinutes || undefined,
            active: incident.isActive,
            affectedVvnIds: incident.affectedVvnIds,
            createdBy: incident.createdBy
        } as IIncidentDTO;
    }

    public static toPersistence(incident: Incident): any {
        return {
            domainId: incident.id.toString(),
            incidentTypeId: incident.incidentTypeId,
            description: incident.description,
            severity: incident.severity,
            startTime: incident.startTime,
            endTime: incident.endTime,
            affectedVvnIds: incident.affectedVvnIds,
            createdBy: incident.createdBy
        };
    }

    public static toDomain(raw: any): Incident | null {
        try {
            // CORREÇÃO 1: Extrair apenas as props necessárias (segurança)
            const props = {
                incidentTypeId: raw.incidentTypeId,
                description: raw.description,
                severity: raw.severity,
                startTime: raw.startTime,
                endTime: raw.endTime,
                affectedVvnIds: raw.affectedVvnIds,
                createdBy: raw.createdBy
            };

            // CORREÇÃO 2: Recuperar o ID original (domainId)
            const domainId = new UniqueEntityID(raw.domainId);

            // CORREÇÃO 3: Passar o ID para o create
            const result = Incident.create(props, domainId);

            if (result.isFailure) {
                console.warn('IncidentMap.toDomain: failed to create Incident', result.error);
                return null;
            }
            return result.getValue();
        } catch (err) {
            console.error('IncidentMap.toDomain: unexpected error', err);
            return null;
        }
    }
}