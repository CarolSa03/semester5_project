// US 4.1.10 - Filtros para pesquisa de VVEs
export interface IVVESearchFiltersDTO {
    startDate?: string;     // ISO Date String (ex: "2026-01-01")
    endDate?: string;       // ISO Date String
    vesselId?: string;      // Filtro por navio específico
    status?: string;        // "IN_PROGRESS" | "COMPLETED"
}
