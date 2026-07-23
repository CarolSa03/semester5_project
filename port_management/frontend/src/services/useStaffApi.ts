import { ref } from "vue"

export interface Qualification {
    code: number
    descriptiveName: string
}

export interface StaffMember {
    id: string
    staffMemberId: string
    shortName: string
    email: string
    phoneNumber: string
    contactDetails?: string
    qualifications: Qualification[]
    isAvailable: boolean
}

export interface StaffFormData {
    id: string
    staffMemberId: string
    shortName: string
    email: string
    phoneNumber: string
    qualifications: string[]
    isAvailable: boolean
}

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export function useStaffApi() {
    const loading = ref(false)
    const error = ref<string | null>(null)

    const fetchStaff = async (): Promise<StaffMember[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember`)
            if (!response.ok) throw new Error("Failed to fetch staff members")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const fetchStaffAvailable = async (): Promise<StaffMember[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/available`)
            if (!response.ok) throw new Error("Failed to fetch available staff")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // GET /unavailable
    const fetchStaffUnavailable = async (): Promise<StaffMember[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/unavailable`)
            if (!response.ok) throw new Error("Failed to fetch unavailable staff")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // GET /guid/{id} (GUID lookup)
    const fetchStaffByGuid = async (id: string): Promise<StaffMember> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/guid/${id}`)
            if (!response.ok) throw new Error("Staff member not found")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // GET /id/{staffMemberId} (staffMemberId lookup)
    const fetchStaffByStaffMemberId = async (staffMemberId: string): Promise<StaffMember> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/id/${staffMemberId}`)
            if (!response.ok) throw new Error("Staff member not found")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // POST: create staff
    const createStaff = async (data: StaffFormData): Promise<StaffMember> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorText = await response.text()
                let errorMessage = "Failed to create staff member"
                try {
                    const errorData = JSON.parse(errorText)
                    errorMessage = errorData.message || errorData.error || errorMessage
                } catch {
                    errorMessage = errorText || errorMessage
                }
                throw new Error(errorMessage)
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // PATCH: update staff by GUID
    const updateStaff = async (id: string, data: StaffFormData): Promise<StaffMember> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/${id}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            })
            if (!response.ok) {
                const errorText = await response.text()
                let errorMessage = "Failed to update staff member"
                try {
                    const errorData = JSON.parse(errorText)
                    errorMessage = errorData.message || errorData.error || errorMessage
                } catch {
                    errorMessage = errorText || errorMessage
                }
                throw new Error(errorMessage)
            }
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // PATCH: deactivate by GUID
    const deactivateStaff = async (id: string): Promise<void> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/${id}/deactivate`, {
                method: "PATCH",
            })
            if (!response.ok) throw new Error("Failed to deactivate staff member")
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // PATCH: activate by GUID
    const activateStaff = async (id: string): Promise<void> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/${id}/activate`, {
                method: "PATCH",
            })
            if (!response.ok) throw new Error("Failed to activate staff member")
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // GET by name search
    const searchStaffByName = async (name: string): Promise<StaffMember[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/search/name?name=${encodeURIComponent(name)}`)
            if (!response.ok) throw new Error("Search by name failed")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    // GET by qualification search
    const searchStaffByQualification = async (qualification: string): Promise<StaffMember[]> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/search/qualification?qualification=${encodeURIComponent(qualification)}`)
            if (!response.ok) throw new Error("Search by qualification failed")
            return await response.json()
        } catch (err) {
            error.value = err instanceof Error ? err.message : "Unknown error"
            throw err
        } finally {
            loading.value = false
        }
    }

    const deleteStaff = async (id: string): Promise<void> => {
        loading.value = true
        error.value = null
        try {
            const response = await fetch(`${API_BASE}/StaffMember/${id}`, {
                method: "DELETE",
            })
            if (!response.ok) throw new Error("Failed to delete staff member")
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
        fetchStaff,
        fetchStaffAvailable,
        fetchStaffUnavailable,
        fetchStaffByGuid,
        fetchStaffByStaffMemberId,
        createStaff,
        updateStaff,
        deactivateStaff,
        activateStaff,
        searchStaffByName,
        searchStaffByQualification,
        deleteStaff,
    }
}
