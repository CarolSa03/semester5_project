import {Router} from 'express';
import {celebrate, Joi} from 'celebrate';
import {Container} from 'typedi';
import IncidentController from '../../controllers/IncidentController';

const route = Router();

export default (app: Router) => {
    app.use('/incidents', route);

    const ctrl = Container.get(IncidentController);

    // POST: Criar
    route.post(
        '',
        celebrate({
            body: Joi.object({
                incidentTypeCode: Joi.string().required(),
                description: Joi.string().required(),
                severity: Joi.string().optional(),
                affectedVvnIds: Joi.array().items(Joi.string()).optional(),
                createdBy: Joi.string().required()
            })
        }),
        (req, res, next) => ctrl.createIncident(req, res, next)
    );

    // PUT: Atualizar / Resolver
    route.put(
        '/:id',
        celebrate({
            body: Joi.object({
                description: Joi.string().optional(),
                severity: Joi.string().optional(),
                endTime: Joi.date().optional(), // Enviar data para resolver
                affectedVvnIds: Joi.array().items(Joi.string()).optional()
            })
        }),
        (req, res, next) => ctrl.updateIncident(req, res, next)
    );

    // GET: Listar com Filtros
    route.get(
        '',
        (req, res, next) => ctrl.getIncidents(req, res, next)
    );

    // DELETE: Remover/Anular
    route.delete(
        '/:id',
        (req, res, next) => ctrl.deleteIncident(req, res, next)
    );
};