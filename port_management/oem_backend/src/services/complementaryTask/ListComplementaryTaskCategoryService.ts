import { Inject, Service } from "typedi";
import { Result } from "../../core/logic/Result";
import { ComplementaryTaskCategory } from "../../domain/complementaryTask/ComplementaryTaskCategory";
import { IComplementaryTaskCategoryDTO } from "../../dto/complementaryTask/IComplementaryTaskCategoryDTO";
import { IComplementaryTaskCategoryRepo } from "../../domain/complementaryTask/IComplementaryTaskCategoryRepo";
import {IIncidentTypeDTO} from "../../dto/incidentType/IIncidentTypeDTO";

@Service()
export class ListComplementaryTaskCategoriesService {
    constructor(
        @Inject("ComplementaryTaskCategoryRepo")
        private repo: IComplementaryTaskCategoryRepo
    ) {}

    public async execute(): Promise<Result<IComplementaryTaskCategoryDTO[]>> {
        try {
            const categories = await this.repo.findAll();
            if (categories.length === 0) {
                return Result.fail<IComplementaryTaskCategoryDTO[]>("No complementary task categories found");
            }

            const categoriesDTO = categories.map((category: ComplementaryTaskCategory) => {
                return {
                    code: category.code,
                    name: category.name,
                    description: category.description,
                    duration: category.duration
                } as IComplementaryTaskCategoryDTO;
            });
            return Result.ok<IComplementaryTaskCategoryDTO[]>(categoriesDTO);


        } catch (error) {
            return Result.fail<IComplementaryTaskCategoryDTO[]>(error);
        }
    }
}
