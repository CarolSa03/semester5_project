import {Repo} from "../../core/infra/Repo";
import {IncidentType} from "./IncidentType";

export interface IIncidentTypeRepo extends Repo<IncidentType> {

    save(incidentType: IncidentType): Promise<IncidentType>;
    findByCode(code: string): Promise<IncidentType | null>;
    findById(id: string): Promise<IncidentType | null>;
    findAll(): Promise<IncidentType[]>;
    findByParentId(parentId: string): Promise<IncidentType[]>;
}