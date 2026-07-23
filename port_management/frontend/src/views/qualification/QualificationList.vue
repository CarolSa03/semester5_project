<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useQualificationApi, type Qualification } from '../../services/useQualificationApi'

const router = useRouter()
const { loading, listQualifications, deleteQualification } = useQualificationApi()

const qualifications = ref<Qualification[]>([])
const searchTerm = ref('')
const errorMessage = ref('')

onMounted(async () => {
  try {
    qualifications.value = await listQualifications()
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load qualifications'
  }
})

const filteredQualifications = computed(() => {
  if (!searchTerm.value) return qualifications.value
  
  const search = searchTerm.value.toLowerCase()
  return qualifications.value.filter(qual =>
    qual.code?.toString().includes(search) ||
    qual.description.toLowerCase().includes(search)
  )
})

function viewQualification(code?: string) {
  if (code) router.push(`/qualification/${code}`)
}

async function handleDelete(code?: string) {
  if (!code) return;
  if (!confirm('Are you sure you want to delete this qualification? This action cannot be undone.')) {
    return;
  }
  try {
    await deleteQualification(code);
    qualifications.value = qualifications.value.filter(q => q.code !== code);
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to delete qualification';
  }
}


function createQualification() {
  router.push('/qualification/create')
}
</script>

<template>
    
    <div class="dashboard-content">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
        <h2>Qualifications</h2>
        <button @click="createQualification" class="btn btn-primary">+ New Qualification</button>
      </div>

      <div v-if="errorMessage" class="error">
        {{ errorMessage }}
      </div>

      <div class="card">
        <div style="margin-bottom: 1.5rem;">
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Search by code or name..."
            style="max-width: 400px;"
          />
        </div>

        <div v-if="loading" class="loading">Loading qualifications...</div>

        <div v-else-if="filteredQualifications.length === 0" class="empty-state">
          <h3>No qualifications found</h3>
          <p>Start by creating your first qualification</p>
          <br>
          <button @click="createQualification" class="btn btn-primary">+ Create Qualification</button>
        </div>

        <table v-else>
          <thead>
            <tr>
              <th>Code</th>
              <th>Descriptive Name</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="qual in filteredQualifications" :key="qual.code">
              <td>
                <span class="badge" style="background: #e0e7ff; color: #3730a3; font-weight: 600;">
                  {{ qual.code }}
                </span>
              </td>
              <td>{{ qual.description }}</td>
              <td>
                <div class="actions-cell">
                  <button @click="handleDelete(qual.code)" class="btn btn-danger btn-small">Delete</button>
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
