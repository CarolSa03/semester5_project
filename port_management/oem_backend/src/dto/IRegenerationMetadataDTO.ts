export interface IRegenerationMetadataDTO {
    regeneratedPlans: number;
    overwrittenPlans: number;
    newPlans: number;
    algorithm: string;
    author: string;
    timestamp: string; // ISO date string
}
