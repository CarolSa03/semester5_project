<!-- src/components/scheduling/ComparisonTable.vue -->
<template>
  <div class="comparison-container">
    <table class="comparison-table">
      <thead>
      <tr>
        <th>Algorithm</th>
        <th>Total Delay</th>
        <th>Computation Time</th>
        <th>Quality vs Optimal</th>
      </tr>
      </thead>
      <tbody>
      <tr
          v-for="result in results"
          :key="result.algorithm"
          :class="{ 'best-result': result.quality === 100 }"
      >
        <td class="algorithm-cell">
          {{ formatAlgorithm(result.algorithm) }}
          <span v-if="result.quality === 100" class="best-badge">Best</span>
        </td>
        <td :class="getDelayClass(result.totalDelay)">
          {{ result.totalDelay }}h
        </td>
        <td class="time-cell">{{ result.computationTime }}</td>
        <td>
          <div class="quality-bar-container">
            <div
                class="quality-bar"
                :style="{ width: `${result.quality}%` }"
                :class="getQualityClass(result.quality)"
            ></div>
            <span class="quality-text">{{ result.quality }}%</span>
          </div>
        </td>
      </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  results: Array<{
    algorithm: string
    totalDelay: number
    computationTime: string
    quality: number
  }>
}>()

const formatAlgorithm = (algo: string): string => {
  const names: Record<string, string> = {
    'optimal': 'Optimal (Permutations)',
    'edt': 'EDT (Early Departure)',
    'eat': 'EAT (Early Arrival)',
    'spt': 'SPT (Shortest Processing)',
    'mst': 'MST (Minimum Slack)'
  }
  return names[algo] || algo.toUpperCase()
}

const getDelayClass = (delay: number): string => {
  if (delay === 0) return 'delay-zero'
  if (delay < 300) return 'delay-low'
  if (delay < 400) return 'delay-medium'
  return 'delay-high'
}

const getQualityClass = (quality: number): string => {
  if (quality >= 90) return 'quality-excellent'
  if (quality >= 70) return 'quality-good'
  if (quality >= 50) return 'quality-fair'
  return 'quality-poor'
}
</script>

<style scoped>
.comparison-container {
  overflow-x: auto;
}

.comparison-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.comparison-table th,
.comparison-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.comparison-table thead {
  background: #f9fafb;
}

.comparison-table th {
  font-weight: 600;
  color: #374151;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.5px;
}

.comparison-table tbody tr {
  transition: background 0.2s;
}

.comparison-table tbody tr:hover {
  background: #f9fafb;
}

.comparison-table tbody tr.best-result {
  background: #f0fdf4;
}

.algorithm-cell {
  font-weight: 600;
  color: #111827;
}

.best-badge {
  display: inline-block;
  margin-left: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: #10b981;
  color: white;
  border-radius: 999px;
  font-size: 0.7rem;
  font-weight: 600;
}

.delay-zero {
  color: #10b981;
  font-weight: 600;
}

.delay-low {
  color: #f59e0b;
}

.delay-medium {
  color: #ef4444;
}

.delay-high {
  color: #dc2626;
  font-weight: 600;
}

.time-cell {
  color: #6b7280;
  font-family: ui-monospace, monospace;
}

.quality-bar-container {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.quality-bar {
  height: 8px;
  border-radius: 999px;
  transition: width 0.3s;
}

.quality-excellent {
  background: #10b981;
}

.quality-good {
  background: #3b82f6;
}

.quality-fair {
  background: #f59e0b;
}

.quality-poor {
  background: #ef4444;
}

.quality-text {
  font-weight: 600;
  font-size: 0.85rem;
  color: #374151;
  min-width: 45px;
}
</style>