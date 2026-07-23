<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStaffApi } from '../../services/useStaffApi'

const route = useRoute()
const router = useRouter()
const { fetchStaffByGuid, deactivateStaff, activateStaff } = useStaffApi()

const staff = ref<any>(null)
const loading = ref(true)

onMounted(async () => {
  try {
    const id = route.params.id as string
    staff.value = await fetchStaffByGuid(id)
    loading.value = false
  } catch (err) {
    alert('Failed to load staff member details')
    router.push('/staff/list')
  }
})


const handleToggleAvailability = async () => {
  try {
    if (staff.value.isAvailable) {
      await deactivateStaff(staff.value.id)
      staff.value.isAvailable = false
    } else {
      await activateStaff(staff.value.id)
      staff.value.isAvailable = true
    }
  } catch (err) {
    alert('Failed to update staff availability')
  }
}
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="loading" class="loading">Loading staff member details...</div>

      <div v-else-if="staff">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.25rem;">
          <h2>{{ staff.shortName }}</h2>
          <div style="display: flex; gap: 0.5rem;">
            <button 
              @click="handleToggleAvailability" 
              class="btn"
              :class="staff.isAvailable ? 'btn-warning' : 'btn-success'"
            >
              {{ staff.isAvailable ? 'Mark Unavailable' : 'Mark Available' }}
            </button>
            <button @click="router.push(`/staff/${staff.id}/edit`)" class="btn btn-primary">
              Edit
            </button>
            <button @click="router.push('/staff/list')" class="btn btn-secondary">
              ← Back
            </button>
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 1.5rem;">
          <div>
            <div class="detail-label">Staff Member ID</div>
            <div class="detail-value badge">{{ staff.staffMemberId }}</div>
          </div>
          <div>
            <div class="detail-label">Short Name</div>
            <div class="detail-value">{{ staff.shortName }}</div>
          </div>
          <div>
            <div class="detail-label">Email</div>
            <div class="detail-value">{{ staff.email }}</div>
          </div>
          <div>
            <div class="detail-label">Phone Number</div>
            <div class="detail-value">{{ staff.phoneNumber }}</div>
          </div>
          <div>
            <div class="detail-label">Availability Status</div>
            <div class="detail-value">
              <span class="badge" :class="staff.isAvailable ? 'badge-success' : 'badge-danger'">
                {{ staff.isAvailable ? 'Available' : 'Unavailable' }}
              </span>
            </div>
          </div>
        </div>

        <div>
          <div class="detail-label">Qualifications</div>
          <div v-if="staff.qualifications && staff.qualifications.length > 0" class="qualifications-grid">
            <div v-for="qual in staff.qualifications" :key="qual.code" class="qualification-card">
              <div class="qualification-code">{{ qual.code }}</div>
              <div class="qualification-name">{{ qual.descriptiveName }}</div>
            </div>
          </div>
          <div v-else class="empty-qualifications">No qualifications assigned</div>
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
.badge-success {
  background: #d1fae5;
  color: #065f46;
}
.badge-danger {
  background: #fee2e2;
  color: #991b1b;
}
.qualifications-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 0.75rem;
  margin-top: 0.5rem;
}
.qualification-card {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  padding: 0.75rem;
}
.qualification-code {
  font-weight: 600;
  color: #2563eb;
  font-size: 0.875rem;
}
.qualification-name {
  color: #6b7280;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}
.empty-qualifications {
  color: #9ca3af;
  font-style: italic;
  margin-top: 0.5rem;
}
.btn-warning {
  background-color: #f59e0b;
  color: white;
}
.btn-warning:hover {
  background-color: #d97706;
}
.btn-success {
  background-color: #10b981;
  color: white;
}
.btn-success:hover {
  background-color: #059669;
}
</style>

