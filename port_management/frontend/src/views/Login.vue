<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthApi } from '@/services/useAuthApi'
import { useUserStore } from '@/services/useUserStore'

const router = useRouter()
const { loginWithEmail, loading, error: apiError } = useAuthApi()
const { setUser } = useUserStore()

const RETURN_URL = import.meta.env.VITE_RETURN_URL || '/dashboard'

const email = ref<string>('')
const error = ref<string | null>(null)
const success = ref<string | null>(null)

const handleLogin = async () => {
  error.value = null
  success.value = null

  try {
    const userData = await loginWithEmail(email.value)
    
    setUser(userData)
    
    success.value = `Welcome, ${userData.name}!`
    
    setTimeout(() => {
      router.push(RETURN_URL)
    }, 500)

  } catch (err: any) {
    error.value = apiError.value || err.message || 'Something went wrong. Please try again.'
    console.error('Login error:', err)
  }
}
</script>

<template>
  <div class="login-container">
    <div class="login-card">
      <h1>PORT MANAGEMENT</h1>
      <p>Sign in with your email address</p>

      <form @submit.prevent="handleLogin">
        <input
          v-model="email"
          type="email"
          placeholder="Enter your email"
          class="email-input"
          required
          :disabled="loading"
        />

        <button type="submit" class="login-btn" :disabled="loading || !email">
          <span v-if="!loading">SIGN IN</span>
          <span v-else>SIGNING IN...</span>
        </button>
      </form>

      <div v-if="error" class="error">
        {{ error }}
      </div>

      <div v-if="success" class="success">
        {{ success }}
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: #f4f5f7;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  color: #111827;
  padding: 1rem;
}

.login-card {
  background: #ffffff;
  padding: 2rem 2.5rem;
  text-align: center;
  max-width: 360px;
  width: 100%;
  border-radius: 0.9rem;
  box-shadow: 0 10px 30px rgba(15, 23, 42, 0.06);
}

h1 {
  color: #111827;
  margin-bottom: 0.75rem;
  font-size: 1.4rem;
  font-weight: 600;
  text-transform: none;
}

p {
  color: #6b7280;
  margin-bottom: 1.5rem;
  font-size: 1rem;
}

form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.email-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-family: inherit;
  transition: border-color 0.15s ease, box-shadow 0.15s ease;
}

.email-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.email-input:disabled {
  background: #f9fafb;
  cursor: not-allowed;
}

.login-btn {
  display: inline-block;
  width: 100%;
  padding: 0.75rem 1rem;
  background: #2563eb;
  color: #ffffff;
  border: none;
  border-radius: 999px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: background 0.15s ease, box-shadow 0.15s ease;
  text-transform: none;
  font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}

.login-btn:hover:not(:disabled) {
  background: #1d4ed8;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.4);
}

.login-btn:active:not(:disabled) {
  background: #3b82f6;
  box-shadow: none;
  transform: translateY(1px);
}

.login-btn:disabled {
  cursor: not-allowed;
  opacity: 0.6;
  background: #9ca3af;
}

.error {
  background: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
  margin-top: 1rem;
  font-size: 0.9rem;
}

.success {
  background: #f0fdf4;
  color: #16a34a;
  border: 1px solid #bbf7d0;
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
  margin-top: 1rem;
  font-size: 0.9rem;
}
</style>
