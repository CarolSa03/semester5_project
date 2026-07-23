<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useOrganizationApi, type Organization } from '../../services/useOrganizationApi'

const route = useRoute()
const router = useRouter()
const { loading, getOrganization } = useOrganizationApi()

const organization = ref<Organization | null>(null)
const errorMessage = ref('')

const orgId = route.params.id as string

onMounted(async () => {
  try {
    organization.value = await getOrganization(orgId)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load organization'
  }
})

function formatDate(dateString?: string): string {
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

function goBack() {
  router.push('/organization/list')
}

function editOrganization() {
  router.push(`/organization/${orgId}/edit`)
}
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="loading" class="loading">Loading organization details...</div>

      <div v-else-if="errorMessage" class="error">{{ errorMessage }}</div>

      <div v-else-if="organization">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem;">
          <h2>{{ organization.legalName }}</h2>
          <div>
            <button @click="goBack" class="btn btn-secondary" style="margin-right: 0.5rem;">
              ← Back
            </button>
            <button @click="editOrganization" class="btn btn-primary">Edit</button>
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
          <div>
            <div class="detail-label">Legal Name</div>
            <div class="detail-value badge">{{ organization.legalName }}</div>
          </div>
          <div>
            <div class="detail-label">Alternative Name</div>
            <div class="detail-value" :class="{ empty: !organization.alternativeName }">
              {{ organization.alternativeName || 'Not provided' }}
            </div>
          </div>
          <div>
            <div class="detail-label">Tax Number</div>
            <div class="detail-value">{{ organization.taxNumber }}</div>
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
</style>

