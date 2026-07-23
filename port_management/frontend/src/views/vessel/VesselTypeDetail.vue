<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useVesselTypeApi, type VesselType } from '../../services/useVesselTypeApi'

const route = useRoute()
const router = useRouter()
const { getVesselType } = useVesselTypeApi()

const vesselType = ref<VesselType | null>(null)
const isLoading = ref(true)

const vesselTypeId = computed(() => route.params.id as string)

const formatDate = (dateString?: string) => {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleString('en-GB', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(async () => {
  try {
    vesselType.value = await getVesselType(vesselTypeId.value)
  } catch (error) {
    alert('Failed to load vessel type details')
    router.push('/vessel-type/list')
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="isLoading" class="loading">Loading vessel type details...</div>

      <div v-else-if="vesselType">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem;">
          <h2>{{ vesselType.name }}</h2>
          <div>
            <button class="btn btn-secondary" @click="router.push('/vessel-type/list')" style="margin-right: 0.5rem;">
              ← Back
            </button>
            <button class="btn btn-primary" @click="router.push(`/vessel-type/${vesselType.id}/edit`)">Edit</button>
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr; gap: 1rem; margin-bottom: 1.5rem;">
          <div>
            <div class="detail-label">Vessel Type Name</div>
            <div class="detail-value badge">{{ vesselType.name }}</div>
          </div>

          <div>
            <div class="detail-label">Description</div>
            <div :class="['detail-value', !vesselType.description && 'empty']">
              {{ vesselType.description || 'No description provided' }}
            </div>
          </div>

          <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
            <div style="flex: 1;">
              <div class="detail-label">Status</div>
              <span :class="['badge', vesselType.isActive ? 'badge-success' : 'badge-danger']">
                {{ vesselType.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>
        </div>

        <div>
          <h3 style="margin-bottom: 0.75rem; font-size: 1.125rem;">Container Specifications</h3>
          <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 1rem; margin-bottom: 1.5rem;">
            <div>
              <div class="detail-label">Capacity (TEU)</div>
              <div :class="['detail-value', !vesselType.capacityTEU && 'empty']">
                {{ vesselType.capacityTEU ? `${vesselType.capacityTEU} TEU` : 'Not specified' }}
              </div>
            </div>

            <div>
              <div class="detail-label">Max Rows</div>
              <div :class="['detail-value', !vesselType.maxRows && 'empty']">
                {{ vesselType.maxRows ?? 'Not specified' }}
              </div>
            </div>

            <div>
              <div class="detail-label">Max Bays</div>
              <div :class="['detail-value', !vesselType.maxBays && 'empty']">
                {{ vesselType.maxBays ?? 'Not specified' }}
              </div>
            </div>

            <div>
              <div class="detail-label">Max Tiers</div>
              <div :class="['detail-value', !vesselType.maxTiers && 'empty']">
                {{ vesselType.maxTiers ?? 'Not specified' }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';

.detail-label {
  font-weight: 500;
  color: #6b7280;
  font-size: 0.95rem;
  margin-bottom: 0.25rem;
}
.detail-value {
  font-size: 1.05rem;
  color: #333;
  margin-bottom: 0.75rem;
}
.detail-value.empty {
  color: #9ca3af;
  font-style: italic;
}
.badge {
  background: #e0e7ff;
  color: #3730a3;
  font-weight: 600;
  border-radius: 6px;
  padding: 4px 12px;
  display: inline-block;
}
.badge-success {
  background: #d1fae5;
  color: #065f46;
}
.badge-danger {
  background: #fee2e2;
  color: #991b1b;
}
</style>
