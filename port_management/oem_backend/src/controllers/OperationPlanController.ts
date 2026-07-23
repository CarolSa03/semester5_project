import {NextFunction, Request, Response} from 'express';
import {Inject, Service} from 'typedi';
import CreateOperationPlanService from '../services/operationPlan/CreateOperationPlanService';
import SearchOperationPlanService from '../services/operationPlan/SearchOperationPlanService';
import UpdateOperationPlanService from '../services/operationPlan/UpdateOperationPlanService';
import GetVVNsWithoutPlanService from '../services/operationPlan/GetVVNsWithoutPlanService';
import RegeneratePlansService from '../services/operationPlan/RegeneratePlansService';
import {Result} from '../core/logic/Result';

@Service()
export default class OperationPlanController {
    constructor(
        @Inject(() => CreateOperationPlanService) private createService: CreateOperationPlanService,
        @Inject(() => SearchOperationPlanService) private searchService: SearchOperationPlanService,
        @Inject(() => UpdateOperationPlanService) private updateService: UpdateOperationPlanService,
        @Inject(() => GetVVNsWithoutPlanService) private missingPlanService: GetVVNsWithoutPlanService,
        @Inject(() => RegeneratePlansService) private regenerateService: RegeneratePlansService
    ) {}

    public async createOperationPlan(req: Request, res: Response, next: NextFunction) {
        try {
            console.log("📥 [Controller] Recebido:", req.body);

            const planData = {
                vvnId: req.body.vvnId,
                date: new Date(req.body.date), // Conversão CRÍTICA
                algorithm: req.body.algorithm
            };

            // Validação de segurança
            if (isNaN(planData.date.getTime())) {
                return res.status(400).json({ error: "Invalid date format." });
            }

            const result = await this.createService.execute(planData);

            if (result.isFailure) {
                return res.status(400).json({ error: result.error });
            }

            return res.status(201).json(result.getValue());
        } catch (e) {
            return next(e);
        }
    }
    public async getOperationPlans(req: Request, res: Response, next: NextFunction) {
        try {
            // 1. Verificar se existem Query Params de data (startDate, endDate)
            const { startDate, endDate } = req.query;

            if (startDate && endDate) {
                // Se existirem datas, usamos o serviço de filtragem
                console.log(`🔍 [Controller] Filtering plans from ${startDate} to ${endDate}`);

                const operationPlansOrError = await this.searchService.getOperationPlansByDateRange(
                    startDate as string,
                    endDate as string
                ) as Result<any>;

                if (operationPlansOrError.isFailure) {
                    return res.status(404).json({ error: operationPlansOrError.error }).send();
                }
                return res.status(200).json(operationPlansOrError.getValue());
            }

            // 2. Se não houver filtros, comportamento padrão (Listar Todos)
            const operationPlansOrError = await this.searchService.getOperationPlans() as Result<any>;

            if (operationPlansOrError.isFailure) {
                return res.status(404).json({ error: operationPlansOrError.error }).send();
            }

            res.status(200).json(operationPlansOrError.getValue());
        } catch (e) {
            return next(e);
        }
    }

    public async getOperationPlansByDateRange(req: Request, res: Response, next: NextFunction) {
        try {
            const { startDate, endDate } = req.params;
            const operationPlansOrError = await this.searchService.getOperationPlansByDateRange(startDate, endDate) as Result<any>;

            if (operationPlansOrError.isFailure) {
                return res.status(404).json({ error: operationPlansOrError.error }).send();
            }
            res.status(200).json(operationPlansOrError.getValue());
        } catch (e) {
            return next(e);
        }
    }

    public async getOperationPlansByVesselId(req: Request, res: Response, next: NextFunction) {
        try {
            const { vesselId } = req.params;
            const operationPlansOrError = await this.searchService.getOperationPlansByVesselId(vesselId) as Result<any>;
            if (operationPlansOrError.isFailure) {
                return res.status(404).json({ error: operationPlansOrError.error }).send();
            }
            res.status(200).json(operationPlansOrError.getValue());
        } catch (e) {
            return next(e);
        }
    }

    public async getOperationPlansById(req: Request, res: Response, next: NextFunction) {
        try {
            const { id } = req.params;

            const operationPlansOrError = await this.searchService.getOperationPlansById(id) as Result<any>;

            if (operationPlansOrError.isFailure) {
                return res.status(404).json({ error: operationPlansOrError.error }).send();
            }

            res.status(200).json(operationPlansOrError.getValue());
        } catch (e) {
            return next(e);
        }
    }

    public async getOperationPlanById(req: Request, res: Response, next: NextFunction) {
        try {
            const { id } = req.params;

            const operationPlanOrError = await this.searchService.getOperationPlanById(id) as Result<any>;

            if (operationPlanOrError.isFailure) {
                return res.status(404).json({ error: operationPlanOrError.error }).send();
            }
            res.status(200).json(operationPlanOrError.getValue());
        } catch (e) {
            return next(e);
        }
    }

    public async updateOperationPlan(req: Request, res: Response, next: NextFunction) {
        try {
            const { id } = req.params;
            const updateData = req.body;

            const result = await this.updateService.execute(id, updateData);

            if (result.isFailure) {
                return res.status(400).json({ error: result.error });
            }

            return res.status(200).json(result.getValue());
        } catch (e) {
            return next(e);
        }
    }

    // US 4.1.5 - Get VVNs without Operation Plans
    public async getVVNsWithoutPlan(req: Request, res: Response, next: NextFunction) {
        try {
            const { date, startDate, endDate } = req.query;

            const result = await this.missingPlanService.execute(
                date as string,
                startDate as string,
                endDate as string
            );

            if (result.isFailure) {
                return res.status(400).json({ error: result.error });
            }

            return res.status(200).json(result.getValue());
        } catch (e) {
            return next(e);
        }
    }

    // US 4.1.5 - Regenerate Operation Plans for a specific date
    public async regeneratePlans(req: Request, res: Response, next: NextFunction) {
        try {
            const result = await this.regenerateService.execute(req.body);

            if (result.isFailure) {
                return res.status(400).json({ error: result.error });
            }

            return res.status(200).json(result.getValue());
        } catch (e) {
            return next(e);
        }
    }

    public async getAllocation(req: Request, res: Response, next: NextFunction) {
        try {
            const { resourceId, start, end } = req.query;

            const result = await this.searchService.getAllocation(
                resourceId as string, 
                start as string, 
                end as string
            );

            return res.status(200).json(result);
        } catch (e) {
            return next(e);
        }
    }
}
