import { Router, Request, Response, NextFunction } from 'express';
import { celebrate, Joi } from 'celebrate';
import { Container } from 'typedi';
import { ComplementaryTaskCategoryController } from '../../controllers/ComplementaryTaskCategoryController';

const route = Router();

export default (app: Router) => {
    app.use('/complementary-task-categories', route);

    // Resolver o Controller via Container (Dependency Injection)
    const controller = Container.get(ComplementaryTaskCategoryController);

    // POST: Criar Categoria
    route.post('',
        celebrate({
            body: Joi.object({
                name: Joi.string().required(),
                code: Joi.string().required(),
                description: Joi.string().optional()
            })
        }),
        (req: Request, res: Response, next: NextFunction) => controller.create(req, res, next)
    );

    // GET: Listar Categorias
    route.get('',
        (req: Request, res: Response, next: NextFunction) => controller.list(req, res, next)
    );

    // DELETE: Apagar Categoria
    route.delete('/:id',
        (req: Request, res: Response, next: NextFunction) => controller.delete(req, res, next)
    );
};