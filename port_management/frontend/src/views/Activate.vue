<template>
  <div class="activate-container">
    <div v-if="success" class="success-message">
      ✓ Your account has been activated! 
      <router-link to="/login">Login</router-link>
    </div>
    <div v-else-if="error" class="error-message">
      ✗ Activation failed: {{ error }} 
      <router-link to="/login">Go to login</router-link>
    </div>
    <div v-else class="loading-message">
      Activating your account...
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const API_BASE = import.meta.env.VITE_API_BASE_URL || "/api"

const route = useRoute()
const success = ref(false)
const error = ref<string | null>(null)

onMounted(async () => {
  const token = route.query.token
  const iamUserId = route.query.iamUserId
  if (!token || !iamUserId) {
    error.value = "Missing activation parameters."
    return
  }

  try {
    const response = await fetch(`${API_BASE}/auth/activate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        activationToken: token,
        iamUserId: iamUserId
      })
    })
    if (response.ok) {
      success.value = true
    } else {
      error.value = (await response.text()) || "Activation failed."
    }
  } catch (err) {
    error.value = "Could not reach the server."
  }
})
</script>

<style scoped>
.activate-container {
  max-width: 480px;
  margin: 5rem auto;
  padding: 1.5rem 1.8rem;
  background: #ffffff;
  border-radius: 0.8rem;
  box-shadow: 0 10px 30px rgba(15, 23, 42, 0.06);
  font-family: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
  text-align: center;
  color: #111827;
}

.success-message,
.error-message,
.loading-message {
  font-size: 1rem;
  line-height: 1.5;
  font-weight: 600;
}

.success-message {
  color: #059669;
}

.error-message {
  color: #dc2626;
}

.loading-message {
  color: #374151;
}

a {
  color: #2563eb;
  text-decoration: underline;
  font-weight: 500;
}

a:visited {
  color: #4c51bf;
}

a:hover {
  color: #1e40af;
  background: transparent;
}
</style>
