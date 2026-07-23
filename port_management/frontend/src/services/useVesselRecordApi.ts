import { ref } from "vue"

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export interface VesselRecord {
    imo: string | { value: string }
    name: string
    vesselTypeId?: string
    vesselType?: {
        id: string
        name: string
        description?: string
        capacityTEU?: number
        maxRows?: number
        maxBays?: number
        maxTiers?: number
    }
    owner: string
    isActive: boolean
    createdAt?: string
    updatedAt?: string
}

export function useVesselRecordApi() {
    const loading = ref(false)
    const error = ref<string | null>(null)

    const listVesselRecords = async (): Promise<VesselRecord[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselRecord`)
            if (!response.ok) throw new Error("Failed to load vessel records")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const getVesselRecord = async (imo: string): Promise<VesselRecord> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselRecord/${encodeURIComponent(imo)}`)
            if (!response.ok) throw new Error("Vessel record not found")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const createVesselRecord = async (data: {
        imoValue: string
        name: string
        vesselTypeId: string
        owner: string
    }): Promise<VesselRecord> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselRecord`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || errorData.error || "Failed to create vessel record")
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const updateVesselRecord = async (imo: string, data: Partial<VesselRecord>): Promise<VesselRecord> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/VesselRecord/${encodeURIComponent(imo)}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || "Failed to update vessel record")
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const searchVesselRecords = async (query: string): Promise<VesselRecord[]> => {
        loading.value = true
        error.value = null
        try {
            const url = query ? `${API_BASE}/VesselRecord/search?q=${encodeURIComponent(query)}` : `${API_BASE}/VesselRecord`
            const response = await fetch(url)
            if (!response.ok) throw new Error("Failed to search vessel records")
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
        listVesselRecords,
        getVesselRecord,
        createVesselRecord,
        updateVesselRecord,
        searchVesselRecords,
    }
}
