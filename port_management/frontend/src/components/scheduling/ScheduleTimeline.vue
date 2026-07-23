<template>
  <div class="timeline-container">
    <div class="timeline-controls">
      <div class="control-group">
        <label class="control-label">Zoom:</label>
        <button @click="zoomOut" class="zoom-btn" :disabled="zoom <= 0.25">−</button>
        <span class="zoom-value">{{ Math.round(zoom * 100) }}%</span>
        <button @click="zoomIn" class="zoom-btn" :disabled="zoom >= 3">+</button>
      </div>

      <div class="control-group">
        <label class="control-label">View:</label>
        <button @click="fitToContent" class="control-btn">Fit to Content</button>
        <button @click="resetView" class="control-btn">Reset</button>
      </div>

      <div class="legend">
        <div class="legend-item">
          <div class="legend-color normal"></div>
          <span>On Time</span>
        </div>
        <div class="legend-item">
          <div class="legend-color warning"></div>
          <span>Delay &lt; 24h</span>
        </div>
        <div class="legend-item">
          <div class="legend-color critical"></div>
          <span>Delay &gt; 24h</span>
        </div>
        <div class="legend-item">
          <div class="legend-color multi-crane"></div>
          <span>Multi-Crane</span>
        </div>
      </div>
    </div>

    <div class="timeline-scroll-wrapper" ref="scrollWrapper">
      <div class="timeline-inner" :style="{ minWidth: innerWidthPx + 'px' }">
        <div class="timeline-header-row">
          <div class="vessel-label header-placeholder">Vessel</div>
          <div class="timeline-scale">
            <div
                v-for="day in dayMarkers"
                :key="`day-${day.day}`"
                class="day-marker"
                :style="{ left: `${(day.startHour / maxHour) * 100}%` }"
            >
              <div class="day-label">{{ dayLabel(day.day) }}</div>
            </div>
            <div
                v-for="hour in timelineHours"
                :key="hour"
                class="hour-marker"
                :style="{ left: `${(hour / maxHour) * 100}%` }"
            >
              {{ formatHourMarker(hour) }}
            </div>
          </div>
        </div>

        <div class="timeline-rows">
          <div class="timeline-grid">
            <div
                v-for="day in dayMarkers"
                :key="`grid-day-${day.day}`"
                class="grid-line day-line"
                :style="{ left: `${(day.startHour / maxHour) * 100}%` }"
            />
            <div
                v-for="hour in timelineHours"
                :key="`grid-${hour}`"
                class="grid-line hour-line"
                :style="{ left: `${(hour / maxHour) * 100}%` }"
            />
          </div>

          <div
              v-for="(item, index) in processedSchedule"
              :key="item.vesselId"
              class="timeline-row"
              :class="{ 'row-even': index % 2 === 0 }"
          >
            <div class="vessel-label" :title="item.vesselId">
              {{ item.vesselId }}
            </div>
            <div class="vessel-bar-container">
              <div
                  v-if="item.etaHours !== undefined"
                  class="eta-marker"
                  :style="{ left: `${(item.etaHours / maxHour) * 100}%` }"
                  :title="`ETA: ${formatTime(item.etaHours)}`"
              >
                <div class="eta-flag">ETA</div>
              </div>
              <div
                  v-if="item.etdHours !== undefined"
                  class="etd-marker"
                  :style="{ left: `${(item.etdHours / maxHour) * 100}%` }"
                  :title="`ETD: ${formatTime(item.etdHours)}`"
              >
                <div class="etd-flag">ETD</div>
              </div>

              <div
                  class="vessel-bar"
                  :class="{ 
                    'no-resources': isNoResources(item),
                    'delay-critical': !isNoResources(item) && item.delayHours >= 24,
                    'delay-warning': !isNoResources(item) && item.delayHours > 0 && item.delayHours < 24,
                    'multi-crane': !isNoResources(item) && item.delayHours <= 0 && item.craneCount && item.craneCount > 1
                  }"
                  :style="{
                    left: `${(item.startTimeHours / maxHour) * 100}%`,
                    width: `${Math.max(0.5, ((item.endTimeHours - item.startTimeHours) / maxHour) * 100)}%`
                  }"
                  :title="barTitle(item)"
                  @click="onBarClick(item)"
              >
                <div class="bar-label">
                  <span class="bar-duration">{{ formatDuration(item) }}</span>

                  <span v-if="item.craneCount && item.craneCount > 1" class="crane-indicator">
                    {{ item.craneCount }}×
                  </span>

                  <span v-if="item.delay > 0" class="delay-indicator">
                    +{{ formatFriendlyDelay(item.delayHours) }}
                  </span>
                </div>
              </div>

              <div
                  v-if="item.delay > 0 && item.etdHours"
                  class="delay-extension"
                  :style="{
                    left: `${(item.etdHours / maxHour) * 100}%`,
                    width: `${((item.endTimeHours - item.etdHours) / maxHour) * 100}%`
                  }"
                  :title="`Delay: ${formatFriendlyDelay(item.delayHours)} beyond ETD`"
              />
            </div>
          </div>

          <div v-if="processedSchedule.length === 0" class="empty-state">
            No schedule data to display
          </div>
        </div>
      </div>
    </div>

    <div class="timeline-stats">
      <div class="stat-item">
        <span class="stat-label">Total Vessels:</span>
        <span class="stat-value">{{ schedule.length }}</span>
      </div>
      <div class="stat-item">
        <span class="stat-label">Delayed:</span>
        <span class="stat-value stat-delayed">{{ delayedCount }}</span>
      </div>
      <div class="stat-item">
        <span class="stat-label">Timespan:</span>
        <span class="stat-value">{{ formatDays(maxHour) }}</span>
      </div>
      <div class="stat-item">
        <span class="stat-label">Total Delay:</span>
        <span class="stat-value stat-delayed">{{ (totalDelay / 60).toFixed(2) }}h</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, defineEmits } from 'vue'

const props = defineProps<{
  schedule: any[]
  targetDate?: string
}>()

const emit = defineEmits(['barClick'])

// Conversão Minutos -> Horas (CRUCIAL)
const processedSchedule = computed(() => {
  return props.schedule.map(item => {
    const startTimeHours = (item.startTime || 0) / 60
    const endTimeHours = (item.endTime || 0) / 60
    const etaHours = item.eta !== undefined ? item.eta / 60 : undefined
    const etdHours = item.etd !== undefined ? item.etd / 60 : undefined
    const delayHours = (item.delay || 0) / 60
    const durationHours = endTimeHours - startTimeHours

    return {
      ...item,
      startTimeHours,
      endTimeHours,
      etaHours,
      etdHours,
      delayHours,
      durationHours
    }
  })
})

const zoom = ref(1)
const scrollWrapper = ref<HTMLElement | null>(null)

// MaxHour em Horas
const maxHour = computed(() => {
  if (processedSchedule.value.length === 0) return 24
  const maxEnd = Math.max(...processedSchedule.value.map(s => s.endTimeHours))
  return Math.max(24, Math.ceil(maxEnd / 12) * 12 + 4)
})

const timelineHours = computed(() => {
  const hours = []
  for (let i = 0; i <= maxHour.value; i += 6) hours.push(i)
  return hours
})

const dayMarkers = computed(() => {
  const days = []
  const numDays = Math.ceil(maxHour.value / 24)
  for (let d = 0; d <= numDays; d++) {
    days.push({ day: d, startHour: d * 24 })
  }
  return days
})

const delayedCount = computed(() => props.schedule.filter(s => s.delay > 0).length)
const totalDelay = computed(() => props.schedule.reduce((sum, s) => sum + (Number(s.delay) || 0), 0))

const innerWidthPx = computed(() => {
  const baseWidth = Math.max(1200, maxHour.value * 60)
  return Math.round(baseWidth * zoom.value)
})

const baseDate = computed(() => {
  if (props.targetDate) {
    const d = new Date(`${props.targetDate}T00:00:00`)
    if (!Number.isNaN(d.getTime())) return d
  }
  const now = new Date()
  return new Date(now.getFullYear(), now.getMonth(), now.getDate())
})

const pad2 = (n: number) => String(n).padStart(2, '0')
const formatDateShort = (d: Date) => `${pad2(d.getDate())}/${pad2(d.getMonth() + 1)}`

const dayLabel = (dayIndex: number) => {
  const d = new Date(baseDate.value)
  d.setDate(d.getDate() + dayIndex)
  return formatDateShort(d)
}

const formatHourMarker = (hour: number) => `${String(hour % 24).padStart(2, '0')}:00`

const formatTime = (hour: number | undefined) => {
  if (hour === null || hour === undefined) return ''
  const dayOffset = Math.floor(hour / 24)
  const hourOfDay = Math.floor(hour % 24)
  const mins = Math.round((hour - Math.floor(hour)) * 60)
  const d = new Date(baseDate.value)
  d.setDate(d.getDate() + dayOffset)
  return `${formatDateShort(d)} ${pad2(hourOfDay)}:${pad2(mins)}`
}

const formatFriendlyDelay = (hours: number) => {
  if (hours < 1) return Math.round(hours * 60) + 'm'
  return hours.toFixed(1) + 'h'
}

const formatDuration = (item: any) => {
  const h = Math.floor(item.durationHours)
  const m = Math.round((item.durationHours - h) * 60)
  return `${pad2(h)}:${pad2(m)}`
}

const formatDays = (hours: number) => `${Math.floor(hours / 24)}d`

const isNoResources = (item: any) => {
  return item.assignedCrane === 'NO_CRANE' || item.assignedStaff === 'NO_STAFF'
}

const barTitle = (item: any) => {
  const start = formatTime(item.startTimeHours)
  const end = formatTime(item.endTimeHours)
  return `Vessel: ${item.vesselId}\nStart: ${start}\nEnd: ${end}\nDelay: ${formatFriendlyDelay(item.delayHours)}`
}

const onBarClick = (item: any) => emit('barClick', item)
const zoomIn = () => { if (zoom.value < 3) zoom.value = Math.min(3, zoom.value + 0.25) }
const zoomOut = () => { if (zoom.value > 0.25) zoom.value = Math.max(0.25, zoom.value - 0.25) }
const fitToContent = () => {
  if (!scrollWrapper.value) return
  zoom.value = Math.max(0.5, Math.min(1, scrollWrapper.value.clientWidth / (maxHour.value * 60)))
}
const resetView = () => {
  zoom.value = 1
  if (scrollWrapper.value) scrollWrapper.value.scrollLeft = 0
}
</script>

<style scoped>
/* (Estilos Container/Layout mantidos iguais ao anterior...) */
.timeline-container {
  background: white;
  border-radius: 0.75rem;
  border: 1px solid #e5e7eb;
  overflow: hidden;
  margin-top: 1rem;
}
.timeline-controls {
  display: flex; justify-content: space-between; align-items: center; padding: 1rem;
  background: #f9fafb; border-bottom: 1px solid #e5e7eb; gap: 1rem; flex-wrap: wrap;
}
.control-group { display: flex; align-items: center; gap: 0.5rem; }
.control-label { font-size: 0.875rem; font-weight: 600; color: #374151; }
.zoom-btn, .control-btn {
  padding: 0.375rem 0.75rem; border: 1px solid #d1d5db; background: white;
  border-radius: 0.375rem; font-size: 0.875rem; cursor: pointer;
}
.zoom-value { font-size: 0.875rem; font-weight: 600; color: #2563eb; min-width: 48px; text-align: center; }

/* LEGEND STYLES */
.legend { display: flex; gap: 1rem; margin-left: auto; }
.legend-item { display: flex; align-items: center; gap: 0.5rem; font-size: 0.875rem; color: #6b7280; }
.legend-color { width: 24px; height: 12px; border-radius: 0.25rem; }

/* CORES DA LEGENDA */
.legend-color.normal { background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%); } /* Azul */
.legend-color.warning { background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); } /* Laranja */
.legend-color.critical { background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%); } /* Vermelho */
.legend-color.multi-crane { background: linear-gradient(135deg, #10b981 0%, #059669 100%); } /* Verde */

/* TIMELINE BODY STYLES */
.timeline-scroll-wrapper { overflow-x: auto; overflow-y: hidden; }
.timeline-inner { display: flex; flex-direction: column; min-width: 1200px; position: relative; padding: 1rem; }
.timeline-header-row { display: flex; align-items: flex-end; height: 48px; border-bottom: 2px solid #e5e7eb; margin-bottom: 0.5rem; }
.header-placeholder { width: 100px; flex-shrink: 0; font-weight: 700; color: #111827; font-size: 0.875rem; }
.timeline-scale { flex: 1; position: relative; height: 100%; }
.day-marker { position: absolute; top: 0; height: 24px; display: flex; align-items: center; border-left: 2px solid #2563eb; }
.day-label { font-size: 0.75rem; font-weight: 700; color: #2563eb; background: #eff6ff; padding: 0.125rem 0.5rem; margin-left: 4px; border-radius: 2px; }
.hour-marker { position: absolute; bottom: 0; font-size: 0.7rem; color: #9ca3af; font-weight: 500; transform: translateX(-50%); }
.timeline-rows { flex: 1; display: flex; flex-direction: column; position: relative; }
.timeline-grid { position: absolute; top: 0; left: 100px; right: 0; bottom: 0; pointer-events: none; }
.grid-line { position: absolute; top: 0; bottom: 0; width: 1px; }
.grid-line.day-line { background: #d1d5db; z-index: 1; }
.grid-line.hour-line { background: #f3f4f6; }
.timeline-row { display: flex; align-items: center; margin-bottom: 0.375rem; background: white; padding: 0.25rem 0; }
.timeline-row.row-even { background: #fafbfc; }
.vessel-label { width: 100px; font-weight: 600; color: #2563eb; font-size: 0.8rem; flex-shrink: 0; padding-right: 0.75rem; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.vessel-bar-container { flex: 1; position: relative; height: 40px; }
.eta-marker { position: absolute; top: 0; bottom: 0; width: 2px; background: #10b981; z-index: 5; }
.etd-marker { position: absolute; top: 0; bottom: 0; width: 2px; background: #f59e0b; z-index: 5; }
.eta-flag, .etd-flag { position: absolute; top: -18px; left: 50%; transform: translateX(-50%); font-size: 0.6rem; font-weight: 700; padding: 0 4px; border-radius: 2px; white-space: nowrap; }
.eta-flag { background: #d1fae5; color: #065f46; }
.etd-flag { background: #fef3c7; color: #92400e; }

/* --- BAR STYLES (UPDATED) --- */
.vessel-bar {
  position: absolute; top: 6px; height: 28px;
  /* COR PADRÃO (ON TIME) - AZUL */
  background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
  border-radius: 4px;
  display: flex; align-items: center; justify-content: center;
  color: white; font-size: 0.75rem; font-weight: 600;
  box-shadow: 0 2px 4px rgba(37, 99, 235, 0.25);
  cursor: pointer; z-index: 10; transition: all 0.2s;
}
.vessel-bar:hover { z-index: 20; transform: scaleY(1.1); }

/* WARNING: Delay < 24h (Laranja) */
.vessel-bar.delay-warning {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  box-shadow: 0 2px 4px rgba(217, 119, 6, 0.25);
}

/* CRITICAL: Delay >= 24h (Vermelho) */
.vessel-bar.delay-critical {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  box-shadow: 0 2px 4px rgba(220, 38, 38, 0.25);
}

/* MULTI-CRANE (Verde - Só aparece se não houver delay) */
.vessel-bar.multi-crane {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  box-shadow: 0 2px 4px rgba(5, 150, 105, 0.25);
}

/* NO RESOURCES (Cinza) */
.vessel-bar.no-resources {
  background: linear-gradient(135deg, #9ca3af 0%, #6b7280 100%);
  box-shadow: 0 2px 4px rgba(107, 114, 128, 0.25);
}

.bar-label { display: flex; gap: 0.5rem; align-items: center; padding: 0 0.5rem; white-space: nowrap; overflow: hidden; }
.crane-indicator, .delay-indicator {
  display: inline-flex; align-items: center; justify-content: center;
  padding: 0.125rem 0.375rem; background: rgba(255, 255, 255, 0.25);
  border-radius: 999px; font-size: 0.625rem; font-weight: 700;
}

.delay-extension {
  position: absolute; top: 6px; height: 28px;
  background: repeating-linear-gradient(45deg, rgba(239, 68, 68, 0.15), rgba(239, 68, 68, 0.15) 5px, transparent 5px, transparent 10px);
  border: 1px dashed #ef4444; border-radius: 4px; pointer-events: none;
}

.timeline-stats { display: flex; justify-content: space-around; padding: 1rem; background: #f9fafb; border-top: 1px solid #e5e7eb; }
.stat-item { display: flex; flex-direction: column; align-items: center; }
.stat-label { font-size: 0.75rem; color: #6b7280; font-weight: 600; }
.stat-value { font-size: 1.1rem; font-weight: 700; color: #111827; }
.stat-value.stat-delayed { color: #dc2626; }
.empty-state { text-align: center; padding: 2rem; color: #9ca3af; font-style: italic; }
</style>