import { ref } from "vue"

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export interface StorageArea {
    id: string
    type: string
    location: string
    maxCapacityTEU: number
    currentCapacityTEU: number
    servedDockIds?: number[]
    notes?: string
}

export function useStorageAreaApi() {
    const loading = ref(false)
    const error = ref<string>("")

    async function listStorageAreas(): Promise<StorageArea[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/StorageArea`)
            if (!response.ok) throw new Error("Failed to load storage areas")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load storage areas"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function getStorageArea(id: string): Promise<StorageArea> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/StorageArea/${encodeURIComponent(id)}`)
            if (!response.ok) throw new Error("Storage area not found")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load storage area"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function createStorageArea(storageArea: Partial<StorageArea>): Promise<StorageArea> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/StorageArea`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(storageArea),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || errorData.error || "Failed to create storage area")
            }
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create storage area"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function updateStorageArea(id: string, storageArea: Partial<StorageArea>): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/StorageArea/${encodeURIComponent(id)}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(storageArea),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.message || errorData.error || "Failed to update storage area")
            }
        } catch (err: any) {
            error.value = err.message || "Failed to update storage area"
            throw err
        } finally {
            loading.value = false
        }
    }

    return {
        loading,
        error,
        listStorageAreas,
        getStorageArea,
        createStorageArea,
        updateStorageArea,
    }
}
