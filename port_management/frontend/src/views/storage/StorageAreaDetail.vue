<template>
    <div class="dashboard-content">
      <div v-if="loading" class="loading">
        <span class="spinner"></span> Loading storage area details...
      </div>
      <div v-else-if="error" class="error">{{ error }}</div>
      <div v-else-if="storageArea" class="card">
        <div class="card-header">
          <h2>{{ storageArea.id }}</h2>
          <div class="actions">
            <router-link
              :to="`/storage-area/edit/${encodeURIComponent(storageArea.id)}`"
              class="btn btn-primary"
              >Edit</router-link
            >
            <router-link to="/storage-area/list" class="btn btn-secondary">Back to List</router-link>
          </div>
        </div>

        <div class="detail-grid">
          <div class="detail-item">
            <div class="detail-label">Storage Area ID</div>
            <div class="detail-value">{{ storageArea.id }}</div>
          </div>

          <div class="detail-item">
            <div class="detail-label">Type</div>
            <div class="detail-value">{{ storageArea.type }}</div>
          </div>

          <div class="detail-item">
            <div class="detail-label">Location</div>
            <div class="detail-value">{{ storageArea.location }}</div>
          </div>

          <div class="detail-item">
            <div class="detail-label">Capacity Overview</div>
            <div class="capacity-section">
              <div class="capacity-card">
                <div class="capacity-label">Current Capacity</div>
                <div class="capacity-value">{{ storageArea.currentCapacityTEU }} TEU</div>
              </div>
              <div class="capacity-card">
                <div class="capacity-label">Maximum Capacity</div>
                <div class="capacity-value">{{ storageArea.maxCapacityTEU }} TEU</div>
              </div>
            </div>
            <div class="capacity-bar">
              <div
                class="capacity-fill"
                :class="utilizationClass"
                :style="{ width: utilizationPercent + '%' }"
              >
                {{ utilizationPercent.toFixed(1) }}% utilized
              </div>
            </div>
          </div>

          <div class="detail-item">
            <div class="detail-label">Served Docks</div>
            <!-- updated to use servedDockIds array instead of servedDocks objects -->
            <div v-if="storageArea.servedDockIds && storageArea.servedDockIds.length > 0" class="docks-grid">
              <div v-for="dockId in storageArea.servedDockIds" :key="dockId" class="dock-card">
                <div class="dock-name">Dock ID: {{ dockId }}</div>
              </div>
            </div>
            <div v-else class="empty-docks">No docks assigned</div>
          </div>

          <div v-if="storageArea.notes" class="detail-item">
            <div class="detail-label">Notes</div>
            <div class="notes-box">{{ storageArea.notes }}</div>
          </div>
        </div>
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useStorageAreaApi, type StorageArea } from '../../services/useStorageAreaApi'

const route = useRoute()
const { loading, error, getStorageArea } = useStorageAreaApi()
const storageArea = ref<StorageArea | null>(null)

const utilizationPercent = computed(() => {
  if (!storageArea.value) return 0
  return (storageArea.value.currentCapacityTEU / storageArea.value.maxCapacityTEU) * 100
})

const utilizationClass = computed(() => {
  const percent = utilizationPercent.value
  if (percent >= 90) return 'danger'
  if (percent >= 70) return 'warning'
  return ''
})

onMounted(async () => {
  const id = route.params.id as string
  if (id) {
    try {
      storageArea.value = await getStorageArea(id)
    } catch (err) {
    }
  }
})
</script>

<style scoped>
@import '../../assets/dashboard.css';

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e5e7eb;
}

.detail-grid {
  display: grid;
  gap: 1.5rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
}

.detail-label {
  font-weight: 600;
  color: #6b7280;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 0.5rem;
}

.detail-value {
  font-size: 1rem;
  color: #1f2937;
}

.capacity-section {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
  margin-top: 1rem;
}

.capacity-card {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 1rem;
}

.capacity-label {
  font-size: 0.875rem;
  color: #6b7280;
  margin-bottom: 0.5rem;
}

.capacity-value {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
}

.capacity-bar {
  height: 24px;
  background: #e5e7eb;
  border-radius: 12px;
  overflow: hidden;
  margin-top: 1rem;
}

.capacity-fill {
  height: 100%;
  background: #2563eb;
  transition: width 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
}

.capacity-fill.warning {
  background: #f59e0b;
}

.capacity-fill.danger {
  background: #dc2626;
}

.docks-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1rem;
  margin-top: 0.5rem;
}

.dock-card {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  padding: 1rem;
}

.dock-name {
  font-weight: 600;
  color: #2563eb;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.empty-docks {
  color: #9ca3af;
  font-style: italic;
}

.notes-box {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  padding: 1rem;
  color: #374151;
  white-space: pre-wrap;
}
</style>
