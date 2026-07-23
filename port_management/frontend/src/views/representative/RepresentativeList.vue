<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useRepresentativeApi } from '../../services/useRepresentativeApi'
import { useOrganizationApi } from '../../services/useOrganizationApi'

const router = useRouter()
const { fetchRepresentatives } = useRepresentativeApi()
const { listOrganizations } = useOrganizationApi()
const representatives = ref<any[]>([])
const organizations = ref<any[]>([])
const loading = ref(true)
const error = ref('')
const searchTerm = ref('')

onMounted(async () => {
  try {
    const [repsData, orgsData] = await Promise.all([
      fetchRepresentatives(),
      listOrganizations()
    ])
    representatives.value = repsData
    organizations.value = orgsData
    loading.value = false
  } catch (err) {
    error.value = 'Failed to load representatives'
    loading.value = false
  }
})

function viewRepresentative(id: string) {
  router.push(`/representative/${id}`)
}
function editRepresentative(id: string) {
  router.push(`/representative/${id}/edit`)
}
function createRepresentative() {
  router.push('/representative/create')
}

const filteredRepresentatives = computed(() => {
  const search = searchTerm.value.trim().toLowerCase()
  if (!search) return representatives.value
  const keywords = search.split(' ').filter(Boolean)
  return representatives.value.filter(rep => {
    const orgName = organizations.value.find(o => o.id === rep.shippingAgentOrganizationId)?.legalName ?? ''
    const haystack = [
      rep.name, rep.email, orgName
    ].map(val => (val ?? '').toString().toLowerCase()).join(' ')
    return keywords.every(kw => haystack.includes(kw))
  })
})
</script>

<template>
  <div class="dashboard-content">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
      <h2>Representatives</h2>
      <button @click="createRepresentative" class="btn btn-primary">+ New Representative</button>
    </div>
    <div style="margin-bottom: 1.5rem;">
      <input
        v-model="searchTerm"
        type="text"
        placeholder="Search by name, email, or organization..."
        style="max-width: 400px;"
      />
    </div>
    <div v-if="error" class="error">{{ error }}</div>
    <div class="card">
      <div v-if="loading" class="loading">Loading representatives...</div>

      <div v-else-if="filteredRepresentatives.length === 0" class="empty-state">
        <h3>No representatives found</h3>
        <p>Start by creating your first representative.</p>
        <button @click="createRepresentative" class="btn btn-primary">+ Create Representative</button>
      </div>

      <table v-else>
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Organization</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="rep in filteredRepresentatives" :key="rep.id">
            <td>
              <span class="badge" style="background: #e0e7ff; color: #3730a3; font-weight: 600;">
                {{ rep.name }}
              </span>
            </td>
            <td>{{ rep.email }}</td>
            <td>{{ organizations.find(o => o.id === rep.shippingAgentOrganizationId)?.legalName || 'N/A' }}</td>
            <td>
              <div class="actions-cell">
                <button @click="viewRepresentative(rep.id)" class="btn btn-secondary btn-small">View</button>
                <button @click="editRepresentative(rep.id)" class="btn btn-primary btn-small">Edit</button>
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

.badge {
  background: #e0e7ff;
  color: #3730a3;
  font-weight: 600;
  border-radius: 6px;
  padding: 4px 12px;
}
</style>
