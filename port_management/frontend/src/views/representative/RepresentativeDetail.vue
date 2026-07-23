<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useRepresentativeApi } from '../../services/useRepresentativeApi'
import { useOrganizationApi } from '../../services/useOrganizationApi'

const route = useRoute()
const router = useRouter()
const { fetchRepresentativeById } = useRepresentativeApi()
const { listOrganizations } = useOrganizationApi()

const representative = ref<any>(null)
const organizationName = ref('')
const loading = ref(true)

onMounted(async () => {
  try {
    const id = route.params.id as string
    const [repData, orgsData] = await Promise.all([
      fetchRepresentativeById(id),
      listOrganizations()
    ])
    
    representative.value = repData
    
    const org = orgsData.find(o => o.id === repData.shippingAgentOrganizationId)
    organizationName.value = org ? org.legalName : 'Unknown'
    
    loading.value = false
  } catch (err) {
    alert('Failed to load representative details')
    router.push('/representative/list')
  }
})

const formatDate = (dateString: string) => {
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
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="loading" class="loading">Loading representative details...</div>

      <div v-else-if="representative">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem;">
          <h2>{{ representative.name }}</h2>
          <div>
            <button @click="router.push('/representative/list')" class="btn btn-secondary" style="margin-right: 0.5rem;">
              ← Back
            </button>
            <button @click="router.push(`/representative/${representative.id}/edit`)" class="btn btn-primary">
              Edit
            </button>
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
          <div>
            <div class="detail-label">Full Name</div>
            <div class="detail-value badge">{{ representative.name }}</div>
          </div>
          <div>
            <div class="detail-label">Email Address</div>
            <div class="detail-value">{{ representative.email }}</div>
          </div>
          <div>
            <div class="detail-label">Phone Number</div>
            <div class="detail-value">{{ representative.phone }}</div>
          </div>
          <div>
            <div class="detail-label">Organization</div>
            <div class="detail-value">{{ organizationName }}</div>
          </div>
          <div>
            <div class="detail-label">Representative ID</div>
            <div class="detail-value">{{ representative.id }}</div>
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
.badge {
  background: #e0e7ff;
  color: #3730a3;
  font-weight: 600;
  border-radius: 6px;
  padding: 4px 12px;
  display: inline-block;
}
</style>

