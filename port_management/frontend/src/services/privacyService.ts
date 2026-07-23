import axios from 'axios';

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api";

export interface PrivacyPolicy {
    id: string;
    content: string;
    version: number;
    createdAt: string;
    createdBy: string;
    isActive: boolean;
}

export default {
    getCurrent() {
        return axios.get<PrivacyPolicy>(`${API_BASE}/privacy`);
    },

    checkNotification() {
        return axios.get<{ needsNotification: boolean }>(`${API_BASE}/privacy/check`);
    },

    getHistory() {
        return axios.get<PrivacyPolicy[]>(`${API_BASE}/privacy/history`);
    },

    markViewed() {
        return axios.post(`${API_BASE}/privacy/view`);
    }
};
