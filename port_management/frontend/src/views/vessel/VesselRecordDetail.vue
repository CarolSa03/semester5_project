<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useVesselRecordApi, type VesselRecord } from '../../services/useVesselRecordApi'

const route = useRoute()
const router = useRouter()
const { getVesselRecord } = useVesselRecordApi()

const vesselRecord = ref<VesselRecord | null>(null)
const isLoading = ref(true)

const imoParam = computed(() => route.params.imo as string)

const renderImo = (imo: string | { value: string }) => {
  if (typeof imo === 'object' && imo !== null && 'value' in imo) {
    return imo.value
  }
  return imo
}

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
    vesselRecord.value = await getVesselRecord(imoParam.value)
  } catch (error) {
    alert('Failed to load vessel details')
    router.push('/vessel-record/list')
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="isLoading" class="loading">Loading vessel details...</div>

      <div v-else-if="vesselRecord">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem;">
          <h2>{{ vesselRecord.name }}</h2>
          <div>
            <button class="btn btn-secondary" @click="router.push('/vessel-record/list')" style="margin-right: 0.5rem;">
              ← Back
            </button>
            <button
              class="btn btn-primary"
              @click="router.push(`/vessel-record/${encodeURIComponent(renderImo(vesselRecord.imo))}/edit`)"
            >
              Edit
            </button>
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 1.5rem;">
          <div>
            <div class="detail-label">IMO Number</div>
            <div class="detail-value badge">{{ renderImo(vesselRecord.imo) }}</div>
          </div>
          <div>
            <div class="detail-label">Status</div>
            <span class="badge" :class="vesselRecord.isActive ? 'badge-success' : 'badge-danger'">
              {{ vesselRecord.isActive ? 'Active' : 'Inactive' }}
            </span>
          </div>
          <div>
            <div class="detail-label">Vessel Name</div>
            <div class="detail-value">{{ vesselRecord.name }}</div>
          </div>
          <div>
            <div class="detail-label">Vessel Type</div>
            <div class="detail-value">
              {{ vesselRecord.vesselType ? vesselRecord.vesselType.name : 'Not specified' }}
            </div>
          </div>
          <div>
            <div class="detail-label">Owner / Operator</div>
            <div class="detail-value">{{ vesselRecord.owner }}</div>
          </div>
        </div>

        <div v-if="vesselRecord.vesselType">
          <h3 style="margin-bottom: 0.75rem; font-size: 1.125rem;">Vessel Type Details</h3>
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 1.5rem;">
            <div v-if="vesselRecord.vesselType.description" style="grid-column: 1 / -1;">
              <div class="detail-label">Description</div>
              <div class="detail-value">{{ vesselRecord.vesselType.description }}</div>
            </div>
            <div v-if="vesselRecord.vesselType.capacityTEU">
              <div class="detail-label">Capacity</div>
              <div class="detail-value">{{ vesselRecord.vesselType.capacityTEU }} TEU</div>
            </div>
            <div v-if="vesselRecord.vesselType.maxRows">
              <div class="detail-label">Max Rows</div>
              <div class="detail-value">{{ vesselRecord.vesselType.maxRows }}</div>
            </div>
            <div v-if="vesselRecord.vesselType.maxBays">
              <div class="detail-label">Max Bays</div>
              <div class="detail-value">{{ vesselRecord.vesselType.maxBays }}</div>
            </div>
            <div v-if="vesselRecord.vesselType.maxTiers">
              <div class="detail-label">Max Tiers</div>
              <div class="detail-value">{{ vesselRecord.vesselType.maxTiers }}</div>
            </div>
          </div>
        </div>

        <div>
          <h3 style="margin-bottom: 0.75rem; font-size: 1.125rem;">Metadata</h3>
          <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
            <div>
              <div class="detail-label">Created At</div>
              <div class="detail-value">{{ formatDate(vesselRecord.createdAt) }}</div>
            </div>
            <div>
              <div class="detail-label">Last Updated</div>
              <div class="detail-value" :class="{ empty: !vesselRecord.updatedAt }">
                {{ vesselRecord.updatedAt ? formatDate(vesselRecord.updatedAt) : 'Never' }}
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
