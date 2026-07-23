import { ComplementaryTaskCategory } from "./ComplementaryTaskCategory";

export interface IComplementaryTaskCategoryRepo {
    save(category: ComplementaryTaskCategory): Promise<void>;
    findByCode(code: string): Promise<ComplementaryTaskCategory | null>;
    findById(id: string): Promise<ComplementaryTaskCategory | null>;
    findAll(): Promise<ComplementaryTaskCategory[]>;
    delete(category: ComplementaryTaskCategory): Promise<void>;
}
