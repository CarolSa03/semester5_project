import {VesselVisitExecution} from "./VesselVisitExecution";

export interface IVesselVisitExecutionRepo {
    save(vve: VesselVisitExecution): Promise<void>;
    findByVVN(vvnId: string): Promise<VesselVisitExecution | null>;
    findById(id: string): Promise<VesselVisitExecution | null>;
    
    // US 4.1.10 - Pesquisa com filtros
    findByFilters(filters: {
        startDate?: Date;
        endDate?: Date;
        vesselId?: string;
        status?: string;
    }): Promise<VesselVisitExecution[]>;
}