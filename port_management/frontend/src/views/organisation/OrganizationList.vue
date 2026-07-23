<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useOrganizationApi, type Organization } from '../../services/useOrganizationApi'

const router = useRouter()
const { loading, listOrganizations } = useOrganizationApi()

const organizations = ref<Organization[]>([])
const errorMessage = ref('')
const searchTerm = ref('')

onMounted(async () => {
  try {
    organizations.value = await listOrganizations()
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load organizations'
  }
})

function formatDate(dateString?: string): string {
  if (!dateString) return '-'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-GB', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function viewOrganization(id?: string) {
  if (id) router.push(`/organization/${id}`)
}

function editOrganization(id?: string) {
  if (id) router.push(`/organization/${id}/edit`)
}

function createOrganization() {
  router.push('/organization/create')
}

const filteredOrganizations = computed(() => {
  const search = searchTerm.value.trim().toLowerCase()
  if (!search) return organizations.value

  const keywords = search.split(' ').filter(Boolean)

  return organizations.value.filter(org => {
    const haystack = [
      org.legalName,
      org.alternativeName,
      org.taxNumber,
      org.createdAt ? formatDate(org.createdAt) : ''
    ]
    .map(str => (str ?? '').toString().toLowerCase())
    .join(' ')
    return keywords.every(kw => haystack.includes(kw))
  })
})


function clearSearch() {
  searchTerm.value = ''
}
</script>

<template>
  <div class="dashboard-content">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
      <h2>Organizations</h2>
      <button @click="createOrganization" class="btn btn-primary">+ New Organization</button>
    </div>

    <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

    <div class="card">
      <div style="margin-bottom: 1.5rem;">
        <input
          v-model="searchTerm"
          type="text"
          placeholder="Search by legal name, alternative name, or tax number..."
          style="max-width: 400px;"
        />
      </div>

      <div v-if="loading" class="loading">Loading organizations...</div>

      <div v-else-if="filteredOrganizations.length === 0 && searchTerm" class="empty-state">
        <h3>No organizations found</h3>
        <p>No organizations match your search "{{ searchTerm }}".</p>
        <button @click="searchTerm = ''" class="btn btn-secondary">Clear Search</button>
      </div>

      <div v-else-if="organizations.length === 0" class="empty-state">
        <h3>No organizations found</h3>
        <p>Start by creating your first organization.</p>
        <button @click="createOrganization" class="btn btn-primary">+ Create Organization</button>
      </div>

      <table v-else>
        <thead>
          <tr>
            <th>Legal Name</th>
            <th>Alternative Name</th>
            <th>Tax Number</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="org in filteredOrganizations" :key="org.id">
            <td>
              <span class="badge" style="background: #e0e7ff; color: #3730a3; font-weight: 600;">
                {{ org.legalName }}
              </span>
            </td>
            <td>{{ org.alternativeName || '-' }}</td>
            <td>{{ org.taxNumber }}</td>
            <td>
              <div class="actions-cell">
                <button @click="viewOrganization(org.id)" class="btn btn-secondary btn-small">View</button>
                <button @click="editOrganization(org.id)" class="btn btn-primary btn-small">Edit</button>
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
</style>


