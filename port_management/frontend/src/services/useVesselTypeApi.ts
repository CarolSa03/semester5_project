import { ref } from "vue"

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export interface VesselType {
    id: string
    name: string
    description?: string | null
    capacityTEU?: number | null
    maxRows?: number | null
    maxBays?: number | null
    maxTiers?: number | null
    isActive: boolean
    createdAt?: string
    updatedAt?: string
}

export function useVesselTypeApi() {
    const loading = ref(false)
    const error = ref<string | null>(null)

    const listVesselTypes = async (): Promise<VesselType[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselType`)
            if (!response.ok) throw new Error("Failed to load vessel types")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const getVesselType = async (id: string): Promise<VesselType> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselType/${id}`)
            if (!response.ok) throw new Error("Vessel type not found")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const createVesselType = async (data: Partial<VesselType>): Promise<VesselType> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselType`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || "Failed to create vessel type")
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const updateVesselType = async (id: string, data: Partial<VesselType>): Promise<VesselType> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselType/${id}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || "Failed to update vessel type")
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const searchVesselTypes = async (query: string): Promise<VesselType[]> => {
        loading.value = true
        error.value = null
        try {
            const url = query ? `${API_BASE}/VesselType/search?q=${encodeURIComponent(query)}` : `${API_BASE}/VesselType`
            const response = await fetch(url)
            if (!response.ok) throw new Error("Failed to search vessel types")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const getActiveVesselTypes = async (): Promise<VesselType[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselType/active`)
            if (!response.ok) throw new Error("Failed to load active vessel types")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const deleteVesselType = async (id: string): Promise<void> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselType/${id}`, { method: "DELETE" })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || "Failed to delete vessel type")
            }
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const inactivateVesselType = async (id: string): Promise<VesselType> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselType/${id}/inactivate`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || "Failed to inactivate vessel type")
            }
            return await response.json()
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
        listVesselTypes,
        getActiveVesselTypes,
        getVesselType,
        createVesselType,
        updateVesselType,
        deleteVesselType,
        inactivateVesselType,
        searchVesselTypes,
    }
}
