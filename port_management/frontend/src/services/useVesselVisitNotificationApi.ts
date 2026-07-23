import { ref } from "vue";

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api";

export interface VesselVisitNotification {
    id?: string;
    businessId: string;
    vesselId: string;
    shippingAgentRepresentativeId: number;
    eta: string;
    etd: string;
    cargoManifestsId: string[];
    crewId: string;
    status: string;
    approvedByOfficerId?: string;
    rejectedByOfficerId?: string;
    approvedAt?: string;
    rejectedAt?: string;
    approvalNotes?: string;
    assignedDockId?: string;
    rejectionReason?: string;
    createdAt?: string;
    updatedAt?: string;
}

export function useVesselVisitNotificationApi() {
    const loading = ref(false);
    const error = ref<string | null>(null);

    // GET all notifications
    async function listNotifications(): Promise<VesselVisitNotification[]> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications`);
            if (!response.ok) throw new Error("Failed to load notifications");
            return await response.json();
        } catch (err: any) {
            error.value = err.message || "Failed to load notifications";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    // GET individual notification
    async function getNotification(businessId: string): Promise<VesselVisitNotification> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications/${businessId}`);
            if (!response.ok) throw new Error("Notification not found");
            return await response.json();
        } catch (err: any) {
            error.value = err.message || "Failed to load notification";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    // CREATE new notification
    async function createNotification(data: Omit<VesselVisitNotification, "id">): Promise<VesselVisitNotification> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            });
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Failed to create notification");
            }
            return await response.json();
        } catch (err: any) {
            error.value = err.message || "Failed to create notification";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    // UPDATE notification
    async function updateNotification(businessId: string, data: Partial<VesselVisitNotification>): Promise<void> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications/${businessId}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            });
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Failed to update notification");
            }
        } catch (err: any) {
            error.value = err.message || "Failed to update notification";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    // DELETE notification
    async function deleteNotification(businessId: string): Promise<void> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications/${businessId}`, {
                method: "DELETE",
            });
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Failed to delete notification");
            }
        } catch (err: any) {
            error.value = err.message || "Failed to delete notification";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    // PATCH helpers
    async function submitNotification(businessId: string): Promise<void> {
        await patchNotification(`${businessId}/submit`);
    }
    async function withdrawNotification(businessId: string): Promise<void> {
        await patchNotification(`${businessId}/withdraw`);
    }
    async function approveNotification(businessId: string): Promise<void> {
        await patchNotification(`${businessId}/approve`);
    }
    async function rejectNotification(businessId: string): Promise<void> {
        await patchNotification(`${businessId}/reject`);
    }
    async function patchNotification(endpoint: string): Promise<void> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications/${endpoint}`, {
                method: "PATCH",
            });
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || "Failed to patch notification");
            }
        } catch (err: any) {
            error.value = err.message || "Failed to patch notification";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    // GET filtered or searched
    async function listPending(): Promise<VesselVisitNotification[]> {
        return await fetchNotificationsByUrl("pending");
    }
    async function listByAgent(agentId: string): Promise<VesselVisitNotification[]> {
        return await fetchNotificationsByUrl(`agent/${agentId}`);
    }
    async function listByStatus(status: string): Promise<VesselVisitNotification[]> {
        return await fetchNotificationsByUrl(`status/${status}`);
    }
    async function searchNotifications(query: string): Promise<VesselVisitNotification[]> {
        return await fetchNotificationsByUrl(`search?q=${encodeURIComponent(query)}`);
    }
    async function advancedSearchNotifications(params: object): Promise<VesselVisitNotification[]> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications/search/advanced`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(params),
            });
            if (!response.ok) throw new Error("Failed to search notifications");
            return await response.json();
        } catch (err: any) {
            error.value = err.message || "Failed to search notifications";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    async function fetchNotificationsByUrl(suffix: string): Promise<VesselVisitNotification[]> {
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch(`${API_BASE}/vessel-visit-notifications/${suffix}`);
            if (!response.ok) throw new Error("Failed to load notifications");
            return await response.json();
        } catch (err: any) {
            error.value = err.message || "Failed to load notifications";
            throw err;
        } finally {
            loading.value = false;
        }
    }

    return {
        loading,
        error,
        listNotifications,
        getNotification,
        createNotification,
        updateNotification,
        deleteNotification,
        submitNotification,
        withdrawNotification,
        approveNotification,
        rejectNotification,
        listPending,
        listByAgent,
        listByStatus,
        searchNotifications,
        advancedSearchNotifications,
    };
}
