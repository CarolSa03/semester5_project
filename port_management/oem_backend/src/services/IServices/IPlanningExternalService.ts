import {Result} from "../../core/logic/Result";
import {IOperationPlanDTO} from "../../dto/IOperationPlanDTO";

export interface IPlanningExternalService {
    // Alinhado para retornar Result e permitir algoritmo opcional
    fetchSchedule(date: Date, algorithm?: string, vvnId?: string): Promise<Result<IOperationPlanDTO>>;
}