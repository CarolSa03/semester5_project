<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useVesselRecordApi, type VesselRecord } from '../../services/useVesselRecordApi'

const router = useRouter()
const { searchVesselRecords } = useVesselRecordApi()

const vesselRecords = ref<VesselRecord[]>([])
const isLoading = ref(true)
const errorMessage = ref('')
const searchQuery = ref('')

const renderImo = (imo: string | { value: string }) => {
  if (typeof imo === 'object' && imo !== null && 'value' in imo) {
    return imo.value
  }
  return imo
}

const loadVesselRecords = async (query = '') => {
  isLoading.value = true
  errorMessage.value = ''
  try {
    vesselRecords.value = await searchVesselRecords(query)
  } catch (error) {
    errorMessage.value = 'Failed to load vessel records. Make sure the API is running.'
  } finally {
    isLoading.value = false
  }
}

const handleSearch = () => {
  loadVesselRecords(searchQuery.value.trim())
}

onMounted(() => loadVesselRecords())
</script>

<template>
  <div class="dashboard-content">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
      <h2>Vessel Records</h2>
      <button class="btn btn-primary" @click="router.push('/vessel-record/create')">
        + New Vessel Record
      </button>
    </div>

    <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

    <div class="card">
      <div style="margin-bottom: 1.5rem; display: flex; gap: 0.5rem;">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search by IMO, name, or owner..."
          style="flex: 1; padding: 0.5rem; border: 1px solid #ddd; border-radius: 6px;"
          @keydown.enter="handleSearch"
        />
        <button class="btn btn-secondary" @click="handleSearch">Search</button>
      </div>

      <div v-if="isLoading" class="loading">Loading vessel records...</div>

      <div v-else-if="vesselRecords.length === 0" class="empty-state">
        <h3>No vessel records found</h3>
        <p>Start by creating your first vessel record.</p>
        <button class="btn btn-primary" @click="router.push('/vessel-record/create')">
          + Create Vessel Record
        </button>
      </div>

      <table v-else>
        <thead>
          <tr>
            <th>IMO Number</th>
            <th>Vessel Name</th>
            <th>Type</th>
            <th>Owner</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="vessel in vesselRecords" :key="renderImo(vessel.imo)">
            <td>
              <span class="badge" style="background: #e0e7ff; color: #3730a3; font-weight: 600;">
                {{ renderImo(vessel.imo) }}
              </span>
            </td>
            <td>{{ vessel.name }}</td>
            <td>{{ vessel.vesselType ? vessel.vesselType.name : '-' }}</td>
            <td>{{ vessel.owner }}</td>
            <td>
              <span class="badge" :class="vessel.isActive ? 'badge-success' : 'badge-danger'">
                {{ vessel.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td>
              <div class="actions-cell">
                <button
                  class="btn btn-secondary btn-small"
                  @click="router.push(`/vessel-record/${encodeURIComponent(renderImo(vessel.imo))}`)"
                >
                  View
                </button>
                <button
                  class="btn btn-primary btn-small"
                  @click="router.push(`/vessel-record/${encodeURIComponent(renderImo(vessel.imo))}/edit`)"
                  disabled
                >
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
