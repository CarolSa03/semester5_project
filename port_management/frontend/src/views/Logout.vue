<template>
  <div class="logout-container">
    <div class="logout-card">
      <h1>You've been logged out</h1>
      <p>Thank you for using Port Management.</p>
      <button @click="goToLogin" class="login-button">
        Go to Login
      </button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useUserStore } from '@/services/useUserStore'
import { useAuthApi } from '@/services/useAuthApi'
import { onMounted } from 'vue'

const router = useRouter()
const userStore = useUserStore()
const authApi = useAuthApi()

onMounted(async () => {
  try {
    await authApi.logout()
  } catch (error) {
    console.error('Logout error:', error)
  } finally {
    userStore.clearUser()
  }
})

const goToLogin = () => {
  router.push({ name: 'login' })
}
</script>

<style scoped>
.logout-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: #fff;
}

.logout-card {
  background: #fff;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(50, 50, 50, 0.12);
  text-align: center;
  max-width: 340px;
}

h1 {
  font-size: 1.5rem;
  color: #222;
  margin-bottom: 1rem;
}

p {
  font-size: 1rem;
  color: #555;
  margin-bottom: 2rem;
}

.login-button {
  background: #eee;
  color: #222;
  border: 1px solid #ccc;
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 600;
  transition: background 0.15s;
}

.login-button:hover {
  background: #ddd;
}
</style>
