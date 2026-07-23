import {Repo} from "../../core/infra/Repo";
import {Incident} from "./Incident";

export interface IIncidentRepo extends Repo<Incident> {
    save(incident: Incident): Promise<Incident>;
    findById(id: string): Promise<Incident | null>;

    // Métodos para os Filtros da US
    findActive(): Promise<Incident[]>;
    findByVessel(vvnId: string): Promise<Incident[]>;
    findByDateRange(start: Date, end: Date): Promise<Incident[]>;
    findBySeverity(severity: string): Promise<Incident[]>;
    delete(id: string): Promise<void>;
    findAll(): Promise<Incident[]>;
    countByTypeId(typeId: string): Promise<number>;
}