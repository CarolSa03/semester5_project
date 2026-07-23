import { ref } from "vue"

export interface PhysicalResourceBase {
    id: string
    code: string
    description: string
    area: string
    setupTime: number
    operationalWindow: string
    requiredQualificationIds: string[]
    status: string
    capacity: string
    capacityUnit: string
}

export interface Truck extends PhysicalResourceBase {
    speed: string
    speedUnit: string
}

export interface YardCrane extends PhysicalResourceBase { }

export interface STSCrane extends PhysicalResourceBase { }

export type PhysicalResource = Truck | YardCrane | STSCrane

const API_BASE = (import.meta.env.VITE_API_BASE_URL || "/api") + "/PhysicalResource"

export function usePhysicalResourceApi() {
    const loading = ref(false)
    const error = ref<string>("")

    const listTrucks = async (): Promise<Truck[]> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/trucks`)
            if (!response.ok) throw new Error("Failed to fetch trucks")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to fetch trucks"
            throw err
        } finally {
            loading.value = false
        }
    }

    const listYardCrane = async (): Promise<YardCrane[]> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/yardcranes`)
            if (!response.ok) throw new Error("Failed to fetch yard cranes")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to fetch yard cranes"
            throw err
        } finally {
            loading.value = false
        }
    }

    const listSTSCranes = async (): Promise<STSCrane[]> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/stscranes`)
            if (!response.ok) throw new Error("Failed to fetch STS cranes")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to fetch STS cranes"
            throw err
        } finally {
            loading.value = false
        }
    }

    const getById = async (id: string): Promise<PhysicalResource> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/${id}`)
            if (!response.ok) throw new Error("Failed to fetch physical resource")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to fetch physical resource"
            throw err
        } finally {
            loading.value = false
        }
    }

    const createTruck = async (data: Omit<Truck, "id">): Promise<Truck> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/truck`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) throw new Error("Failed to create truck")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create truck"
            throw err
        } finally {
            loading.value = false
        }
    }

    const createYardCrane = async (data: Omit<YardCrane, "id">): Promise<YardCrane> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/yardcrane`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) throw new Error("Failed to create yard crane")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create yard crane"
            throw err
        } finally {
            loading.value = false
        }
    }

    const createSTSCrane = async (data: Omit<STSCrane, "id">): Promise<STSCrane> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/stscrane`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) throw new Error("Failed to create STS crane")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create STS crane"
            throw err
        } finally {
            loading.value = false
        }
    }

    const update = async (id: string, data: Partial<PhysicalResource>): Promise<PhysicalResource> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) throw new Error("Failed to update physical resource")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to update physical resource"
            throw err
        } finally {
            loading.value = false
        }
    }

    const deactivate = async (id: string): Promise<void> => {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/${id}/deactivate`, {
                method: "PATCH",
            })
            if (!response.ok) throw new Error("Failed to deactivate physical resource")
        } catch (err: any) {
            error.value = err.message || "Failed to deactivate physical resource"
            throw err
        } finally {
            loading.value = false
        }
    }

    return {
        loading,
        error,
        listTrucks,
        listYardCrane,
        listSTSCranes,
        getById,
        createTruck,
        createYardCrane,
        createSTSCrane,
        update,
        deactivate,
    }
}
