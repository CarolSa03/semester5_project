<template>
  <div class="schedule-table-container">
    <table class="schedule-table">
      <thead>
      <tr>
        <th>Vessel ID</th>
        <th>Start Time</th>
        <th>End Time</th>
        <th>Duration</th>
        <th>Assigned Crane</th>
        <th>Assigned Staff</th>
        <th>Delay</th>
        <th v-if="hasCranes">Cranes</th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="item in sortedSchedule" :key="item.vesselId">
        <td class="vessel-cell">{{ item.vesselId }}</td>
        <td>{{ formatDateTime(item.startTime) }}</td>

        <td>{{ formatDateTime(item.endTime) }}</td>

        <td>{{ formatDuration(item) }}</td>

        <td>
          <template v-if="item.crane">
            {{ String(item.crane) }}
          </template>
          <template v-else>
            {{ item.assignedCrane ?? '—' }}
          </template>
        </td>

        <td>
          <template v-if="item.assignedStaff">
            {{ item.assignedStaff }}
          </template>
          <template v-else>
            {{ item.staff ?? item.staffId ?? '—' }}
          </template>
        </td>

        <td>
          <span
              class="status-badge"
              :class="{
              'status-ok': !item.delay || item.delay <= 0,
              'status-warning': (item.delay / 60) > 0 && (item.delay / 60) < 24,
              'status-critical': (item.delay / 60) >= 24
            }"
          >
            <span v-if="item.delay > 0">️</span>
            <span v-else></span>
            
            {{ formatFriendlyDelay(item.delay / 60) }}
          </span>
        </td>

        <td v-if="hasCranes" :class="{ 'multi-crane': (item.craneCount || 1) > 1 }">
          {{ item.craneCount || (item.crane ? String(item.crane).split(',').length : 1) }}
          <span v-if="(item.craneCount || (item.crane ? String(item.crane).split(',').length : 1)) > 1" class="crane-badge">
            ×{{ item.craneCount || (item.crane ? String(item.crane).split(',').length : 1) }}
          </span>
        </td>
      </tr>
      </tbody>
      <tfoot>
      <tr>
        <td colspan="6"><strong>Total Delay:</strong></td>
        <td :colspan="hasCranes ? 2 : 1"><strong>{{ totalDelay }}h</strong></td>
      </tr>
      </tfoot>
    </table>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  schedule: any[],
  targetDate?: string
}>()

// --- NOVA LÓGICA DE ORDENAÇÃO ---
const sortedSchedule = computed(() => {
  // Cria uma cópia para não alterar a prop original e ordena por startTime
  return [...props.schedule].sort((a, b) => {
    const startA = Number(a.startTime) || 0
    const startB = Number(b.startTime) || 0
    return startA - startB
  })
})
// -------------------------------

const pad = (n: number) => String(n).padStart(2, '0')

const formatDateTime = (minutesInput: any) => {
  if (minutesInput === undefined || minutesInput === null) return '—'

  const totalMinutes = Number(minutesInput)
  const dateObj = props.targetDate ? new Date(props.targetDate) : new Date()

  // Resetar horas para o início do dia alvo
  dateObj.setHours(0, 0, 0, 0)
  // Adicionar minutos do agendamento
  dateObj.setMinutes(dateObj.getMinutes() + totalMinutes)

  const day = pad(dateObj.getDate())
  const month = pad(dateObj.getMonth() + 1)
  const hour = pad(dateObj.getHours())
  const minute = pad(dateObj.getMinutes())

  return `${day}/${month} ${hour}:${minute}`
}

const formatDuration = (item: any) => {
  if (item.durationLabel) return item.durationLabel
  let hours: number | undefined
  if (item.estimatedDuration !== undefined) hours = item.estimatedDuration
  else if (item.durationHours !== undefined) hours = item.durationHours
  else if (item.durationMinutes !== undefined) hours = item.durationMinutes / 60

  if (hours === undefined && item.endTime !== undefined && item.startTime !== undefined) {
    hours = (item.endTime - item.startTime) / 60
  }
  return (typeof hours === 'number' && !isNaN(hours)) ? hours.toFixed(2) + 'h' : '—'
}

const hasCranes = computed(() => {
  return props.schedule.some(s =>
      (s.assignedCrane && s.assignedCrane.length > 1) ||
      (s.craneCount && s.craneCount > 1)
  )
})

const totalDelay = computed(() => {
  const totalMinutes = props.schedule.reduce((sum, item) => sum + (Number(item.delay) || 0), 0)
  return (totalMinutes / 60).toFixed(2)
})

const formatFriendlyDelay = (hours: number) => {
  if (!hours || hours <= 0) return 'On Time'

  if (hours < 1) {
    const mins = Math.round(hours * 60)
    return `${mins}m`
  }

  if (hours < 24) {
    const h = Math.floor(hours)
    const m = Math.round((hours - h) * 60)
    return `${h}h ${m > 0 ? m + 'm' : ''}`
  }

  const days = Math.floor(hours / 24)
  const remainingHours = Math.round(hours % 24)
  return `${days}d ${remainingHours}h`
}
</script>

<style scoped>
.schedule-table-container {
  overflow-x: auto;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
}

.schedule-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
  background: white;
}

.schedule-table th,
.schedule-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.schedule-table thead {
  background: #f9fafb;
}

.schedule-table th {
  font-weight: 600;
  color: #374151;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.5px;
}

.schedule-table tbody tr:hover {
  background: #f9fafb;
}

.vessel-cell {
  font-weight: 600;
  color: #2563eb;
}

.multi-crane {
  color: #059669;
  font-weight: 600;
}

.crane-badge {
  display: inline-block;
  margin-left: 0.25rem;
  padding: 0.125rem 0.375rem;
  background: #d1fae5;
  color: #065f46;
  border-radius: 999px;
  font-size: 0.7rem;
  font-weight: 600;
}

.schedule-table tfoot {
  background: #f9fafb;
  font-weight: 600;
}

.schedule-table tfoot td {
  border-top: 2px solid #d1d5db;
}

/* Badges de Status */
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 600;
  white-space: nowrap;
}

.status-ok {
  background-color: #ecfdf5;
  color: #059669;
  border: 1px solid #d1fae5;
}

.status-warning {
  background-color: #fffbeb;
  color: #d97706;
  border: 1px solid #fde68a;
}

.status-critical {
  background-color: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
}
</style>