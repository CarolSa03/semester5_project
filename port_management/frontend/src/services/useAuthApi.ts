import { ref } from "vue"

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

export interface AppUser {
    id: string
    iamUserId: string
    email: string
    name: string
    isActive: boolean
    activatedOn: string | null
    createdAt: string
    updatedAt: string | null
    roles: string[]
    activationToken?: string
}

export interface CreateUserDto {
    iamUserId: string
    email: string
    name: string
}

export interface UpdateUserDto {
    name?: string
    email?: string
}

export interface AuditLog {
    id: string
    timestamp: string
    eventType: string
    performedBy: string
    affectedUser: string
    details: string
    ipAddress?: string
}

export function useAuthApi() {
    const loading = ref(false)
    const error = ref<string>("")

    /**
     * Login with email (no password)
     */
    async function loginWithEmail(email: string): Promise<AppUser> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/auth/login-email`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include", // Important for cookies!
                body: JSON.stringify({ email }),
            })
            if (!response.ok) {
                const errorData = await response.json()
                throw new Error(errorData.error || "Login failed")
            }
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Login failed"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Logout current user
     */
    async function logout(): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/auth/logout`, {
                method: "POST",
                credentials: "include",
            })
            if (!response.ok) throw new Error("Logout failed")
        } catch (err: any) {
            error.value = err.message || "Logout failed"
            throw err
        } finally {
            loading.value = false
        }
    }

    // ==================== ADMIN USER MANAGEMENT ====================

    /**
     * Create a new user (Admin only)
     */
    async function createUser(dto: CreateUserDto): Promise<AppUser> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(dto),
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to create user"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Get all users (Admin only)
     */
    async function getAllUsers(): Promise<AppUser[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users`, {
                credentials: "include",
            })
            if (!response.ok) throw new Error("Failed to load users")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load users"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Get user by ID (Admin only)
     */
    async function getUserById(id: string): Promise<AppUser> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users/${id}`, {
                credentials: "include",
            })
            if (!response.ok) throw new Error("User not found")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load user"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Update user (Admin only)
     */
    async function updateUser(id: string, dto: UpdateUserDto): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(dto),
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
        } catch (err: any) {
            error.value = err.message || "Failed to update user"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Assign role to user (Admin only)
     */
    async function assignRole(userId: string, role: string): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users/${userId}/roles`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({ role }),
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
        } catch (err: any) {
            error.value = err.message || "Failed to assign role"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Remove role from user (Admin only)
     */
    async function removeRole(userId: string, role: string): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users/${userId}/roles/${role}`, {
                method: "DELETE",
                credentials: "include",
            })
            if (!response.ok) {
                const errorText = await response.text()
                throw new Error(parseErrorMessage(errorText))
            }
        } catch (err: any) {
            error.value = err.message || "Failed to remove role"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Deactivate user (Admin only)
     */
    async function deactivateUser(userId: string): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users/${userId}/deactivate`, {
                method: "POST",
                credentials: "include",
            })
            if (!response.ok) throw new Error("Failed to deactivate user")
        } catch (err: any) {
            error.value = err.message || "Failed to deactivate user"
            throw err
        } finally {
            loading.value = false
        }
    }

    /**
     * Activate user (Admin only)
     */
    async function activateUser(userId: string): Promise<void> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/users/${userId}/activate`, {
                method: "POST",
                credentials: "include",
            })
            if (!response.ok) throw new Error("Failed to activate user")
        } catch (err: any) {
            error.value = err.message || "Failed to activate user"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function getSecurityAuditLogs(): Promise<AuditLog[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/audit/security`, {
                credentials: "include",
            })
            if (!response.ok) throw new Error("Failed to load security audit logs")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load audit logs"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function getFailedAuthAttempts(): Promise<AuditLog[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/audit/failures`, {
                credentials: "include",
            })
            if (!response.ok) throw new Error("Failed to load failed auth attempts")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load failed attempts"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function getUserAuditLogs(iamUserId: string): Promise<AuditLog[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/admin/audit/user/${iamUserId}`, {
                credentials: "include",
            })
            if (!response.ok) throw new Error("Failed to load user audit logs")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load user audit logs"
            throw err
        } finally {
            loading.value = false
        }
    }

    async function getAuditLogsByDateRange(startDate: string, endDate: string): Promise<AuditLog[]> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(
                `${API_BASE}/admin/audit/date-range?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}`,
                { credentials: "include" }
            )
            if (!response.ok) throw new Error("Failed to load audit logs")
            return await response.json()
        } catch (err: any) {
            error.value = err.message || "Failed to load audit logs"
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

    async function fetchCurrentUser(): Promise<AppUser | null> {
        loading.value = true
        error.value = ""
        try {
            const response = await fetch(`${API_BASE}/auth/user`, {
                credentials: "include",
            })
            if (!response.ok) {
                if (response.status === 401) {
                    error.value = "Unauthorized"
                    return null
                }
                throw new Error("Failed to fetch current user")
            }
            return await response.json() as AppUser
        } catch (err: any) {
            error.value = err.message || "Failed to fetch current user"
            return null
        } finally {
            loading.value = false
        }
    }

    return {
        loading,
        error,
        loginWithEmail,
        logout,
        createUser,
        getAllUsers,
        getUserById,
        updateUser,
        assignRole,
        removeRole,
        deactivateUser,
        activateUser,
        getSecurityAuditLogs,
        getFailedAuthAttempts,
        getUserAuditLogs,
        getAuditLogsByDateRange,
        fetchCurrentUser,
    }
}
