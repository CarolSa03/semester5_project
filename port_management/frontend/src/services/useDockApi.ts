import { ref } from "vue"

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export interface VesselType {
    id: string
    name: string
}

export interface Dock {
    id?: string
    name: string
    location: string
    length: number
    depth: number
    maxDraft: number
    maxSTS: number
    allowedVesselTypes: string[]
    stsCranes?: string[]
    isActive?: boolean
}

export function useDockApi() {
    const loading = ref(false)
    const error = ref<string>("")

    // Fetch docks optionally filtered by search query
    async function fetchDocks(searchQuery?: string): Promise<Dock[]> {
        loading.value = true
        error.value = ""
        try {
            let url = `${API_BASE}/Dock`
            if (searchQuery && searchQuery.trim().length > 0) {
                url = `${API_BASE}/Dock/search?q=${encodeURIComponent(searchQuery)}`
            }
            const response = await fetch(url)
            if (!response.ok) throw new Error("Failed to load docks")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load docks"
            throw err
        } finally {
            loading.value = false
        }
    }

    // Fetch a single dock by its ID
    async function fetchDockById(id: string): Promise<Dock> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Dock/${id}`)
            if (!response.ok) throw new Error("Dock not found")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load dock"
            throw err
        } finally {
            loading.value = false
        }
    }

    // Fetch all vessel types
    async function fetchVesselTypes(): Promise<VesselType[]> {
        try {
            const response = await fetch(`${API_BASE}/VesselType`)
            if (!response.ok) throw new Error("Failed to load vessel types")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load vessel types"
            throw err
        }
    }

    // Create a new dock
    async function createDock(dock: Dock): Promise<Dock> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Dock`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(formatDockPayload(dock)),
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create dock"
            throw err
        } finally {
            loading.value = false
        }
    }

    // Update an existing dock using PATCH
    async function updateDock(id: string, dock: Dock): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/Dock/${id}`, {
                method: "PATCH",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(formatDockPayload(dock)),
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
        } catch (err: any) {
            error.value = err.message || "Failed to update dock"
            throw err
        } finally {
            loading.value = false
        }
    }

    // Helper to format the payload sent to API
    function formatDockPayload(dock: Dock) {
        return {
            name: dock.name,
            location: dock.location,
            length: dock.length,
            depth: dock.depth,
            maxDraft: dock.maxDraft,
            maxSTS: dock.maxSTS,
            allowedVesselTypes: dock.allowedVesselTypes,
            stsCranes: dock.stsCranes || [],
            isActive: dock.isActive ?? true,
        }
    }

    // Helper to parse error messages from backend
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
        fetchDocks,
        fetchDockById,
        fetchVesselTypes,
        createDock,
        updateDock,
    }
}
