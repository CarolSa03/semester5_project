<template>
  <div class="update-operation-plan-container">
    <h1>Update Operation Plan</h1>
    <p class="subtitle">US 4.1.4 - Modify existing operation plan schedules and resources</p>

    <!-- Loading State -->
    <div v-if="viewModel.isLoading.value" class="loading-state">
      <div class="spinner-large"></div>
      <p>Loading operation plan...</p>
    </div>

    <!-- Main Content -->
    <div v-else-if="viewModel.originalPlan.value">
      <!-- Plan Header Info -->
      <div class="info-card">
        <h2>Plan Information</h2>
        <div class="info-grid">
          <div class="info-item">
            <strong>Plan ID:</strong> {{ viewModel.originalPlan.value.id }}
          </div>
          <div class="info-item">
            <strong>VVN ID:</strong> {{ viewModel.originalPlan.value.vvnId }}
          </div>
          <div class="info-item">
            <strong>Date:</strong> {{ viewModel.originalPlan.value.date }}
          </div>
          <div class="info-item">
            <strong>Algorithm:</strong> {{ viewModel.originalPlan.value.metrics.algorithm }}
          </div>
        </div>
      </div>

      <!-- Update Metadata Form -->
      <div class="form-card">
        <h2>Update Details</h2>

        <div class="form-group">
          <label for="author">Modified By *</label>
          <input
            id="author"
            v-model="viewModel.author.value"
            type="text"
            placeholder="Your name"
            :disabled="viewModel.isSaving.value"
            required
          />
        </div>

        <div class="form-group">
          <label for="reason">Modification Reason *</label>
          <textarea
            id="reason"
            v-model="viewModel.reason.value"
            rows="3"
            placeholder="Explain why this plan is being modified..."
            :disabled="viewModel.isSaving.value"
            required
          ></textarea>
        </div>

        <div class="form-group">
          <label for="status">Status</label>
          <select
            id="status"
            v-model="viewModel.status.value"
            :disabled="viewModel.isSaving.value"
          >
            <option value="ACTIVE">Active</option>
            <option value="SUPERSEDED">Superseded</option>
            <option value="ARCHIVED">Archived</option>
          </select>
        </div>
      </div>

      <!-- Operations Editor -->
      <div class="operations-card">
        <div class="operations-header">
          <h2>Scheduled Operations</h2>
          <button
            class="btn-secondary"
            @click="viewModel.toggleComparison()"
            v-if="viewModel.hasChanges()"
          >
            {{ viewModel.showComparison.value ? 'Hide' : 'Show' }} Comparison
          </button>
        </div>

        <div class="operations-table-wrapper">
          <table class="operations-table">
            <thead>
              <tr>
                <th>Vessel ID</th>
                <th>Start Time</th>
                <th>End Time</th>
                <th>Duration</th>
                <th>Assigned Crane</th>
                <th>Delay (min)</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(operation, index) in viewModel.editedOperations.value"
                :key="index"
                :class="{ 'modified-row': viewModel.showComparison.value && hasOperationChanged(index) }"
              >
                <td>{{ operation.vesselId }}</td>
                <td>
                  <input
                    type="datetime-local"
                    :value="timestampToDatetimeLocal(operation.startTime)"
                    @change="updateStartTime(index, $event)"
                    :disabled="viewModel.isSaving.value"
                    class="time-input"
                  />
                </td>
                <td>
                  <input
                    type="datetime-local"
                    :value="timestampToDatetimeLocal(operation.endTime)"
                    @change="updateEndTime(index, $event)"
                    :disabled="viewModel.isSaving.value"
                    class="time-input"
                  />
                </td>
                <td>{{ viewModel.formatDuration(operation.durationMinutes) }}</td>
                <td>
                  <input
                      :value="operation.assignedCranes[0]"
                      @input="updateResource(index, $event)"
                      type="text"
                      :disabled="viewModel.isSaving.value"
                      class="resource-input"
                  />
                </td>
                <td :class="{ 'delay-warning': operation.delay > 0 }">
                  {{ operation.delay }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Comparison View -->
        <div v-if="viewModel.showComparison.value && viewModel.hasChanges()" class="comparison-panel">
          <h3>Changes Summary</h3>
          <ul class="changes-list">
            <li v-for="(change, index) in getChanges()" :key="index">
              {{ change }}
            </li>
          </ul>
        </div>
      </div>

      <!-- Error/Success Messages -->
      <div v-if="viewModel.errorMessage.value" class="error-message">
        <span class="icon">⚠️</span>
        {{ viewModel.errorMessage.value }}
      </div>

      <div v-if="viewModel.successMessage.value" class="success-message">
        <span class="icon">✅</span>
        {{ viewModel.successMessage.value }}
      </div>

      <!-- Action Buttons -->
      <div class="button-group">
        <button
          class="btn-primary"
          @click="handleUpdate"
          :disabled="viewModel.isSaving.value || !viewModel.hasChanges()"
        >
          <span v-if="viewModel.isSaving.value" class="spinner"></span>
          {{ viewModel.isSaving.value ? 'Saving...' : 'Save Changes' }}
        </button>

        <button
          class="btn-secondary"
          @click="viewModel.resetChanges()"
          :disabled="viewModel.isSaving.value"
        >
          Reset
        </button>

        <router-link to="/operation-plans/list" class="btn-secondary">
          Cancel
        </router-link>
      </div>
    </div>

    <!-- Error State -->
    <div v-else class="error-state">
      <p>{{ viewModel.errorMessage.value || 'Operation plan not found' }}</p>
      <router-link to="/operation-plans/list" class="btn-primary">
        Back to List
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { UpdateOperationPlanViewModel } from '@/viewmodels/operationPlan/UpdateOperationPlanViewModel';

const route = useRoute();
const router = useRouter();
const viewModel = new UpdateOperationPlanViewModel();

onMounted(() => {
  const planId = route.params.id as string;
  if (planId) {
    viewModel.loadPlan(planId);
  }
});

function timestampToDatetimeLocal(timestamp: number): string {
  const date = new Date(timestamp);
  const offset = date.getTimezoneOffset() * 60000;
  const localDate = new Date(date.getTime() - offset);
  return localDate.toISOString().slice(0, 16);
}

function updateStartTime(index: number, event: Event): void {
  const target = event.target as HTMLInputElement;
  const newTime = new Date(target.value).getTime();
  const operation = viewModel.editedOperations.value[index];
  viewModel.updateOperationTime(index, newTime, operation.endTime);
}

function updateEndTime(index: number, event: Event): void {
  const target = event.target as HTMLInputElement;
  const newTime = new Date(target.value).getTime();
  const operation = viewModel.editedOperations.value[index];
  viewModel.updateOperationTime(index, operation.startTime, newTime);
}

function updateResource(index: number, event: Event): void {
  const target = event.target as HTMLInputElement;
  viewModel.updateOperationResource(index, target.value);
}

function hasOperationChanged(index: number): boolean {
  if (!viewModel.originalPlan.value) return false;
  const original = viewModel.originalPlan.value.schedule[index];
  const edited = viewModel.editedOperations.value[index];
  return JSON.stringify(original) !== JSON.stringify(edited);
}

function getChanges(): string[] {
  if (!viewModel.originalPlan.value) return [];
  
  const changes: string[] = [];
  viewModel.editedOperations.value.forEach((edited, index) => {
    const original = viewModel.originalPlan.value!.schedule[index];
    
    if (edited.startTime !== original.startTime) {
      changes.push(`Operation ${index + 1}: Start time changed`);
    }
    if (edited.endTime !== original.endTime) {
      changes.push(`Operation ${index + 1}: End time changed`);
    }
    if (edited.assignedCranes[0] !== original.assignedCranes[0]) {
      changes.push(`Operation ${index + 1}: Crane changed from ${original.assignedCranes[0]} to ${edited.assignedCranes[0]}`);
    }
  });
  
  return changes;
}

async function handleUpdate(): Promise<void> {
  const success = await viewModel.updatePlan();
  if (success) {
    setTimeout(() => {
      router.push({ name: 'operation-plan-list' });
    }, 2000);
  }
}
</script>

<style scoped>
.update-operation-plan-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
}

h1 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.subtitle {
  color: #7f8c8d;
  margin-bottom: 2rem;
}

.loading-state,
.error-state {
  text-align: center;
  padding: 3rem;
}

.spinner-large {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(52, 152, 219, 0.3);
  border-top-color: #3498db;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin: 0 auto 1rem;
}

.info-card,
.form-card,
.operations-card {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.info-item {
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 4px;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #2c3e50;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
  border-color: #3498db;
}

.operations-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.operations-table-wrapper {
  overflow-x: auto;
}

.operations-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.operations-table thead {
  background-color: #34495e;
  color: white;
}

.operations-table th,
.operations-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

.operations-table tbody tr:hover {
  background-color: #f8f9fa;
}

.modified-row {
  background-color: #fff3cd !important;
  border-left: 3px solid #ffc107;
}

.time-input,
.resource-input {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 0.9rem;
  width: 100%;
}

.time-input:focus,
.resource-input:focus {
  outline: none;
  border-color: #3498db;
}

.delay-warning {
  color: #e74c3c;
  font-weight: bold;
}

.comparison-panel {
  margin-top: 2rem;
  padding: 1.5rem;
  background: #f8f9fa;
  border-radius: 4px;
  border-left: 4px solid #3498db;
}

.comparison-panel h3 {
  margin-top: 0;
  color: #2c3e50;
}

.changes-list {
  list-style: none;
  padding: 0;
}

.changes-list li {
  padding: 0.5rem 0;
  border-bottom: 1px solid #ddd;
}

.changes-list li:last-child {
  border-bottom: none;
}

.error-message,
.success-message {
  padding: 1rem;
  border-radius: 4px;
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.error-message {
  background-color: #fee;
  color: #c33;
  border: 1px solid #fcc;
}

.success-message {
  background-color: #efe;
  color: #3c3;
  border: 1px solid #cfc;
}

.button-group {
  display: flex;
  gap: 1rem;
}

.btn-primary,
.btn-secondary {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.3s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  color: white;
}

.btn-primary {
  background-color: #3498db;
}

.btn-primary:hover:not(:disabled) {
  background-color: #2980b9;
}

.btn-secondary {
  background-color: #95a5a6;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #7f8c8d;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
