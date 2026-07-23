import axios from 'axios';

const OEM_BASE_URL = import.meta.env.VITE_OEM_BASE_URL || '/oem';

// Interfaces (podes mover para @/entities/Incident se preferires)
export interface IIncidentType {
    id: string;
    code: string;
    name: string;
    description: string;
    severity: 'Minor' | 'Major' | 'Critical';
    parentId?: string;
    active: boolean;
}

export interface IIncident {
    id: string;
    incidentTypeId: string;
    description: string;
    severity: string;
    startTime: string;
    endTime?: string;
    affectedVvnIds: string[];
    createdBy: string;
    durationMinutes?: number;
}

export interface ICreateIncidentDTO {
    incidentTypeCode: string;
    description: string;
    severity?: string;
    affectedVvnIds?: string[];
    createdBy: string;
}

export class IncidentService {

    // --- US 4.1.12: Gestão de Tipos de Incidente ---

    async getIncidentTypes(parentId?: string): Promise<IIncidentType[]> {
        try {
            const params = parentId ? { parentId } : {};
            const response = await axios.get<IIncidentType[]>(`${OEM_BASE_URL}/incident-types`, { params });
            return response.data;
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to fetch incident types');
        }
    }

    async createIncidentType(data: Partial<IIncidentType>): Promise<IIncidentType> {
        try {
            const response = await axios.post<IIncidentType>(`${OEM_BASE_URL}/incident-types`, data);
            return response.data;
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to create incident type');
        }
    }

    // --- US 4.1.13: Gestão de Incidentes (VVE) ---

    async getIncidentsByVVN(vvnId: string): Promise<IIncident[]> {
        try {
            // Assume que o endpoint suporta filtro ?vvnId=...
            const response = await axios.get<IIncident[]>(`${OEM_BASE_URL}/incidents`, {
                params: { vvnId }
            });
            return response.data;
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to fetch incidents');
        }
    }

    async createIncident(data: ICreateIncidentDTO): Promise<IIncident> {
        try {
            const response = await axios.post<IIncident>(`${OEM_BASE_URL}/incidents`, data);
            return response.data;
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to report incident');
        }
    }

    async resolveIncident(id: string, endTime: string): Promise<IIncident> {
        try {
            const response = await axios.put<IIncident>(`${OEM_BASE_URL}/incidents/${id}`, { endTime });
            return response.data;
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to resolve incident');
        }
    }

    // --- Incident Type delete ---
    async deleteIncidentType(id: string): Promise<void> {
        try {
            await axios.delete(`${OEM_BASE_URL}/incident-types/${id}`);
        } catch (error: any) {
            throw new Error(error.response?.data?.error || 'Failed to delete incident type');
        }
    }
}

export default new IncidentService();