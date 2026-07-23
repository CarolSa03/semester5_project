// src1/dto/vesselVE/IVVESearchResultDTO.ts
import {IVVEMetricsDTO} from "./IVVEMetricsDTO";

export interface IExecutedOperationResultDTO {
    operationId: string;
    status: string;
    realStartTime: string;
    realEndTime: string;
    realResource: string;
}

export interface IVVESearchResultDTO {
    id: string;                         // <--- OBRIGATÓRIO para a tabela do Vue
    vvnId: string;
    vesselName?: string;
    vesselId?: string;
    status: string;
    arrivalDate: string | null;
    departureDate: string | null;
    expectedArrival?: string | null;
    expectedDeparture?: string | null;
    assignedDock?: string;
    metrics: IVVEMetricsDTO;
    executedOperations: IExecutedOperationResultDTO[];
}