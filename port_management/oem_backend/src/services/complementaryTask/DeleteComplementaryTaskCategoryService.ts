
import { Inject, Service } from "typedi";
import { Result } from "../../core/logic/Result";
import { IComplementaryTaskCategoryRepo } from "../../domain/complementaryTask/IComplementaryTaskCategoryRepo";


@Service()
export class DeleteComplementaryTaskCategoryService {
    constructor(
        @Inject("ComplementaryTaskCategoryRepo")
        private repo: IComplementaryTaskCategoryRepo
    ) {}

    public async execute(code: string): Promise<Result<void>> {
        try {
            const category = await this.repo.findById(code);
            if (!category) {
                return Result.fail("Category not found");
            }

            await this.repo.delete(category);
            return Result.ok<void>();
        } catch (error) {
            return Result.fail(error);
        }

    }
}
