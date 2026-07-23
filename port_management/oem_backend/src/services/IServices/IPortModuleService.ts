export interface IVVNInfo {
    vvnId: string;
    vesselName?: string;
    vesselId?: string;      // US 4.1.10 - Para filtrar por vessel
    expectedArrival?: Date; // ETA
    expectedDeparture?: Date; // ETD - US 4.1.10
    assignedDock?: string;  // US 4.1.10 - Para mostrar dock atribuído
}

export interface IPortModuleService {
    fetchApprovedVVNs(date?: Date, startDate?: Date, endDate?: Date): Promise<IVVNInfo[]>;
    fetchVVNById(vvnId: string): Promise<IVVNInfo | null>;
}
