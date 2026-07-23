export interface IRegeneratePlansDTO {
    date: string; // YYYY-MM-DD
    algorithm: string; // 'genetic', 'greedy', etc.
    author: string;
    reason: string;
    overwriteExisting: boolean;
}
