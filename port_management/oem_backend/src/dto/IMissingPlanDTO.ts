export interface IMissingPlanDTO {
    vvnId: string;
    vesselName?: string;
    expectedArrival?: string; // ISO date string
    detectedAt: string; // ISO date string
}
