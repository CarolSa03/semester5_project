import { Request, Response, NextFunction } from "express";
import { Inject, Service } from "typedi";
import { CreateComplementaryTaskCategoryService } from "../services/complementaryTask/CreateComplementaryTaskCategoryService";
import { ListComplementaryTaskCategoriesService } from "../services/complementaryTask/ListComplementaryTaskCategoryService";
import { DeleteComplementaryTaskCategoryService } from "../services/complementaryTask/DeleteComplementaryTaskCategoryService";
import { ComplementaryTaskCategoryMap } from "../mappers/ComplementaryTaskCategoryMap";

@Service()
export class ComplementaryTaskCategoryController {

    constructor(
        private createService: CreateComplementaryTaskCategoryService,
        private listService: ListComplementaryTaskCategoriesService,
        private deleteService: DeleteComplementaryTaskCategoryService
    ) {}

    public async create(req: Request, res: Response, next: NextFunction) {
        try {
            const dto = req.body;
            const result = await this.createService.execute(dto);
            if (result.isFailure) {
                return res.status(400).json({ error: result.error });
            }
            return res.status(201).json(result.getValue());
        } catch (e) {
            return next(e);
        }
    }

    public async list(req: Request, res: Response, next: NextFunction) {
        try {
            const result = await this.listService.execute();

            if (result.isFailure) {
                return res.status(500).json({ error: result.error });
            }

            return res.status(201).json(result.getValue())
        } catch (e) {
            return next(e);
        }
    }

    public async delete(req: Request, res: Response, next: NextFunction) {
        try {
            const result = await this.deleteService.execute(req.params.id);

            if (result.isFailure) {
                return res.status(404).json({ error: result.error });
            }

            return res.status(204).send();
        } catch (e) {
            return next(e);
        }
    }
}
