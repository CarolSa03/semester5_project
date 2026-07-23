<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useVesselTypeApi, type VesselType } from '../../services/useVesselTypeApi'

const router = useRouter()
const { searchVesselTypes } = useVesselTypeApi()

const vesselTypes = ref<VesselType[]>([])
const isLoading = ref(true)
const errorMessage = ref('')
const searchQuery = ref('')

const loadVesselTypes = async (query = '') => {
  isLoading.value = true
  errorMessage.value = ''
  try {
    vesselTypes.value = await searchVesselTypes(query)
  } catch (error) {
    errorMessage.value = 'Failed to load vessel types. Make sure the API is running.'
  } finally {
    isLoading.value = false
  }
}

const handleSearch = () => {
  loadVesselTypes(searchQuery.value.trim())
}

onMounted(() => loadVesselTypes())
</script>

<template>
  <div class="dashboard-content">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
      <h2>Vessel Types</h2>
      <button class="btn btn-primary" @click="router.push('/vessel-type/create')">
        + New Vessel Type
      </button>
    </div>

    <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

    <div class="card">
      <div style="margin-bottom: 1.5rem; display: flex; gap: 0.5rem;">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search name or description..."
          style="flex: 1; padding: 0.5rem; border: 1px solid #ddd; border-radius: 6px;"
          @keydown.enter="handleSearch"
        />
        <button class="btn btn-secondary" @click="handleSearch">Search</button>
      </div>

      <div v-if="isLoading" class="loading">Loading vessel types...</div>

      <div v-else-if="vesselTypes.length === 0" class="empty-state">
        <h3>No vessel types found</h3>
        <p>Start by creating your first vessel type.</p>
        <button class="btn btn-primary" @click="router.push('/vessel-type/create')">
          + Create Vessel Type
        </button>
      </div>

      <table v-else>
        <thead>
          <tr>
            <th>Name</th>
            <th>Capacity (TEU)</th>
            <th>Dimensions (R×B×T)</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="vt in vesselTypes" :key="vt.id">
            <td>
              <span class="badge" style="background: #e0e7ff; color: #3730a3; font-weight: 600;">
                {{ vt.name }}
              </span>
            </td>
            <td>{{ vt.capacityTEU ?? '-' }}</td>
            <td style="font-size: 0.875rem; color: #6b7280;">
              {{ [vt.maxRows ?? '-', vt.maxBays ?? '-', vt.maxTiers ?? '-'].join('×') }}
            </td>
            <td>
              <span class="badge" :class="vt.isActive ? 'badge-success' : 'badge-danger'">
                {{ vt.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td>
              <div class="actions-cell">
                <button class="btn btn-secondary btn-small" @click="router.push(`/vessel-type/${vt.id}`)">
                  View
                </button>
                <button class="btn btn-primary btn-small" @click="router.push(`/vessel-type/${vt.id}/edit`)">
                  Edit
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';

.badge-success {
  background: #d1fae5;
  color: #065f46;
}
.badge-danger {
  background: #fee2e2;
  color: #991b1b;
}
.actions-cell {
  display: flex;
  gap: 0.5rem;
}
</style>
