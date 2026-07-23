// src1/dto/vesselVE/IVVEMetricsDTO.ts
export interface IVVEMetricsDTO {
    turnaroundTime: number | null;
    berthOccupancyTime: number | null;
    waitingTimeForBerthing: number | null;

    // Atrasos
    arrivalDelay: number | null;
    departureDelay: number | null;
    operationDelays: number | null;

    // CAMPOS NOVOS (Correção do erro TS2353)
    totalOperations: number;
    completedOperations: number;
    delayedOperations: number;
    progressPercentage: number;
}