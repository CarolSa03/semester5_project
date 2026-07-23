<template>
    <div class="dashboard-content">
      <div class="actions">
        <div class="left-actions">
          <h2>Storage Areas</h2>
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Search by ID, type, or location..."
            style="margin-left: 1rem; padding: 0.5rem; width: 300px"
          />
        </div>
        <router-link to="/storage-area/create" class="btn btn-primary">+ New Storage Area</router-link>
      </div>
      <div v-if="error" class="error">{{ error }}</div>
      <div class="card">
        <div v-if="loading" class="loading">
          <span class="spinner"></span> Loading storage areas...
        </div>
        <div v-else>
          <template v-if="filteredAreas.length">
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Type</th>
                  <th>Location</th>
                  <th>Capacity</th>
                  <th>Utilization</th>
                  <th>Served Docks</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="area in filteredAreas" :key="area.id">
                  <td><strong>{{ area.id }}</strong></td>
                  <td>{{ area.type }}</td>
                  <td>{{ area.location }}</td>
                  <td>{{ area.currentCapacityTEU }} / {{ area.maxCapacityTEU }} TEU</td>
                  <td>
                    <div class="capacity-bar-container">
                      <div class="capacity-bar">
                        <div
                          class="capacity-fill"
                          :class="getUtilizationClass(area)"
                          :style="{ width: getUtilizationPercent(area) + '%' }"
                        ></div>
                      </div>
                      <span class="capacity-text">{{ getUtilizationPercent(area).toFixed(1) }}%</span>
                    </div>
                  </td>
                  <td>{{ area.servedDockIds?.length || 0 }} dock(s)</td>
                  <td>
                    <div class="actions-cell">
                      <router-link
                        :to="`/storage-area/view/${encodeURIComponent(area.id)}`"
                        class="btn btn-secondary btn-small"
                        >View</router-link
                      >
                      <router-link
                        :to="`/storage-area/edit/${encodeURIComponent(area.id)}`"
                        class="btn btn-primary btn-small"
                        >Edit</router-link
                      >
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </template>
          <template v-else>
            <div class="empty-state">
              <h3>No Storage Areas Found</h3>
              <p>Create your first storage area to get started.</p>
              <br />
              <router-link to="/storage-area/create" class="btn btn-primary"
                >+ New Storage Area</router-link
              >
            </div>
          </template>
        </div>
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useStorageAreaApi, type StorageArea } from '../../services/useStorageAreaApi'

const searchTerm = ref('')
const { loading, error, listStorageAreas } = useStorageAreaApi()
const storageAreas = ref<StorageArea[]>([])

onMounted(async () => {
  try {
    storageAreas.value = await listStorageAreas()
  } catch (err) {
  }
})

const filteredAreas = computed(() => {
  if (!searchTerm.value) return storageAreas.value

  const term = searchTerm.value.toLowerCase()
  return storageAreas.value.filter(
    (area) =>
      area.id.toLowerCase().includes(term) ||
      area.type.toLowerCase().includes(term) ||
      area.location.toLowerCase().includes(term)
  )
})

function getUtilizationPercent(area: StorageArea): number {
  return (area.currentCapacityTEU / area.maxCapacityTEU) * 100
}

function getUtilizationClass(area: StorageArea): string {
  const percent = getUtilizationPercent(area)
  if (percent >= 90) return 'danger'
  if (percent >= 70) return 'warning'
  return ''
}
</script>

<style scoped>
@import '../../assets/dashboard.css';

.actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.left-actions {
  display: flex;
  align-items: center;
}

.capacity-bar-container {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.capacity-bar {
  width: 100px;
  height: 20px;
  background: #e5e7eb;
  border-radius: 4px;
  overflow: hidden;
}

.capacity-fill {
  height: 100%;
  background: #2563eb;
  transition: width 0.3s;
}

.capacity-fill.warning {
  background: #f59e0b;
}

.capacity-fill.danger {
  background: #dc2626;
}

.capacity-text {
  font-size: 0.875rem;
  color: #6b7280;
}
</style>
