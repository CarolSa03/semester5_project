import { ComplementaryTaskCategory } from "../domain/complementaryTask/ComplementaryTaskCategory";
import { IComplementaryTaskCategoryDTO } from "../dto/complementaryTask/IComplementaryTaskCategoryDTO";
import {UniqueEntityID} from "../core/domain/UniqueEntityID";
import {IncidentType} from "../domain/incidentType/IncidentType";

export class ComplementaryTaskCategoryMap {
    public static toDTO(category: ComplementaryTaskCategory): IComplementaryTaskCategoryDTO {
        return {
            code: category.code,
            name: category.name,
            description: category.description,
            duration: category.duration
        };
    }

    public static toDomain(raw: any): ComplementaryTaskCategory {
        const categoryOrError = ComplementaryTaskCategory.create({
            code: raw.code,
            name: raw.name,
            description: raw.description,
            duration: raw.duration
        }, new UniqueEntityID());

        return categoryOrError.getValue();
    }

    public static toPersistence(category: ComplementaryTaskCategory): any {
        return {
            domainID: category.id.toString(),
            code: category.code,
            name: category.name,
            description: category.description,
            duration: category.duration
        };
    }
}
