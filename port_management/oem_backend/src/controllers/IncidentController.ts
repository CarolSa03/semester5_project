import {NextFunction, Request, Response} from 'express';
import {Inject, Service} from 'typedi';
import CreateIncidentService from '../services/incident/CreateIncidentService';
import UpdateIncidentService from '../services/incident/UpdateIncidentService';
import GetIncidentsService from '../services/incident/GetIncidentsService';
import DeleteIncidentService from '../services/incident/DeleteIncidentService'; // Importar
import {ICreateIncidentDTO} from '../dto/incident/ICreateIncidentDTO';
import {IUpdateIncidentDTO} from '../dto/incident/IUpdateIncidentDTO';

@Service()
export default class IncidentController {
    constructor(
        @Inject(() => CreateIncidentService) private createService: CreateIncidentService,
        @Inject(() => UpdateIncidentService) private updateService: UpdateIncidentService,
        @Inject(() => GetIncidentsService) private getService: GetIncidentsService,
        @Inject(() => DeleteIncidentService) private deleteService: DeleteIncidentService // Injetar
    ) {}

    public async createIncident(req: Request, res: Response, next: NextFunction) {
        try {
            const dto = req.body as ICreateIncidentDTO;
            const result = await this.createService.execute(dto);
            if (result.isFailure) return res.status(400).json({ error: result.error });
            return res.status(201).json(result.getValue());
        } catch (e) { return next(e); }
    }

    public async updateIncident(req: Request, res: Response, next: NextFunction) {
        try {
            const id = req.params.id;
            const dto = { ...req.body, id } as IUpdateIncidentDTO;
            const result = await this.updateService.execute(dto);
            if (result.isFailure) return res.status(404).json({ error: result.error });
            return res.status(200).json(result.getValue());
        } catch (e) { return next(e); }
    }

    public async getIncidents(req: Request, res: Response, next: NextFunction) {
        try {
            const filters = {
                status: req.query.status as 'active' | 'resolved',
                vvnId: req.query.vvnId as string,
                startDate: req.query.startDate as string,
                endDate: req.query.endDate as string,
                severity: req.query.severity as string
            };
            const result = await this.getService.execute(filters);
            if (result.isFailure) return res.status(400).json({ error: result.error });
            return res.status(200).json(result.getValue());
        } catch (e) { return next(e); }
    }

    // NOVO: Metodo em falta
    public async deleteIncident(req: Request, res: Response, next: NextFunction) {
        try {
            const id = req.params.id;
            const result = await this.deleteService.execute(id);

            if (result.isFailure) return res.status(404).json({ error: result.error });
            return res.status(204).send(); // 204 No Content
        } catch (e) { return next(e); }
    }
}