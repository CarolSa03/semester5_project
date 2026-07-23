<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useStaffApi } from '../../services/useStaffApi'

const router = useRouter()
const staffApi = useStaffApi()
const { 
  fetchStaff, 
  fetchStaffAvailable, 
  fetchStaffUnavailable,
  searchStaffByName,
  searchStaffByQualification,
  deactivateStaff,
  activateStaff,
  deleteStaff
} = staffApi

const allStaff = ref<any[]>([])
const searchTerm = ref('')
const searchType = ref<'all' | 'name' | 'qualification'>('all')
const filterType = ref<'all' | 'available' | 'unavailable'>('all')
const loading = ref(true)
const error = ref('')

const loadStaff = async () => {
  loading.value = true
  error.value = ''
  try {
    if (filterType.value === 'available') {
      allStaff.value = await fetchStaffAvailable()
    } else if (filterType.value === 'unavailable') {
      allStaff.value = await fetchStaffUnavailable()
    } else {
      allStaff.value = await fetchStaff()
    }
  } catch (err) {
    error.value = 'Failed to load staff members'
  } finally {
    loading.value = false
  }
}

onMounted(() => loadStaff())

const handleSearch = async () => {
  if (!searchTerm.value.trim()) {
    await loadStaff()
    return
  }

  loading.value = true
  error.value = ''
  try {
    if (searchType.value === 'name') {
      allStaff.value = await searchStaffByName(searchTerm.value)
    } else if (searchType.value === 'qualification') {
      allStaff.value = await searchStaffByQualification(searchTerm.value)
    } else {
      await loadStaff()
    }
  } catch (err) {
    error.value = 'Search failed'
  } finally {
    loading.value = false
  }
}

const handleFilterChange = async () => {
  await loadStaff()
}

const filteredStaff = computed(() => {
  if (searchType.value === 'all' && searchTerm.value) {
    const term = searchTerm.value.toLowerCase()
    return allStaff.value.filter(member =>
      member.shortName.toLowerCase().includes(term) ||
      member.email?.toLowerCase().includes(term) ||
      member.phoneNumber?.toLowerCase().includes(term) ||
      member.staffMemberId?.toLowerCase().includes(term)
    )
  }
  return allStaff.value
})

const handleDelete = async (id: string, name: string) => {
  if (!confirm(`Are you sure you want to delete ${name}? This action cannot be undone.`)) {
    return
  }

  try {
    await deleteStaff(id)
    await loadStaff()
  } catch (err) {
    alert('Failed to delete staff member')
  }
}

const getQualificationsList = (qualifications: any[]) => {
  if (!qualifications || qualifications.length === 0) return 'None'
  return qualifications.map(q => q.descriptiveName).join(', ')
}

const handleToggleAvailability = async (member: any) => {
  try {
    if (member.isAvailable) {
      await deactivateStaff(member.id)
    } else {
      await activateStaff(member.id)
    }
    await loadStaff()
  } catch (err) {
    alert('Failed to update staff availability')
  }
}
</script>

<template>
  <div class="dashboard-content">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
      <h2>Staff Members</h2>
      <button @click="router.push('/staff/create')" class="btn btn-primary">
        + New Staff Member
      </button>
    </div>

    <div v-if="error" class="error">{{ error }}</div>

    <div class="card">
      <div style="display: flex; gap: 1rem; margin-bottom: 1.5rem; flex-wrap: wrap;">
        <select v-model="filterType" @change="handleFilterChange" style="padding: 0.5rem; border-radius: 6px;">
          <option value="all">All Staff</option>
          <option value="available">Available Only</option>
          <option value="unavailable">Unavailable Only</option>
        </select>

        <select v-model="searchType" style="padding: 0.5rem; border-radius: 6px;">
          <option value="all">Local Search</option>
          <option value="name">Search by Name (API)</option>
          <option value="qualification">Search by Qualification (API)</option>
        </select>

        <input
          v-model="searchTerm"
          type="text"
          :placeholder="searchType === 'qualification' ? 'Search by qualification...' : 'Search by name...'"
          @keyup.enter="handleSearch"
          style="flex: 1; padding: 0.5rem; border: 1px solid #ddd; border-radius: 6px;"
        />
        <button @click="handleSearch" class="btn btn-secondary">Search</button>
        <button v-if="searchTerm" @click="searchTerm = ''; loadStaff()" class="btn btn-secondary">Clear</button>
      </div>

      <div v-if="loading" class="loading">Loading staff members...</div>
      
      <div v-else-if="filteredStaff.length === 0" class="empty-state">
        <h3>No staff members found</h3>
        <p>Start by creating your first staff member.</p>
        <button @click="router.push('/staff/create')" class="btn btn-primary">+ Create Staff Member</button>
      </div>

      <table v-else>
        <thead>
          <tr>
            <th>Staff ID</th>
            <th>Name</th>
            <th>Email</th>
            <th>Qualifications</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="member in filteredStaff" :key="member.id">
            <td>
              <span class="badge" style="background: #e0e7ff; color: #3730a3; font-weight: 600;">
                {{ member.staffMemberId }}
              </span>
            </td>
            <td>{{ member.shortName }}</td>
            <td>{{ member.email }}</td>
            <td style="font-size: 0.875rem; color: #6b7280;">
              {{ getQualificationsList(member.qualifications) }}
            </td>
            <td>
              <span class="badge" :class="member.isAvailable ? 'badge-success' : 'badge-danger'">
                {{ member.isAvailable ? 'Available' : 'Unavailable' }}
              </span>
            </td>
            <td>
              <div class="actions-cell">
                <button @click="router.push(`/staff/${member.id}`)" class="btn btn-secondary btn-small">
                  View
                </button>
                <button @click="router.push(`/staff/${member.id}/edit`)" class="btn btn-primary btn-small">
                  Edit
                </button>
                <button @click="handleDelete(member.id, member.shortName)" class="btn btn-danger btn-small">
                  Delete
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
.btn-danger {
  background-color: #ef4444;
  color: white;
}
.btn-danger:hover {
  background-color: #dc2626;
}
.actions-cell {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}
</style>
