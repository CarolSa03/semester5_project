import {Router} from 'express';
import {Container} from 'typedi';
import {celebrate, Joi} from 'celebrate';
import VesselVisitExecutionController from '../../controllers/VesselVisitExecutionController';

const route = Router();

export default (app: Router) => {
    app.use('/vessel-visit-executions', route);

    const ctrl = Container.get(VesselVisitExecutionController);

    // POST: Create
    route.post(
        '',
        celebrate({
            body: Joi.object({
                vvnId: Joi.string().required(),
                arrivalDate: Joi.string().required()
            })
        }),
        (req, res, next) => ctrl.create(req, res, next)
    );

    // GET: Search
    route.get(
        '/search',
        celebrate({
            query: Joi.object({
                startDate: Joi.string().optional(),
                endDate: Joi.string().when('startDate', {
                    is: Joi.exist(), then: Joi.string().required(), otherwise: Joi.optional()
                }),
                vesselId: Joi.string().optional(),
                status: Joi.string().valid('IN_PROGRESS', 'COMPLETED', 'PENDING').optional()
            }).unknown(true) // Permite outros parametros se necessário
        }),
        (req, res, next) => ctrl.searchVVEs(req, res, next)
    );

    // --- NOVAS ROTAS ---

    // GET: Get By ID
    route.get('/:id', (req, res, next) => ctrl.getDetails(req, res, next));

    // PATCH: Register Berth
    route.patch(
        '/:id/berth',
        celebrate({
            body: Joi.object({
                berthDate: Joi.string().required(),
                dockId: Joi.string().required()
            })
        }),
        (req, res, next) => ctrl.registerBerthing(req, res, next)
    );

    // PATCH: Register Departure (US 11)
    route.patch(
        '/:id/complete',
        celebrate({
            body: Joi.object({
                departureTime: Joi.string().required() 
            }).unknown(true) 
        }),
        (req, res, next) => {
            req.body.departureDate = req.body.departureTime; 
            ctrl.registerDeparture(req, res, next);
        }
    );
};
