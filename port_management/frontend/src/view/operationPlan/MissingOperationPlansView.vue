<template>
  <div class="missing-plans-container">
    <h1>Missing Operation Plans</h1>
    <p class="subtitle">US 4.1.5 - Identify VVNs without scheduled operation plans</p>

    <!-- Filter Card -->
    <div class="filter-card">
      <h2>Date Range Filter</h2>

      <div class="filter-row">
        <div class="form-group">
          <label for="startDate">Start Date</label>
          <input
            id="startDate"
            v-model="viewModel.startDate.value"
            type="date"
            :disabled="viewModel.isLoading.value"
          />
        </div>

        <div class="form-group">
          <label for="endDate">End Date</label>
          <input
            id="endDate"
            v-model="viewModel.endDate.value"
            type="date"
            :disabled="viewModel.isLoading.value"
          />
        </div>
      </div>

      <div class="button-group">
        <button
          class="btn-primary"
          @click="viewModel.applyFilters()"
          :disabled="viewModel.isLoading.value"
        >
          <span v-if="viewModel.isLoading.value" class="spinner"></span>
          {{ viewModel.isLoading.value ? 'Searching...' : 'Apply Filter' }}
        </button>

        <button
          class="btn-secondary"
          @click="viewModel.resetFilters()"
          :disabled="viewModel.isLoading.value"
        >
          Reset
        </button>

        <button
          class="btn-secondary"
          @click="viewModel.exportToCSV()"
          :disabled="viewModel.isLoading.value || viewModel.missingCount.value === 0"
        >
          Export CSV
        </button>
      </div>

      <!-- Error Message -->
      <div v-if="viewModel.errorMessage.value" class="error-message">
        <span class="icon">Error: </span>
        {{ viewModel.errorMessage.value }}
      </div>
    </div>

    <!-- Results Card -->
    <div class="results-card">
      <div class="results-header">
        <div>
          <h2>VVNs Without Operation Plans</h2>
          <p class="count-badge" :class="viewModel.missingCount.value > 0 ? 'warning' : 'success'">
            {{ viewModel.missingCount.value }} missing plan(s)
          </p>
        </div>

        <div class="header-actions" v-if="viewModel.missingCount.value > 0">
          <span v-if="viewModel.getSelectedCount() > 0" class="selection-info">
            {{ viewModel.getSelectedCount() }} selected
          </span>
          <router-link
            v-if="viewModel.getSelectedCount() === 1"
            :to="{
              name: 'operation-plan-generate',
              query: { vvnId: Array.from(viewModel.selectedVvnIds.value)[0] }
            }"
            class="btn-primary"
          >
            Generate Plan for Selected
          </router-link>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="viewModel.isLoading.value" class="loading-state">
        <div class="spinner-large"></div>
        <p>Analyzing VVNs...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="viewModel.missingCount.value === 0" class="empty-state success">
        <div class="success-icon">✅</div>
        <h3>All Clear!</h3>
        <p>All approved vessel visits have operation plans.</p>
      </div>

      <!-- Missing Plans Table -->
      <div v-else class="table-wrapper">
        <table class="missing-plans-table">
          <thead>
            <tr>
              <th class="checkbox-col">
                <input
                  type="checkbox"
                  :checked="viewModel.allSelected.value"
                  @change="viewModel.toggleSelectAll()"
                  title="Select All"
                />
              </th>
              <th>Urgency</th>
              <th>VVN ID</th>
              <th>Vessel</th>
              <th>Arrival Date</th>
              <th>Departure Date</th>
              <th>Dock</th>
              <th>Containers</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="plan in viewModel.missingPlans.value"
              :key="plan.vvnId"
              :class="['urgency-' + viewModel.getUrgency(plan.arrivalDate)]"
            >
              <td class="checkbox-col">
                <input
                  type="checkbox"
                  :checked="viewModel.isSelected(plan.vvnId)"
                  @change="viewModel.toggleSelection(plan.vvnId)"
                />
              </td>
              <td>
                <span
                  class="urgency-badge"
                  :class="'urgency-' + viewModel.getUrgency(plan.arrivalDate)"
                >
                  {{ viewModel.getUrgencyLabel(plan.arrivalDate) }}
                </span>
              </td>
              <td><strong>{{ plan.vvnId }}</strong></td>
              <td>
                <div>{{ plan.vesselName || plan.vesselId }}</div>
                <div class="vessel-id">{{ plan.vesselId }}</div>
              </td>
              <td>{{ viewModel.formatDate(plan.arrivalDate) }}</td>
              <td>{{ viewModel.formatDate(plan.departureDate) }}</td>
              <td>{{ plan.dockId || 'N/A' }}</td>
              <td class="text-center">{{ plan.totalContainers }}</td>
              <td>
                <span class="status-badge" :class="'status-' + plan.status">
                  {{ plan.status }}
                </span>
              </td>
              <td class="actions-cell">
                <router-link
                  :to="{
                    name: 'operation-plan-generate',
                    query: { vvnId: plan.vvnId }
                  }"
                  class="btn-action btn-generate"
                  title="Generate Plan"
                >
                  Generate
                </router-link>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Legend -->
      <div v-if="viewModel.missingCount.value > 0" class="legend">
        <h4>Urgency Legend:</h4>
        <div class="legend-items">
          <span class="legend-item">
            <span class="urgency-badge urgency-high">High</span>
            Arrival within 2 days
          </span>
          <span class="legend-item">
            <span class="urgency-badge urgency-medium">Medium</span>
            Arrival within 7 days
          </span>
          <span class="legend-item">
            <span class="urgency-badge urgency-low">Low</span>
            Arrival after 7 days
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { MissingOperationPlansViewModel } from '@/viewmodels/operationPlan/MissingOperationPlansViewModel';

const viewModel = new MissingOperationPlansViewModel();

onMounted(() => {
  // Set default date range (next 30 days)
  const today = new Date();
  const nextMonth = new Date(today);
  nextMonth.setDate(nextMonth.getDate() + 30);

  viewModel.startDate.value = today.toISOString().split('T')[0];
  viewModel.endDate.value = nextMonth.toISOString().split('T')[0];

  viewModel.loadMissingPlans();
});
</script>

<style scoped>
.missing-plans-container {
  max-width: 1600px;
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

.filter-card,
.results-card {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

.filter-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #2c3e50;
}

.form-group input {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-group input:focus {
  outline: none;
  border-color: #3498db;
}

.button-group {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
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

.error-message {
  padding: 1rem;
  border-radius: 4px;
  margin-top: 1rem;
  background-color: #fee;
  color: #c33;
  border: 1px solid #fcc;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.results-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.count-badge {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-weight: bold;
  margin-top: 0.5rem;
}

.count-badge.warning {
  background-color: #fff3cd;
  color: #856404;
  border: 1px solid #ffc107;
}

.count-badge.success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #28a745;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.selection-info {
  color: #7f8c8d;
  font-weight: 600;
}

.loading-state {
  text-align: center;
  padding: 3rem;
  color: #7f8c8d;
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

.empty-state {
  text-align: center;
  padding: 3rem;
}

.empty-state.success {
  color: #155724;
}

.success-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.table-wrapper {
  overflow-x: auto;
}

.missing-plans-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.missing-plans-table thead {
  background-color: #34495e;
  color: white;
}

.missing-plans-table th,
.missing-plans-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

.checkbox-col {
  width: 40px;
  text-align: center;
}

.text-center {
  text-align: center;
}

.missing-plans-table tbody tr {
  transition: background-color 0.3s;
}

.missing-plans-table tbody tr:hover {
  background-color: #f8f9fa;
}

.missing-plans-table tbody tr.urgency-high {
  border-left: 4px solid #e74c3c;
}

.missing-plans-table tbody tr.urgency-medium {
  border-left: 4px solid #f39c12;
}

.missing-plans-table tbody tr.urgency-low {
  border-left: 4px solid #3498db;
}

.urgency-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: bold;
}

.urgency-badge.urgency-high {
  background-color: #e74c3c;
  color: white;
}

.urgency-badge.urgency-medium {
  background-color: #f39c12;
  color: white;
}

.urgency-badge.urgency-low {
  background-color: #3498db;
  color: white;
}

.vessel-id {
  font-size: 0.85rem;
  color: #7f8c8d;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
}

.status-badge.status-approved {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.status-pending {
  background-color: #fff3cd;
  color: #856404;
}

.actions-cell {
  white-space: nowrap;
}

.btn-action {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  text-decoration: none;
  font-size: 0.9rem;
  transition: all 0.3s;
}

.btn-generate {
  background-color: #27ae60;
  color: white;
}

.btn-generate:hover {
  background-color: #229954;
  transform: translateY(-2px);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.legend {
  margin-top: 2rem;
  padding: 1.5rem;
  background: #f8f9fa;
  border-radius: 4px;
  border-left: 4px solid #3498db;
}

.legend h4 {
  margin-top: 0;
  margin-bottom: 1rem;
  color: #2c3e50;
}

.legend-items {
  display: flex;
  gap: 2rem;
  flex-wrap: wrap;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #7f8c8d;
}
</style>
