import { Inject, Service } from "typedi";
import { Result } from "../../core/logic/Result";
import { ComplementaryTaskCategory } from "../../domain/complementaryTask/ComplementaryTaskCategory";
import { IComplementaryTaskCategoryDTO } from "../../dto/complementaryTask/IComplementaryTaskCategoryDTO";
import { IComplementaryTaskCategoryRepo } from "../../domain/complementaryTask/IComplementaryTaskCategoryRepo";
import {ComplementaryTaskCategoryMap} from "../../mappers/ComplementaryTaskCategoryMap";

@Service()
export class CreateComplementaryTaskCategoryService {
    constructor(
        @Inject("ComplementaryTaskCategoryRepo")
        private repo: IComplementaryTaskCategoryRepo
    ) {}


    public async execute(dto: IComplementaryTaskCategoryDTO): Promise<Result<IComplementaryTaskCategoryDTO>> {
        try {
            const existing = await this.repo.findByCode(dto.code);
            if (existing) {
                return Result.fail(`Category with code ${dto.code} already exists`);
            }

            const categoryOrError = ComplementaryTaskCategory.create(dto);
            if (categoryOrError.isFailure) {
                return Result.fail(categoryOrError.error);
            }

            const category = categoryOrError.getValue();

            await this.repo.save(category);
            return Result.ok<IComplementaryTaskCategoryDTO>(ComplementaryTaskCategoryMap.toDTO(category));
        } catch (error) {
            return Result.fail(error);
        }
    }
}
