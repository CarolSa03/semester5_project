import { ref } from 'vue'
import type { AppUser } from '@/services/useAuthApi'

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api'

const currentUser = ref<AppUser | null>(null)
const isLoading = ref(false)
const error = ref<string | null>(null)

export function useUserStore() {
    const fetchCurrentUser = async (): Promise<AppUser | null> => {
        isLoading.value = true
        error.value = null

        try {
            const response = await fetch(`${API_BASE}/auth/user`, {
                credentials: 'include'
            })

            if (response.ok) {
                const userData = await response.json()
                currentUser.value = userData
                return userData
            } else if (response.status === 401) {
                currentUser.value = null
                return null
            } else {
                throw new Error('Failed to fetch user')
            }
        } catch (err: any) {
            error.value = err.message || 'Failed to load user'
            currentUser.value = null
            return null
        } finally {
            isLoading.value = false
        }
    }
    const setUser = (user: AppUser) => {
        currentUser.value = user
    }

    const clearUser = () => {
        currentUser.value = null
    }
    const hasRole = (roleName: string): boolean => {
        return currentUser.value?.roles?.includes(roleName) ?? false
    }

    const isAuthenticated = (): boolean => {
        return currentUser.value !== null
    }

    return {
        currentUser,
        isLoading,
        error,
        fetchCurrentUser,
        setUser,
        clearUser,
        hasRole,
        isAuthenticated
    }
}
