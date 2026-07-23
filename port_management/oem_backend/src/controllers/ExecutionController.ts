import {NextFunction, Request, Response} from 'express';
import {Inject, Service} from 'typedi';
import RegisterOperationService from '../services/vesselVE/RegisterOperationService';
import {IRegisterExecutionDTO} from '../dto/vesselVE/IRegisterExecutionDTO';

@Service()
export default class ExecutionController {
    constructor(
        @Inject(() => RegisterOperationService) private registerService: RegisterOperationService
    ) {}

    public async registerOperation(req: Request, res: Response, next: NextFunction) {
        try {
            const dto = req.body as IRegisterExecutionDTO;

            // Podes adicionar validações extra aqui se o DTO vier incompleto
            if (!dto.vvnId || !dto.operationId) {
                return res.status(400).json({ error: "Missing required fields (vvnId or operationId)." });
            }

            const result = await this.registerService.execute(dto);

            if (result.isFailure) {
                return res.status(400).json({ error: result.error });
            }

            return res.status(200).json({ message: "Operation execution registered successfully." });
        } catch (e) {
            return next(e);
        }
    }
}