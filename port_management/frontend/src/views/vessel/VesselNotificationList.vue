<template>
  <div class="dashboard-content">
    <div class="page-header">
      <div>
        <h1>Vessel Visit Notifications</h1>
        <p class="subtitle">View and manage all vessel visit notifications</p>
      </div>
      <router-link to="/notification/create" class="btn btn-primary">
        <FilePlus :size="18" />
        <span>Create New</span>
      </router-link>
    </div>

    <!-- Filters -->
    <div class="card" style="margin-bottom: 1.5rem;">
      <div class="filters">
        <div class="form-group">
          <label for="statusFilter">Status</label>
          <select id="statusFilter" v-model="filters.status" @change="applyFilters">
            <option value="">All Statuses</option>
            <option value="InProgress">In Progress</option>
            <option value="PendingApproval">Pending Approval</option>
            <option value="Approved">Approved</option>
            <option value="Rejected">Rejected</option>
            <option value="Withdrawn">Withdrawn</option>
          </select>
        </div>

        <div class="form-group">
          <label for="searchBusinessId">Business ID</label>
          <input 
            id="searchBusinessId" 
            v-model="filters.businessId" 
            type="text" 
            placeholder="Search by Business ID"
            @input="applyFilters"
          />
        </div>

        <button @click="clearFilters" class="btn btn-secondary" style="margin-top: 1.5rem;">
          Clear Filters
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <Loader2 :size="32" class="spin" />
      <p>Loading notifications...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-message">
      <AlertCircle :size="20" />
      <span>{{ error }}</span>
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredNotifications.length === 0" class="empty-state">
      <Ship :size="48" />
      <h3>No notifications found</h3>
      <p>There are no vessel visit notifications matching your criteria.</p>
      <router-link to="/notification/create" class="btn btn-primary">
        <FilePlus :size="18" />
        <span>Create First Notification</span>
      </router-link>
    </div>

    <!-- Notifications Table -->
    <div v-else class="card">
      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>Business ID</th>
              <th>Vessel ID</th>
              <th>Status</th>
              <th>ETA</th>
              <th>ETD</th>
              <th>Agent Rep ID</th>
              <th>Created</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="notification in paginatedNotifications" :key="notification.businessId">
              <td>
                <strong>{{ notification.businessId }}</strong>
              </td>
              <td>
                <code style="font-size: 0.8rem;">{{ formatUuid(notification.vesselId) }}</code>
              </td>
              <td>
                <span :class="['status-badge', getStatusClass(notification.status)]">
                  {{ notification.status }}
                </span>
              </td>
              <td>{{ formatDate(notification.eta) }}</td>
              <td>{{ formatDate(notification.etd) }}</td>
              <td>{{ notification.shippingAgentRepresentativeId }}</td>
              <td>{{ formatDate(notification.createdAt) }}</td>
              <td>
                <div class="action-buttons">
                  <button 
                    @click="viewDetails(notification.businessId)" 
                    class="btn-icon" 
                    title="View Details"
                  >
                    <Eye :size="16" />
                  </button>
                  <button 
                    v-if="canEdit(notification.status)"
                    @click="editNotification(notification.businessId)" 
                    class="btn-icon" 
                    title="Edit"
                  >
                    <Edit :size="16" />
                  </button>
                  <button 
                    v-if="canDelete(notification.status)"
                    @click="deleteNotification(notification.businessId)" 
                    class="btn-icon btn-danger" 
                    title="Delete"
                  >
                    <Trash2 :size="16" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="pagination" v-if="totalPages > 1">
        <button 
          @click="currentPage--" 
          :disabled="currentPage === 1"
          class="btn btn-secondary"
        >
          Previous
        </button>
        <span class="page-info">Page {{ currentPage }} of {{ totalPages }}</span>
        <button 
          @click="currentPage++" 
          :disabled="currentPage === totalPages"
          class="btn btn-secondary"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { 
  FilePlus, Ship, Eye, Edit, Trash2, AlertCircle, Loader2 
} from 'lucide-vue-next'
import { useVesselVisitNotificationApi, type VesselVisitNotification } from '@/services/useVesselVisitNotificationApi'

const router = useRouter()
const { listNotifications, deleteNotification: apiDelete, loading, error } = useVesselVisitNotificationApi()

const notifications = ref<VesselVisitNotification[]>([])
const filters = ref({
  status: '',
  businessId: '',
})

const currentPage = ref(1)
const itemsPerPage = 10

// Load notifications
onMounted(async () => {
  await loadNotifications()
})

async function loadNotifications() {
  try {
    notifications.value = await listNotifications()
  } catch (err) {
    console.error('Failed to load notifications:', err)
  }
}

// Filtering
const filteredNotifications = computed(() => {
  let result = notifications.value

  if (filters.value.status) {
    result = result.filter(n => n.status === filters.value.status)
  }

  if (filters.value.businessId) {
    result = result.filter(n => 
      n.businessId.toLowerCase().includes(filters.value.businessId.toLowerCase())
    )
  }

  return result
})

// Pagination
const totalPages = computed(() => 
  Math.ceil(filteredNotifications.value.length / itemsPerPage)
)

const paginatedNotifications = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredNotifications.value.slice(start, end)
})

function applyFilters() {
  currentPage.value = 1
}

function clearFilters() {
  filters.value = {
    status: '',
    businessId: '',
  }
  currentPage.value = 1
}

// Actions
function viewDetails(businessId: string) {
  router.push(`/notification/${businessId}`)
}

function editNotification(businessId: string) {
  router.push(`/notification/${businessId}/edit`)
}

async function deleteNotification(businessId: string) {
  if (!confirm(`Are you sure you want to delete notification ${businessId}?`)) {
    return
  }

  try {
    await apiDelete(businessId)
    await loadNotifications()
  } catch (err: any) {
    alert(`Failed to delete: ${err.message}`)
  }
}

// Permissions
function canEdit(status: string) {
  return status === 'InProgress' || status === 'Rejected'
}

function canDelete(status: string) {
  return status === 'InProgress'
}

// Formatters
function formatDate(date?: string) {
  if (!date) return 'N/A'
  return new Date(date).toLocaleString('en-GB', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatUuid(uuid: string) {
  return uuid.substring(0, 8) + '...'
}

function getStatusClass(status: string) {
  const classes: Record<string, string> = {
    'InProgress': 'status-progress',
    'PendingApproval': 'status-pending',
    'Approved': 'status-approved',
    'Rejected': 'status-rejected',
    'Withdrawn': 'status-withdrawn',
  }
  return classes[status] || 'status-default'
}
</script>

<style scoped>
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 1.875rem;
  font-weight: 700;
  color: #1e3a5f;
  margin: 0;
}

.subtitle {
  color: #6b7280;
  margin-top: 0.25rem;
}

.filters {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  align-items: flex-end;
}

.form-group {
  flex: 1;
  min-width: 200px;
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
}

.form-group input,
.form-group select {
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.loading-state,
.empty-state {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.empty-state h3 {
  margin: 1rem 0 0.5rem;
  color: #374151;
}

.empty-state .btn {
  margin-top: 1.5rem;
}

.table-container {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th,
.data-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.data-table th {
  background: #f9fafb;
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.data-table tr:hover {
  background: #f9fafb;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-progress {
  background: #dbeafe;
  color: #1e40af;
}

.status-pending {
  background: #fef3c7;
  color: #92400e;
}

.status-approved {
  background: #d1fae5;
  color: #065f46;
}

.status-rejected {
  background: #fee2e2;
  color: #991b1b;
}

.status-withdrawn {
  background: #f3f4f6;
  color: #4b5563;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.btn-icon {
  padding: 0.375rem;
  background: transparent;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s;
}

.btn-icon:hover {
  background: #f3f4f6;
  color: #374151;
}

.btn-icon.btn-danger:hover {
  background: #fee2e2;
  color: #991b1b;
  border-color: #fecaca;
}

.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.page-info {
  font-size: 0.875rem;
  color: #6b7280;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-weight: 500;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:hover {
  background-color: #2563eb;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #4b5563;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.error-message {
  padding: 1rem;
  background-color: #fee2e2;
  color: #991b1b;
  border-radius: 0.375rem;
  border: 1px solid #ef4444;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
</style>
