// src/services/usePrologApi.ts
import { ref } from 'vue'

const PROLOG_BASE = import.meta.env.VITE_PROLOG_BASE_URL || '/prolog'

export function usePrologApi() {
    const loading = ref(false)
    const error = ref<string | null>(null)

    // Health Check (Verifica se o servidor Prolog está online)
    const getHealth = async () => {
        loading.value = true
        error.value = null
        try {
            const res = await fetch(`${PROLOG_BASE}/health`)
            if (!res.ok) throw new Error(`Health check failed: ${res.status}`)
            return await res.json()
        } catch (err: any) {
            error.value = err.message
            throw err
        } finally {
            loading.value = false
        }
    }

    // US 4.3.2: Schedule Inteligente (Dispatcher)
    // Aceita { strategy: 'auto' | 'genetic' | 'optimal' | 'greedy' } dentro de data
    const schedule = async (data: any) => {
        loading.value = true
        error.value = null
        try {
            const res = await fetch(`${PROLOG_BASE}/schedule`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })

            // --- ALTERAÇÃO DE DEBUG AQUI ---
            if (!res.ok) {
                // Tenta ler a mensagem de erro enviada pelo Prolog
                const errorData = await res.json().catch(() => null)
                const errorMsg = errorData?.detail || errorData?.error || `Server Error ${res.status}`
                console.error("❌ ERRO BACKEND PROLOG:", errorData) // Vê isto na consola do browser!
                throw new Error(errorMsg)
            }
            // -------------------------------

            return await res.json()
        } catch (err: any) {
            error.value = err.message
            throw err
        } finally {
            loading.value = false
        }
    }

    // US 4.3.3: Rebalanceamento de Docas
    const rebalanceDocks = async (data: any) => {
        loading.value = true
        error.value = null
        try {
            const res = await fetch(`${PROLOG_BASE}/rebalance`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })
            if (!res.ok) throw new Error(`Rebalance error: ${res.status}`)
            return await res.json()
        } catch (err: any) {
            error.value = err.message
            throw err
        } finally {
            loading.value = false
        }
    }

    // --- Métodos Legacy / Helpers (Mantidos para compatibilidade) ---

    const scheduleGreedy = async (data: any) => {
        loading.value = true
        error.value = null
        try {
            const res = await fetch(`${PROLOG_BASE}/schedule/greedy`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })
            if (!res.ok) throw new Error(`Greedy error: ${res.status}`)
            return await res.json()
        } catch (err: any) {
            error.value = err.message
            throw err
        } finally {
            loading.value = false
        }
    }

    const scheduleMultiCrane = async (data: any) => {
        loading.value = true
        error.value = null
        try {
            const res = await fetch(`${PROLOG_BASE}/schedule/multi-crane`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })
            if (!res.ok) throw new Error(`Multi-crane error: ${res.status}`)
            return await res.json()
        } catch (err: any) {
            error.value = err.message
            throw err
        } finally {
            loading.value = false
        }
    }

    const compareHeuristics = async (data: any) => {
        loading.value = true
        error.value = null
        try {
            const res = await fetch(`${PROLOG_BASE}/schedule/compare`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })
            if (!res.ok) throw new Error(`Compare error: ${res.status}`)
            return await res.json()
        } catch (err: any) {
            error.value = err.message
            throw err
        } finally {
            loading.value = false
        }
    }

    const analyzeComplexity = async (data: any) => {
        loading.value = true
        error.value = null
        try {
            const res = await fetch(`${PROLOG_BASE}/schedule/complexity`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })
            if (!res.ok) throw new Error(`Complexity analysis error: ${res.status}`)
            return await res.json()
        } catch (err: any) {
            error.value = err.message
            throw err
        } finally {
            loading.value = false
        }
    }

    return {
        loading,
        error,
        getHealth,
        schedule,
        rebalanceDocks, // <--- Novo
        scheduleGreedy,
        scheduleMultiCrane,
        compareHeuristics,
        analyzeComplexity
    }
}