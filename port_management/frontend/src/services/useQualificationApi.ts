import { ref } from "vue"

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export interface Qualification {
    id: string;
    code: string;
    description: string;
}


export function useQualificationApi() {
    const loading = ref(false)
    const error = ref<string>("")

    async function createQualification(qualification: Qualification): Promise<Qualification> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Qualification`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    code: qualification.code,
                    description: qualification.description,
                })

            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create qualification"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function listQualifications(): Promise<Qualification[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Qualification`)
            if (!response.ok) throw new Error("Failed to load qualifications")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load qualifications"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function getQualification(code: string): Promise<Qualification> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Qualification/${code}`)
            if (!response.ok) throw new Error("Qualification not found")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load qualification"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function updateQualification(code: string, qualification: Qualification): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Qualification/${code}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    code: qualification.code,
                    description: qualification.description,
                })
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
        } catch (err: any) {
            error.value = err.message || "Failed to update qualification"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function deleteQualification(code: string): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Qualification/${code}`, {
                method: "DELETE",
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
        } catch (err: any) {
            error.value = err.message || "Failed to delete qualification"
            throw err
        } finally {
            loading.value = false
        }
    }


    function parseErrorMessage(errorText: string): string {
        try {
            const errorJson = JSON.parse(errorText)
            return errorJson.message || errorJson.error || "An error occurred"
        } catch {
            return errorText || "An error occurred"
        }
    }

    return {
        loading,
        error,
        createQualification,
        listQualifications,
        getQualification,
        updateQualification,
        deleteQualification,
    }
}
