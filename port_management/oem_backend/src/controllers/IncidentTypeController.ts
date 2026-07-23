import {NextFunction, Request, Response} from 'express';
import {Inject, Service} from 'typedi';
import CreateIncidentTypeService from '../services/incidentType/CreateIncidentTypeService';
import GetIncidentTypesService from "../services/incidentType/GetIncidentTypesService";
import UpdateIncidentTypeService from "../services/incidentType/UpdateIncidentTypeService";
import DeleteIncidentTypeService from "../services/incidentType/DeleteIncidentTypeService";
import {IIncidentTypeDTO} from '../dto/incidentType/IIncidentTypeDTO';

@Service()
export default class IncidentTypeController {
    constructor(
        @Inject(() => CreateIncidentTypeService) private createService: CreateIncidentTypeService,
        @Inject(() => GetIncidentTypesService) private getService: GetIncidentTypesService,
        @Inject(() => UpdateIncidentTypeService) private updateService: UpdateIncidentTypeService,
        @Inject(() => DeleteIncidentTypeService) private deleteService: DeleteIncidentTypeService
    ) {}

    public async create(req: Request, res: Response, next: NextFunction) {
        try {
            const dto = req.body as IIncidentTypeDTO;
            const result = await this.createService.execute(dto);

            if (result.isFailure) return res.status(400).json({ error: result.error });
            return res.status(201).json(result.getValue());
        } catch (e) { return next(e); }
    }

    public async list(req: Request, res: Response, next: NextFunction) {
        try {
            // Obtém parentId da query (?parentId=123)
            const parentId = req.query.parentId as string;

            const result = await this.getService.execute(parentId);

            if (result.isFailure) return res.status(400).json({ error: result.error });
            return res.json(result.getValue());
        } catch (e) { return next(e); }
    }

    public async update(req: Request, res: Response, next: NextFunction) {
        try {
            const id = req.params.id;
            const dto = req.body as Partial<IIncidentTypeDTO>;

            const result = await this.updateService.execute(id, dto);

            if (result.isFailure) return res.status(404).json({ error: result.error });
            return res.json(result.getValue());
        } catch (e) { return next(e); }
    }

    public async delete(req: Request, res: Response, next: NextFunction) {
        try {
            const id = req.params.id;

            const result = await this.deleteService.execute(id);

            if (result.isFailure) return res.status(404).json({ error: result.error });
            return res.status(200).send();
        } catch (e) { return next(e); }
    }
}