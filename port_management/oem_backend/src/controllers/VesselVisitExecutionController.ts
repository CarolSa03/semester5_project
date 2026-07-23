import {NextFunction, Request, Response} from 'express';
import {Inject, Service} from 'typedi';
import SearchVVEService from '../services/vesselVE/SearchVVEService';
import {IVVESearchFiltersDTO} from '../dto/vesselVE/IVVESearchFiltersDTO';
import RegisterDepartureService from "../services/vesselVE/RegisterDepartureService";
import CreateVesselVisitExecutionService from "../services/vesselVE/CreateVesselVisitExecutionService";

@Service()
export default class VesselVisitExecutionController {
    constructor(
        @Inject(() => SearchVVEService) private searchService: SearchVVEService,
        @Inject(() => CreateVesselVisitExecutionService) private createService: CreateVesselVisitExecutionService,
        @Inject(() => RegisterDepartureService) private departureService: RegisterDepartureService
    ) {}

    /**
     * POST /api/vessel-visit-executions
     */
    public async create(req: Request, res: Response, next: NextFunction) {
        try {
            const {vvnId, arrivalDate} = req.body;
            const result = await this.createService.execute(vvnId, arrivalDate);
            if (result.isFailure) return res.status(400).json({ error: result.error });
            return res.status(201).json(result.getValue());
        } catch (e) { return next(e); }
    }

    /**
     * GET /api/vessel-visit-executions/search
     */
    public async searchVVEs(req: Request, res: Response, next: NextFunction) {
        try {
            const filters: IVVESearchFiltersDTO = {
                startDate: req.query.startDate as string,
                endDate: req.query.endDate as string,
                vesselId: req.query.vesselId as string,
                status: req.query.status as string
            };
            const result = await this.searchService.execute(filters);
            if (result.isFailure) return res.status(400).json({ error: result.error });

            const vveResults = result.getValue();
            return res.status(200).json({ success: true, count: vveResults.length, data: vveResults });
        } catch (e) { return next(e); }
    }

    /**
     * NOVO: GET /api/vessel-visit-executions/:id
     * (Simulado usando o search ou retornando erro amigável se não encontrado)
     */
    public async getDetails(req: Request, res: Response, next: NextFunction) {
        try {
            const id = req.params.id;
            // TODO: Implementar GetByIdService real.
            // Workaround: Usar o search para encontrar em memória ou DB
            const result = await this.searchService.execute({});
            if (result.isFailure) return res.status(404).json({ error: "Not found" });

            const list = result.getValue();
            const found = list.find((v: any) => v.id === id);

            if (!found) return res.status(404).json({ error: `VVE with id ${id} not found` });
            return res.status(200).json(found);
        } catch (e) { return next(e); }
    }

    /**
     * NOVO: PATCH /api/vessel-visit-executions/:id/berth
     */
    public async registerBerthing(req: Request, res: Response, next: NextFunction) {
        try {
            const id = req.params.id;
            const { berthDate, dockId } = req.body;

            // TODO: Injetar RegisterBerthingService real.
            // Retorna sucesso falso para o frontend avançar
            console.log(`[STUB] Berthing registered for ${id}:`, berthDate, dockId);

            return res.status(200).json({
                id, status: 'IN_PROGRESS', berthDate, dockId, message: "Berthing registered (Stub)"
            });
        } catch (e) { return next(e); }
    }

    public async registerDeparture(req: Request, res: Response, next: NextFunction) {
        try {
            const id = req.params.id;
            const { departureDate } = req.body;

            // Call the real service
            const result = await this.departureService.execute(id, departureDate);

            if (result.isFailure) {
                return res.status(400).json({ error: result.error });
            }

            const dto = result.getValue();
            return res.status(200).json(dto);

        } catch (e) {
            return next(e);
        }
    }
}
