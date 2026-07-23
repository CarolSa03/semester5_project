<template>
  <div class="dashboard-content">
    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <Loader2 :size="32" class="spin" />
      <p>Loading notification...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-message">
      <AlertCircle :size="20" />
      <span>{{ error }}</span>
      <router-link to="/notification/list" class="btn btn-secondary" style="margin-top: 1rem;">
        Back to List
      </router-link>
    </div>

    <!-- Notification Details -->
    <div v-else-if="notification">
      <!-- Header -->
      <div class="page-header">
        <div>
          <div class="breadcrumb">
            <router-link to="/notification/list">Notifications</router-link>
            <ChevronRight :size="16" />
            <span>{{ notification.businessId }}</span>
          </div>
          <h1>Vessel Visit Notification</h1>
          <span :class="['status-badge-large', getStatusClass(notification.status)]">
            {{ notification.status }}
          </span>
        </div>
        <div class="header-actions">
          <button 
            v-if="canEdit"
            @click="editNotification" 
            class="btn btn-secondary"
          >
            <Edit :size="18" />
            <span>Edit</span>
          </button>
          <button 
            v-if="canDelete"
            @click="deleteNotification" 
            class="btn btn-danger"
          >
            <Trash2 :size="18" />
            <span>Delete</span>
          </button>
          <button 
            v-if="canSubmit"
            @click="submitNotification" 
            class="btn btn-primary"
          >
            <Send :size="18" />
            <span>Submit for Approval</span>
          </button>
          <button 
            v-if="canWithdraw"
            @click="withdrawNotification" 
            class="btn btn-warning"
          >
            <XCircle :size="18" />
            <span>Withdraw</span>
          </button>
        </div>
      </div>

      <!-- Basic Information -->
      <div class="card">
        <h2 class="section-title">
          <FileText :size="20" />
          Basic Information
        </h2>
        <div class="detail-grid">
          <div class="detail-item">
            <label>Business ID</label>
            <div class="detail-value">
              <strong>{{ notification.businessId }}</strong>
            </div>
          </div>

          <div class="detail-item">
            <label>Status</label>
            <div class="detail-value">
              <span :class="['status-badge', getStatusClass(notification.status)]">
                {{ notification.status }}
              </span>
            </div>
          </div>

          <div class="detail-item">
            <label>Vessel ID</label>
            <div class="detail-value code-value">
              {{ notification.vesselId }}
            </div>
          </div>

          <div class="detail-item">
            <label>Shipping Agent Representative ID</label>
            <div class="detail-value">
              {{ notification.shippingAgentRepresentativeId }}
            </div>
          </div>
        </div>
      </div>

      <!-- Schedule Information -->
      <div class="card">
        <h2 class="section-title">
          <Calendar :size="20" />
          Schedule
        </h2>
        <div class="detail-grid">
          <div class="detail-item">
            <label>ETA (Estimated Time of Arrival)</label>
            <div class="detail-value">
              <Clock :size="16" style="color: #3b82f6;" />
              {{ formatDateTime(notification.eta) }}
            </div>
          </div>

          <div class="detail-item">
            <label>ETD (Estimated Time of Departure)</label>
            <div class="detail-value">
              <Clock :size="16" style="color: #ef4444;" />
              {{ formatDateTime(notification.etd) }}
            </div>
          </div>

          <div class="detail-item">
            <label>Duration</label>
            <div class="detail-value">
              {{ calculateDuration(notification.eta, notification.etd) }}
            </div>
          </div>
        </div>
      </div>

      <!-- Optional Information -->
      <div class="card" v-if="hasOptionalInfo">
        <h2 class="section-title">
          <Package :size="20" />
          Additional Information
        </h2>
        <div class="detail-grid">
          <div class="detail-item" v-if="notification.crewId">
            <label>Crew ID</label>
            <div class="detail-value code-value">
              {{ notification.crewId }}
            </div>
          </div>

          <div class="detail-item" v-if="notification.cargoManifestsId?.length">
            <label>Cargo Manifests</label>
            <div class="detail-value">
              <div v-for="(id, index) in notification.cargoManifestsId" :key="index" class="code-chip">
                {{ formatUuid(id) }}
              </div>
            </div>
          </div>

          <div class="detail-item" v-if="notification.assignedDockId">
            <label>Assigned Dock ID</label>
            <div class="detail-value code-value">
              {{ notification.assignedDockId }}
            </div>
          </div>
        </div>
      </div>

      <!-- Approval Information -->
      <div class="card" v-if="hasApprovalInfo">
        <h2 class="section-title">
          <CheckCircle :size="20" />
          Approval Information
        </h2>
        <div class="detail-grid">
          <div class="detail-item" v-if="notification.approvedByOfficerId">
            <label>Approved By Officer ID</label>
            <div class="detail-value">
              {{ notification.approvedByOfficerId }}
            </div>
          </div>

          <div class="detail-item" v-if="notification.approvedAt">
            <label>Approved At</label>
            <div class="detail-value">
              {{ formatDateTime(notification.approvedAt) }}
            </div>
          </div>

          <div class="detail-item" v-if="notification.approvalNotes">
            <label>Approval Notes</label>
            <div class="detail-value">
              {{ notification.approvalNotes }}
            </div>
          </div>

          <div class="detail-item" v-if="notification.rejectedByOfficerId">
            <label>Rejected By Officer ID</label>
            <div class="detail-value">
              {{ notification.rejectedByOfficerId }}
            </div>
          </div>

          <div class="detail-item" v-if="notification.rejectedAt">
            <label>Rejected At</label>
            <div class="detail-value">
              {{ formatDateTime(notification.rejectedAt) }}
            </div>
          </div>

          <div class="detail-item" v-if="notification.rejectionReason">
            <label>Rejection Reason</label>
            <div class="detail-value alert-box">
              {{ notification.rejectionReason }}
            </div>
          </div>
        </div>
      </div>

      <!-- Timestamps -->
      <div class="card">
        <h2 class="section-title">
          <Clock :size="20" />
          Timestamps
        </h2>
        <div class="detail-grid">
          <div class="detail-item">
            <label>Created At</label>
            <div class="detail-value">
              {{ formatDateTime(notification.createdAt) }}
            </div>
          </div>

          <div class="detail-item" v-if="notification.updatedAt">
            <label>Updated At</label>
            <div class="detail-value">
              {{ formatDateTime(notification.updatedAt) }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { 
  FileText, Calendar, Package, CheckCircle, Clock, 
  Edit, Trash2, Send, XCircle, ChevronRight, 
  AlertCircle, Loader2 
} from 'lucide-vue-next'
import { useVesselVisitNotificationApi, type VesselVisitNotification } from '@/services/useVesselVisitNotificationApi'

const route = useRoute()
const router = useRouter()
const { 
  getNotification, 
  deleteNotification: apiDelete,
  submitNotification: apiSubmit,
  withdrawNotification: apiWithdraw,
  loading, 
  error 
} = useVesselVisitNotificationApi()

const notification = ref<VesselVisitNotification | null>(null)
const businessId = route.params.businessId as string

onMounted(async () => {
  await loadNotification()
})

async function loadNotification() {
  try {
    notification.value = await getNotification(businessId)
  } catch (err) {
    console.error('Failed to load notification:', err)
  }
}

// Computed properties
const hasOptionalInfo = computed(() => {
  if (!notification.value) return false
  return !!(
    notification.value.crewId || 
    notification.value.cargoManifestsId?.length || 
    notification.value.assignedDockId
  )
})

const hasApprovalInfo = computed(() => {
  if (!notification.value) return false
  return !!(
    notification.value.approvedByOfficerId || 
    notification.value.rejectedByOfficerId || 
    notification.value.approvalNotes || 
    notification.value.rejectionReason
  )
})

const canEdit = computed(() => {
  if (!notification.value) return false
  return notification.value.status === 'InProgress' || notification.value.status === 'Rejected'
})

const canDelete = computed(() => {
  if (!notification.value) return false
  return notification.value.status === 'InProgress'
})

const canSubmit = computed(() => {
  if (!notification.value) return false
  return notification.value.status === 'InProgress'
})

const canWithdraw = computed(() => {
  if (!notification.value) return false
  return notification.value.status === 'InProgress' || notification.value.status === 'PendingApproval'
})

// Actions
function editNotification() {
  router.push(`/notification/${businessId}/edit`)
}

async function deleteNotification() {
  if (!confirm(`Are you sure you want to delete notification ${businessId}?`)) {
    return
  }

  try {
    await apiDelete(businessId)
    router.push('/notification/list')
  } catch (err: any) {
    alert(`Failed to delete: ${err.message}`)
  }
}

async function submitNotification() {
  if (!confirm('Submit this notification for approval?')) {
    return
  }

  try {
    await apiSubmit(businessId)
    await loadNotification()
    alert('Notification submitted for approval!')
  } catch (err: any) {
    alert(`Failed to submit: ${err.message}`)
  }
}

async function withdrawNotification() {
  const reason = prompt('Enter withdrawal reason (optional):')
  
  try {
    await apiWithdraw(businessId)
    await loadNotification()
    alert('Notification withdrawn!')
  } catch (err: any) {
    alert(`Failed to withdraw: ${err.message}`)
  }
}

// Formatters
function formatDateTime(date?: string) {
  if (!date) return 'N/A'
  return new Date(date).toLocaleString('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatUuid(uuid: string) {
  return uuid.substring(0, 13) + '...'
}

function calculateDuration(eta?: string, etd?: string) {
  if (!eta || !etd) return 'N/A'
  const start = new Date(eta)
  const end = new Date(etd)
  const hours = Math.abs(end.getTime() - start.getTime()) / 36e5
  return `${hours.toFixed(1)} hours`
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
  margin-bottom: 2rem;
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: #6b7280;
  margin-bottom: 0.5rem;
}

.breadcrumb a {
  color: #3b82f6;
  text-decoration: none;
}

.breadcrumb a:hover {
  text-decoration: underline;
}

.page-header h1 {
  font-size: 1.875rem;
  font-weight: 700;
  color: #1e3a5f;
  margin: 0.5rem 0;
}

.status-badge-large {
  display: inline-block;
  padding: 0.375rem 1rem;
  border-radius: 9999px;
  font-size: 0.875rem;
  font-weight: 600;
  text-transform: uppercase;
  margin-top: 0.5rem;
}

.header-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 1rem;
  flex-wrap: wrap;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.25rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 1.5rem;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid #e5e7eb;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.detail-item label {
  font-size: 0.875rem;
  font-weight: 600;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.detail-value {
  font-size: 1rem;
  color: #1f2937;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.code-value {
  font-family: 'Courier New', monospace;
  background: #f3f4f6;
  padding: 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  word-break: break-all;
}

.code-chip {
  display: inline-block;
  background: #dbeafe;
  color: #1e40af;
  padding: 0.25rem 0.75rem;
  border-radius: 0.375rem;
  font-family: 'Courier New', monospace;
  font-size: 0.75rem;
  margin-right: 0.5rem;
  margin-bottom: 0.5rem;
}

.alert-box {
  background: #fef3c7;
  border-left: 4px solid #f59e0b;
  padding: 0.75rem;
  border-radius: 0.375rem;
  color: #92400e;
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

.loading-state {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
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

.error-message {
  padding: 1rem;
  background-color: #fee2e2;
  color: #991b1b;
  border-radius: 0.375rem;
  border: 1px solid #ef4444;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
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

.btn-secondary:hover {
  background-color: #4b5563;
}

.btn-danger {
  background-color: #ef4444;
  color: white;
}

.btn-danger:hover {
  background-color: #dc2626;
}

.btn-warning {
  background-color: #f59e0b;
  color: white;
}

.btn-warning:hover {
  background-color: #d97706;
}

.card {
  background: white;
  border-radius: 0.5rem;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  margin-bottom: 1.5rem;
}
</style>
