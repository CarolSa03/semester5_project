import { ref } from "vue"

export interface Representative {
    id: string
    name: string
    email: string
    phone: string
    shippingAgentOrganizationId: string
    createdAt: string
}

export interface RepresentativeFormData {
    name: string
    email: string
    phone: string
    shippingAgentOrganizationId: string
}

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export function useRepresentativeApi() {
    const loading = ref(false)
    const error = ref<string | null>(null)

    const fetchRepresentatives = async (): Promise<Representative[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentRepresentative`)
            if (!response.ok) throw new Error("Failed to fetch representatives")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const fetchRepresentativeById = async (id: string): Promise<Representative> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentRepresentative/${id}`)
            if (!response.ok) throw new Error("Representative not found")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const createRepresentative = async (data: RepresentativeFormData): Promise<Representative> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentRepresentative`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || "Failed to create representative")
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const updateRepresentative = async (id: string, data: RepresentativeFormData): Promise<Representative> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentRepresentative/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || "Failed to update representative")
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const deleteRepresentative = async (id: string): Promise<void> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentRepresentative/${id}`, {
                method: "DELETE",
            })
            if (!response.ok) throw new Error("Failed to delete representative")
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    return {
        loading,
        error,
        fetchRepresentatives,
        fetchRepresentativeById,
        createRepresentative,
        updateRepresentative,
        deleteRepresentative,
    }
}
