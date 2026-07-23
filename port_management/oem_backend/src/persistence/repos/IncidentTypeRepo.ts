import {Inject, Service} from "typedi";
import {Model} from "mongoose";
import {IncidentType} from "../../domain/incidentType/IncidentType";
import {IncidentTypeMap} from "../../mappers/IncidentTypeMap";
import {IIncidentTypeRepo} from "../../domain/incidentType/IIncidentTypeRepo";
import {UniqueEntityID} from "../../core/domain/UniqueEntityID";

@Service()
export default class IncidentTypeRepo implements IIncidentTypeRepo {
    constructor(
        @Inject('IncidentTypeSchema') private schema: Model<Document & any>
    ) {}

    public async exists(t: IncidentType): Promise<boolean> {
        const idX = t.id instanceof UniqueEntityID ? t.id.toValue() : t.id;
        const query = { domainId: idX };
        const doc = await this.schema.findOne(query);
        return !!doc;
    }

    public async save(incidentType: IncidentType): Promise<IncidentType> {
        const query = { domainId: incidentType.id.toString() };
        const raw = IncidentTypeMap.toPersistence(incidentType);

        try {
            if (await this.exists(incidentType)) {
                await this.schema.findOneAndUpdate(query, raw);
            } else {
                await this.schema.create(raw);
            }
            return incidentType;
        } catch (err) {
            throw err;
        }
    }

    public async findByCode(code: string): Promise<IncidentType | null> {
        // Filtra apenas ativos
        const query = { code: code, active: true };
        const raw = await this.schema.findOne(query);

        if (!raw) return null;
        return IncidentTypeMap.toDomain(raw);
    }

    public async findById(id: string): Promise<IncidentType | null> {
        // Aceita buscar inativos (para update/ativar) ou filtra se preferires
        const query = { domainId: id };
        const raw = await this.schema.findOne(query);

        if (!raw) return null;
        return IncidentTypeMap.toDomain(raw);
    }

    public async findAll(): Promise<IncidentType[]> {
        // Retorna apenas os ativos
        const docs = await this.schema.find({ active: true });
        return docs.map(doc => IncidentTypeMap.toDomain(doc));
    }

    public async findByParentId(parentId: string): Promise<IncidentType[]> {
        // Procura pelo campo parentId e garante que estão ativos
        const docs = await this.schema.find({ parentId: parentId, active: true });
        return docs.map(doc => IncidentTypeMap.toDomain(doc));
    }
}