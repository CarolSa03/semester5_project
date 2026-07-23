export interface ICreateVesselVisitExecutionDTO {
    vvnId: string;// ID da Vessel Visit Notification
    arrivalDate?: string; // ISO Date String (ex: "2026-01-01T12:00:00Z")
    status?: string; // "IN_PROGRESS" | "COMPLETED"
}