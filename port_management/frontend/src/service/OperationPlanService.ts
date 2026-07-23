import axios from 'axios';
import { IOperationPlan, IGenerateOperationPlanRequest, IUpdateOperationPlanRequest } from '@/entities/OperationPlan';

const OEM_BASE_URL = import.meta.env.VITE_OEM_BASE_URL || '/oem';

export class OperationPlanService {

    // US 4.1.2 - Generate
    async generateOperationPlan(request: any): Promise<IOperationPlan> {
        try {
            const response = await axios.post<IOperationPlan>(
                `${OEM_BASE_URL}/operation-plans/generate`,
                request
            );
            return response.data;
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to generate plan');
        }
    }

    // US 4.1.5 - Missing Plans (CORRIGIDO)
    async getMissingOperationPlans(startDate?: string, endDate?: string): Promise<any[]> {
        try {
            const params: any = {};
            if (startDate) params.startDate = startDate;
            if (endDate) params.endDate = endDate;

            // Rota correta: /vvns-without-plan
            const response = await axios.get<any[]>(
                `${OEM_BASE_URL}/operation-plans/vvns-without-plan`,
                { params }
            );
            return response.data;
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to fetch missing plans');
        }
    }

    // Auxiliar: Get Plan by VVN
    async getPlanByVVN(vvnId: string): Promise<IOperationPlan | null> {
        try {
            // Assume que existe uma rota de search ou listagem
            const response = await axios.get<IOperationPlan[]>(`${OEM_BASE_URL}/operation-plans`, {
                params: { vvnId: vvnId }
            });
            return response.data.length > 0 ? response.data[0] : null;
        } catch (error: any) {
            return null;
        }
    }

    // Listar todos (se necessário)
    async getAllOperationPlans(): Promise<IOperationPlan[]> {
        // Se a rota GET /operation-plans não existir no backend, isto falhará.
        // Confirmar operationPlanRoute.ts
        const response = await axios.get<IOperationPlan[]>(`${OEM_BASE_URL}/operation-plans`);
        return response.data;
    }

    async getOperationPlansByDateRange(startDate: string, endDate: string): Promise<IOperationPlan[]> {
        try {
            // Assume que o backend aceita parâmetros de data
            const response = await axios.get<IOperationPlan[]>(`${OEM_BASE_URL}/operation-plans`, {
                params: {
                    startDate: startDate,
                    endDate: endDate
                }
            });
            return response.data;
        } catch (error: any) {
            // Fallback: Se falhar, busca todos e filtra no cliente (opcional)
            console.warn("Date filter failed on server, fetching all.");
            const all = await this.getAllOperationPlans();
            // Filtragem simples cliente-side
            return all.filter(p => p.date >= startDate && p.date <= endDate);
        }
    }

    async getOperationPlanById(id: string): Promise<IOperationPlan> {
        try {
            // Rota no Backend: GET /oem/operation-plans/:id
            const response = await axios.get<IOperationPlan>(
                `${OEM_BASE_URL}/operation-plans/${id}`
            );
            return response.data;
        } catch (error: any) {
            console.error("Error fetching plan details:", error);
            throw new Error(error.response?.data?.error || 'Failed to fetch plan details');
        }
    }

    async updateOperationPlan(id: string, planData: any): Promise<IOperationPlan> {
        try {
            // Rota no Backend: PUT /oem/operation-plans/:id
            const response = await axios.put<IOperationPlan>(
                `${OEM_BASE_URL}/operation-plans/${id}`,
                planData
            );
            return response.data;
        } catch (error: any) {
            console.error("Error updating plan:", error);
            throw new Error(error.response?.data?.error || 'Failed to update plan');
        }
    }

    async getAllocation(resourceId: string, start: string, end: string): Promise<any[]> {
        if (!resourceId) return [];

        const res = await axios.get(`${OEM_BASE_URL}/operation-plans/allocation`, {
            params: { resourceId, start, end }
        });

        return res.data.map((op: any) => ({
            ...op,
            startTime: new Date(op.startTime || op.start),
            endTime:   new Date(op.endTime   || op.end),
            start:     new Date(op.startTime || op.start),
            end:       new Date(op.endTime   || op.end)
        }));
    }
}

export default new OperationPlanService();
