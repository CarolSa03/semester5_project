import axios from 'axios';

// Ajusta para a porta 3000 (Backend Node)
// const OEM_BASE_URL = import.meta.env.VITE_OEM_BASE_URL || '/oem';
const OEM_BASE_URL = "/oem";

// --- Interfaces ---

export interface IVVEMetrics {
    turnaroundTime: number | null;
    berthOccupancyTime: number | null;
    waitingTimeForBerthing: number | null;
    arrivalDelay: number | null;
    departureDelay: number | null;
    operationDelays: number | null;
    totalOperations: number;
    completedOperations: number;
    delayedOperations: number;
    progressPercentage: number;
}

export interface IExecutedOperation {
    operationId: string;
    realStartTime: string;
    realEndTime: string;
    realResource: string;
    status: 'COMPLETED' | 'DELAYED';
}

export interface IVesselVisit {
    id: string;
    vvnId: string;
    vesselName?: string;
    vesselId?: string; // Adicionado
    status: 'PENDING' | 'IN_PROGRESS' | 'COMPLETED';

    // Datas Reais
    arrivalDate?: string;
    berthDate?: string;
    departureDate?: string;
    unberthDate?: string;

    // Datas Previstas (Adicionadas para corrigir o erro TS2339)
    expectedArrival?: string;
    expectedDeparture?: string;

    // Locais
    dockId?: string;       // Legado ou usado internamente
    assignedDock?: string; // Vindo do Porto (Adicionado)

    // Estruturas Complexas
    executedOperations?: IExecutedOperation[];
    metrics?: IVVEMetrics; // (Adicionado para corrigir o erro TS2339)
}

export interface ITaskCategory {
    code: string;
    name: string;
    description?: string;
}

export interface IComplementaryTask {
    id: string;
    categoryCode: string;
    description: string;
    startTime: string;
    endTime: string;
}

export interface IRegisterExecutionDTO {
    vvnId: string;
    operationId: string;
    realStartTime: string;
    realEndTime: string;
    realResource: string;
    status: 'COMPLETED' | 'DELAYED';
    completedBy: string;
}

export class VesselVisitService {

    // --- US 4.1.11: COMPLETE VISIT ---
    async completeVisit(vvnId: string, departureTime: string): Promise<IVesselVisit> {
        // PATCH /oem/vessel-visit-executions/:id/depart
        const res = await axios.patch(`${OEM_BASE_URL}/vessel-visit-executions/${vvnId}/depart`, {
            departureDate: departureTime
        });
        return res.data;
    }

    // --- Ciclo de Vida (Vessel Visits) ---

    async getAll(filters?: any): Promise<IVesselVisit[]> {
        // Rota correta: /vessel-visit-executions/search
        const res = await axios.get(`${OEM_BASE_URL}/vessel-visit-executions/search`, { params: filters });

        // O backend retorna { success: true, data: [...] } ou diretamente o array dependendo da implementação final do Controller.
        // O searchVVEs no controller devolve { success: true, data: [...] }
        return res.data.data || res.data;
    }

    async getById(id: string): Promise<IVesselVisit> {
        // Rota correta: /vessel-visit-executions/:id
        const res = await axios.get(`${OEM_BASE_URL}/vessel-visit-executions/${id}`);
        return res.data;
    }

    async registerArrival(vvnId: string, arrivalDate: string): Promise<IVesselVisit> {
        // Rota correta: POST /vessel-visit-executions
        const res = await axios.post(`${OEM_BASE_URL}/vessel-visit-executions`, { vvnId, arrivalDate });
        return res.data;
    }

    async registerBerthing(id: string, berthDate: string, dockId: string): Promise<IVesselVisit> {
        // Rota correta: PATCH /:id/berth
        const res = await axios.patch(`${OEM_BASE_URL}/vessel-visit-executions/${id}/berth`, { berthDate, dockId });
        return res.data;
    }

    async registerDeparture(id: string, unberthDate: string, departureDate: string): Promise<IVesselVisit> {
        // Rota correta: PATCH /:id/depart
        const res = await axios.patch(`${OEM_BASE_URL}/vessel-visit-executions/${id}/depart`, { unberthDate, departureDate });
        return res.data;
    }

    // --- Execução de Operações de Carga ---

    async registerOperationExecution(data: IRegisterExecutionDTO): Promise<void> {
        // Rota correta: /executions/register
        await axios.post(`${OEM_BASE_URL}/executions/register`, data);
    }

    // --- Tarefas Complementares ---

    async getTaskCategories(): Promise<ITaskCategory[]> {
        const res = await axios.get(`${OEM_BASE_URL}/complementary-task-categories`);
        return res.data;
    }

    async createTaskCategory(data: Partial<ITaskCategory>): Promise<void> {
        await axios.post(`${OEM_BASE_URL}/complementary-task-categories`, data);
    }

    async addTask(data: any): Promise<void> {
        await axios.post(`${OEM_BASE_URL}/complementary-tasks`, data);
    }

    async getTasks(vveId: string): Promise<IComplementaryTask[]> {
        const res = await axios.get(`${OEM_BASE_URL}/complementary-tasks`, { params: { vveId } });
        return res.data;
    }

    // --- Estatísticas ---

    async getResourceStats(resourceId: string, start: string, end: string): Promise<any> {
        const res = await axios.get(`${OEM_BASE_URL}/stats/resources`, {
            params: { resourceId, start, end }
        });
        return res.data;
    }
}

export default new VesselVisitService();