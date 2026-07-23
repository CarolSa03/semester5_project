<template>
  <div class="dashboard-container">
    <div class="dashboard-content">
      <div class="card">
        <h2>Current User Info</h2>

        <div v-if="loading" class="loading">Loading user info...</div>
        <div v-else-if="error" class="error">{{ error }}</div>
        <div v-else-if="user">
          <p><strong>Name:</strong> {{ user.name }}</p>
          <p><strong>Email:</strong> {{ user.email }}</p>
          <p><strong>Active:</strong> 
            <span :class="user.isActive ? 'badge badge-active' : 'badge badge-inactive'">
              {{ user.isActive ? 'Yes' : 'No' }}
            </span>
          </p>
          <p><strong>Roles:</strong> {{ user.roles.join(", ") || "-" }}</p>
        </div>
        <div v-else>
          <p class="empty-state">No user logged in</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue"
import { useAuthApi, type AppUser } from "@/services/useAuthApi"

const { fetchCurrentUser, loading, error } = useAuthApi()
const user = ref<AppUser | null>(null)

async function loadUser() {
  user.value = await fetchCurrentUser()
}

onMounted(loadUser)
</script>

<style scoped>
@import '../assets/dashboard.css';

h2 {
  color: #1e3a5f;
  margin-bottom: 1rem;
}

.empty-state {
  color: #6b7280;
  font-style: italic;
  margin-top: 1rem;
}
</style>
