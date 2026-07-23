import {Router} from 'express';
import {celebrate, Joi} from 'celebrate';
import {Container} from 'typedi';
import IncidentTypeController from '../../controllers/IncidentTypeController';

const route = Router();

export default (app: Router) => {
    app.use('/incident-types', route);

    const ctrl = Container.get(IncidentTypeController);

    // CREATE
    route.post(
        '',
        celebrate({
            body: Joi.object({
                // Adicionada regex para validar formato do código (ex: apenas letras, números e hífens)
                code: Joi.string().required().pattern(/^[a-zA-Z0-9-]+$/),
                name: Joi.string().required(),
                description: Joi.string().required(),
                severity: Joi.string().valid('Minor', 'Major', 'Critical').required(),
                parentId: Joi.string().allow(null, '')
            })
        }),
        (req, res, next) => ctrl.create(req, res, next)
    );

    // LIST (Com filtro opcional)
    route.get(
        '',
        celebrate({
            query: Joi.object({
                parentId: Joi.string().optional()
            })
        }),
        (req, res, next) => ctrl.list(req, res, next)
    );

    // UPDATE
    route.put(
        '/:id',
        celebrate({
            body: Joi.object({
                name: Joi.string(),
                description: Joi.string(),
                severity: Joi.string().valid('Minor', 'Major', 'Critical'),
                parentId: Joi.string().allow(null, '')
            })
        }),
        (req, res, next) => ctrl.update(req, res, next)
    );

    // DELETE
    route.delete(
        '/:id',
        (req, res, next) => ctrl.delete(req, res, next)
    );
};