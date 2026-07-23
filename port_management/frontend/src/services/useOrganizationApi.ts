import { ref } from "vue"

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export interface Organization {
    id?: string
    legalName: string
    alternativeName: string
    taxNumber: string
    createdAt?: string
    updatedAt?: string
}


export function useOrganizationApi() {
    const loading = ref(false)
    const error = ref<string>("")

    async function createOrganization(organization: Organization): Promise<Organization> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentOrganization`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    id: organization.id,
                    legalName: organization.legalName,
                    alternativeName: organization.alternativeName,
                    taxNumber: organization.taxNumber,
                    createdAt: organization.createdAt,
                }),
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create organization"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function listOrganizations(): Promise<Organization[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentOrganization`)
            if (!response.ok) throw new Error("Failed to load organizations")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load organizations"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function getOrganization(id: string): Promise<Organization> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentOrganization/${id}`)
            if (!response.ok) throw new Error("Organization not found")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load organization"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function updateOrganization(id: string, organization: Organization): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentOrganization/${id}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    Id: organization.id,
                    LegalName: organization.legalName,
                    AlternativeName: organization.alternativeName,
                    TaxNumber: organization.taxNumber,
                    CreatedAt: organization.createdAt
                }),

            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
        } catch (err: any) {
            error.value = err.message || "Failed to update organization"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function deleteOrganization(id: string): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/ShippingAgentOrganization/${id}`, {
                method: "DELETE",
            })
            if (!response.ok) throw new Error("Failed to delete organization")
        } catch (err: any) {
            error.value = err.message || "Failed to delete organization"
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
        createOrganization,
        listOrganizations,
        getOrganization,
        updateOrganization,
        deleteOrganization,
    }
}
