import {Router} from 'express';
import {Container} from 'typedi';
import {celebrate, Joi} from "celebrate";
import OperationPlanController from '../../controllers/OperationPlanController';

const route = Router();

export default (app: Router) => {
    app.use('/operation-plans', route);

    const ctrl = Container.get(OperationPlanController);

    route.post(
        '/generate',
        // --- ADICIONA ESTE BLOCO DE VALIDAÇÃO ---
        celebrate({
            body: Joi.object({
                // Valida UUID se o teu sistema usar UUIDs, ou string simples se não
                vvnId: Joi.string().required(),
                // Garante que é uma data válida
                date: Joi.string().required(), // Aceita ISO e YYYY-MM-DD
                // Garante que é um dos algoritmos permitidos (Case sensitive!)
                algorithm: Joi.string().valid('genetic', 'astar', 'Genetic', 'AStar', 'auto').required()
            })
        }),
        // ----------------------------------------
        (req, res, next) => ctrl.createOperationPlan(req, res, next)
    );

    // US 4.1.5 - Regenerate Operation Plans
    route.post(
        '/regenerate',
        celebrate({
            body: Joi.object({
                date: Joi.string().required(),
                algorithm: Joi.string().valid('genetic', 'astar', 'Genetic', 'AStar', 'greedy').required(),
                author: Joi.string().required(),
                reason: Joi.string().required(),
                overwriteExisting: Joi.boolean().required().valid(true)
            })
        }),
        (req, res, next) => ctrl.regeneratePlans(req, res, next)
    );

    route.get(
        '/allocation',
        celebrate({
            query: Joi.object({
                resourceId: Joi.string().required(),
                start: Joi.string().optional(),
                end: Joi.string().optional()
            }).unknown(true)
        }),
        (req, res, next) => ctrl.getAllocation(req, res, next)
    );

    route.get(
        '/vvns-without-plan',
        celebrate({
            query: Joi.object({
                date: Joi.string().optional(),
                startDate: Joi.string().optional(),
                endDate: Joi.string().when('startDate', {
                    is: Joi.exist(),
                    then: Joi.string().required(),
                    otherwise: Joi.optional()
                })
            })
        }),
        (req, res, next) => ctrl.getVVNsWithoutPlan(req, res, next)
    );

    route.get(
        '/',
        (req, res, next) => ctrl.getOperationPlans(req, res, next)
    );

    // US 4.1.5 - Get VVNs without Operation Plans (ANTES de rotas com parâmetros!)
    route.get(
        '/missing',
        celebrate({
            query: Joi.object({
                date: Joi.string().optional(),
                startDate: Joi.string().optional(),
                endDate: Joi.string().when('startDate', {
                    is: Joi.exist(),
                    then: Joi.string().required(),
                    otherwise: Joi.optional()
                })
            })
        }),
        (req, res, next) => ctrl.getVVNsWithoutPlan(req, res, next)
    );

    route.get(
        '/date-range/:startDate/:endDate',
        (req, res, next) => ctrl.getOperationPlansByDateRange(req, res, next)
    );

    route.get(
        '/vessel/:vesselId',
        (req, res, next) => ctrl.getOperationPlansByVesselId(req, res, next)
    );

    route.get(
        '/ids/:id',
        (req, res, next) => ctrl.getOperationPlansById(req, res, next)
    );

    route.get(
        '/:id',
        (req, res, next) => ctrl.getOperationPlanById(req, res, next)
    );

    route.put(
        '/:id',
        (req, res, next) => ctrl.updateOperationPlan(req, res, next)
    );
};
