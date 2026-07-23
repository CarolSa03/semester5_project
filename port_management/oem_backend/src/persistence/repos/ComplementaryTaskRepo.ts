import { Service, Inject } from "typedi";
import { Model, Document } from "mongoose";
import { ComplementaryTaskCategory } from "../../domain/complementaryTask/ComplementaryTaskCategory";
import { ComplementaryTaskCategoryMap } from "../../mappers/ComplementaryTaskCategoryMap";

@Service()
export default class ComplementaryTaskCategoryRepo {
    constructor(
        @Inject('ComplementaryTaskCategorySchema')
        private schema: Model<Document & any>
    ) {}

    // Verifica se o documento já existe
    public async exists(category: ComplementaryTaskCategory): Promise<boolean> {
        const idValue = category.id.toString(); // converte UniqueEntityID para string
        const doc = await this.schema.findOne({ domainId: idValue });
        return !!doc;
    }

    // Salva ou atualiza a entidade
    public async save(category: ComplementaryTaskCategory): Promise<ComplementaryTaskCategory> {
        const idValue = category.id.toString(); // converte UniqueEntityID para string
        const raw = ComplementaryTaskCategoryMap.toPersistence(category);

        if (await this.exists(category)) {
            await this.schema.findOneAndUpdate({ domainId: idValue }, raw, { new: true });
        } else {
            await this.schema.create({ ...raw, domainId: idValue });
        }

        return category;
    }

    // Busca pelo código único da categoria
    public async findByCode(code: string): Promise<ComplementaryTaskCategory | null> {
        const raw = await this.schema.findOne({ code, active: true });
        if (!raw) return null;
        return ComplementaryTaskCategoryMap.toDomain(raw);
    }

    // Busca pelo ID do AggregateRoot
    public async findById(id: string): Promise<ComplementaryTaskCategory | null> {
        const raw = await this.schema.findOne({ domainId: id });
        if (!raw) return null;
        return ComplementaryTaskCategoryMap.toDomain(raw);
    }

    // Retorna todas as categorias ativas
    public async findAll(): Promise<ComplementaryTaskCategory[]> {
        const docs = await this.schema.find({ active: true });
        return docs.map(doc => ComplementaryTaskCategoryMap.toDomain(doc));
    }

    // Soft delete da categoria
    public async delete(category: ComplementaryTaskCategory): Promise<void> {
        const idValue = category.id.toString();
        await this.schema.findOneAndUpdate(
            { domainId: idValue },
            { active: false }
        );
    }
}


