import {Inject, Service} from 'typedi';
import {IIncidentRepo} from '../../domain/incident/IIncidentRepo';
import {Incident} from '../../domain/incident/Incident';
import {IncidentMap} from '../../mappers/IncidentMap';

@Service()
export default class IncidentRepo implements IIncidentRepo {
    constructor(
        @Inject('incidentSchema') private incidentSchema: any
    ) {}

    public async exists(t: Incident): Promise<boolean> {
        const id = t.id instanceof Object ? t.id.toString() : t.id;
        const query = { domainId: id };
        const record = await this.incidentSchema.findOne(query);
        return !!record;
    }

    public async save(incident: Incident): Promise<Incident> {
        const query = { domainId: incident.id.toString() };
        const rawIncident = IncidentMap.toPersistence(incident);
        try {
            const created = await this.incidentSchema.findOneAndUpdate(
                query,
                rawIncident,
                { upsert: true, new: true }
            );
            return IncidentMap.toDomain(created) as Incident; // assume created is valid
        } catch (err) {
            throw err;
        }
    }

    public async findById(id: string): Promise<Incident | null> {
        const query = { domainId: id };
        const record = await this.incidentSchema.findOne(query);
        const domain = record ? IncidentMap.toDomain(record) : null;
        return domain;
    }

    public async delete(id: string): Promise<void> {
        const query = { domainId: id };
        await this.incidentSchema.deleteOne(query);
    }

    // --- Filtros ---

    public async findActive(): Promise<Incident[]> {
        const query = { $or: [{ endTime: null }, { endTime: { $exists: false } }] };
        const records = await this.incidentSchema.find(query);
        return records.map((r: any) => IncidentMap.toDomain(r)).filter((i: any) => i != null) as Incident[];
    }

    public async findByVessel(vvnId: string): Promise<Incident[]> {
        const query = { affectedVvnIds: vvnId };
        const records = await this.incidentSchema.find(query);
        return records.map((r: any) => IncidentMap.toDomain(r)).filter((i: any) => i != null) as Incident[];
    }

    public async findByDateRange(start: Date, end: Date): Promise<Incident[]> {
        const query = { startTime: { $gte: start, $lte: end } };
        const records = await this.incidentSchema.find(query);
        return records.map((r: any) => IncidentMap.toDomain(r)).filter((i: any) => i != null) as Incident[];
    }

    public async findBySeverity(severity: string): Promise<Incident[]> {
        const query = { severity: severity };
        const records = await this.incidentSchema.find(query);
        return records.map((r: any) => IncidentMap.toDomain(r)).filter((i: any) => i != null) as Incident[];
    }

    public async findAll(): Promise<Incident[]> {
        const records = await this.incidentSchema.find({});
        return records.map((r: any) => IncidentMap.toDomain(r)).filter((i: any) => i != null) as Incident[];
    }

    public async countByTypeId(typeId: string): Promise<number> {
        // Verifica se existem documentos com este incidentTypeId
        const count = await this.incidentSchema.countDocuments({ incidentTypeId: typeId });
        return count;
    }
}