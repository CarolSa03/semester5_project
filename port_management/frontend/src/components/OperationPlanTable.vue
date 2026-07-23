<template>
  <div class="operation-plan-table">
    <table>
      <thead>
        <tr>
          <th>Vessel ID</th>
          <th>Dock ID</th>
          <th>Start Time</th>
          <th>End Time</th>
          <th>Duration (min)</th>
          <th>Assigned Cranes</th>
          <th>Delay (min)</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(operation, index) in operations" :key="index">
          <td>{{ operation.vesselId }}</td>
          <td>{{ operation.dockId }}</td>
          <td>{{ formatTimestamp(operation.startTime) }}</td>
          <td>{{ formatTimestamp(operation.endTime) }}</td>
          <td>{{ operation.durationMinutes }}</td>
          <td>{{ operation.assignedCranes.join(', ') }}</td>
          <td :class="{ 'delay-warning': operation.delay > 0 }">
            {{ operation.delay }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { IPlannedOperation } from '@/entities/OperationPlan';

defineProps<{
  operations: IPlannedOperation[];
}>();

function formatTimestamp(timestamp: number): string {
  return new Date(timestamp).toLocaleString();
}
</script>

<style scoped>
.operation-plan-table {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

thead {
  background-color: #34495e;
  color: white;
}

th, td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

tbody tr:hover {
  background-color: #f8f9fa;
}

.delay-warning {
  color: #e74c3c;
  font-weight: bold;
}
</style>
