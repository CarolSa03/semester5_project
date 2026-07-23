<template>
  <div class="operation-plan-list-container">
    <h1>Operation Plans</h1>
    <p class="subtitle">US 4.1.3 - Search and list operation plans for a given day or period</p>

    <div class="filter-card">
      <h2>Search Filters</h2>

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

        <div class="form-group">
          <label for="vesselId">Vessel ID</label>
          <input
              id="vesselId"
              v-model="viewModel.vesselId.value"
              type="text"
              placeholder="Search by vessel..."
              :disabled="viewModel.isLoading.value"
          />
        </div>
      </div>

      <div class="button-group">
        <button
            class="btn-primary"
            @click="viewModel.searchPlans()"
            :disabled="viewModel.isLoading.value"
        >
          <span v-if="viewModel.isLoading.value" class="spinner"></span>
          {{ viewModel.isLoading.value ? 'Searching...' : 'Search' }}
        </button>

        <button
            class="btn-secondary"
            @click="viewModel.resetFilters()"
            :disabled="viewModel.isLoading.value"
        >
          Reset
        </button>
      </div>

      <div v-if="viewModel.errorMessage.value" class="error-message">
        <span class="icon">Error: </span>
        {{ viewModel.errorMessage.value }}
      </div>
    </div>

    <div class="results-card">
      <div class="results-header">
        <h2>Operation Plans ({{ viewModel.sortedPlans.value.length }})</h2>
        <router-link to="generate" class="btn-primary">
          + Generate New Plan
        </router-link>
      </div>

      <div v-if="viewModel.isLoading.value" key="loading" class="loading-state">
        <div class="spinner-large"></div>
        <p>Loading operation plans...</p>
      </div>

      <div v-else-if="viewModel.sortedPlans.value.length === 0" key="empty" class="empty-state">
        <p>No operation plans found.</p>
        <p class="hint">Try adjusting your search filters or generate a new plan.</p>
      </div>

      <div v-else key="results" class="table-wrapper">
        <table class="plans-table">
          <thead>
          <tr>
            <th @click="viewModel.changeSortBy('startTime')" class="sortable">
              Date
              <span class="sort-indicator" v-if="viewModel.sortBy.value === 'startTime'">
                  {{ viewModel.sortDirection.value === 'asc' ? '▲' : '▼' }}
                </span>
            </th>
            <th @click="viewModel.changeSortBy('vesselName')" class="sortable">
              VVN ID
              <span class="sort-indicator" v-if="viewModel.sortBy.value === 'vesselName'">
                  {{ viewModel.sortDirection.value === 'asc' ? '▲' : '▼' }}
                </span>
            </th>
            <th>Algorithm</th>
            <th @click="viewModel.changeSortBy('delay')" class="sortable">
              Total Delay (min)
              <span class="sort-indicator" v-if="viewModel.sortBy.value === 'delay'">
                  {{ viewModel.sortDirection.value === 'asc' ? '▲' : '▼' }}
                </span>
            </th>
            <th>Computation Time (s)</th>
            <th>Summary</th>
            <th>Actions</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="(plan, index) in viewModel.sortedPlans.value" :key="plan.id || index">
            <td>{{ viewModel.formatDate(plan.date) }}</td>
            <td><strong>{{ plan.vvnId || 'Unknown' }}</strong></td>
            <td>
              <span class="badge" v-if="plan.metrics">{{ plan.metrics.algorithm }}</span>
              <span v-else class="badge warning">N/A</span>
            </td>
            <td :class="{ 'delay-warning': plan.metrics && plan.metrics.totalDelay > 0 }">
              {{ plan.metrics ? plan.metrics.totalDelay : 0 }}
            </td>
            <td>{{ plan.metrics ? plan.metrics.computationTime.toFixed(2) : '0.00' }}</td>
            <td class="summary-cell">{{ viewModel.getPlanSummary(plan) }}</td>
            <td class="actions-cell">
              <router-link
                  v-if="plan.id"
                  :to="{ name: 'operation-plan-detail', params: { id: plan.id } }"
                  class="btn-action"
                  title="View Details"
              >
                👁️
              </router-link>
              <router-link
                  v-if="plan.id"
                  :to="{ name: 'operation-plan-edit', params: { id: plan.id } }"
                  class="btn-action"
                  title="Edit Plan"
              >
                ✏️
              </router-link>
              <span v-else style="color:red; font-size: 0.8em;">Inv. ID</span>
            </td>
          </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { OperationPlanListViewModel } from '@/viewmodels/operationPlan/OperationPlanListViewModel';

const viewModel = new OperationPlanListViewModel();

onMounted(() => {
  viewModel.loadAllPlans();
});
</script>

<style scoped>
.operation-plan-list-container {
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
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
  color: white;
}

.btn-primary { background-color: #3498db; }
.btn-primary:hover:not(:disabled) { background-color: #2980b9; }
.btn-secondary { background-color: #95a5a6; }
.btn-secondary:hover:not(:disabled) { background-color: #7f8c8d; }
button:disabled { opacity: 0.6; cursor: not-allowed; }

.error-message {
  padding: 1rem;
  background-color: #fee;
  color: #c33;
  border: 1px solid #fcc;
  margin-top: 1rem;
  border-radius: 4px;
}

.results-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.loading-state, .empty-state {
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

@keyframes spin { to { transform: rotate(360deg); } }

.table-wrapper { overflow-x: auto; }

.plans-table {
  width: 100%;
  border-collapse: collapse;
}

.plans-table th, .plans-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

.plans-table thead {
  background-color: #34495e;
  color: white;
}

.plans-table th.sortable { cursor: pointer; }

.badge {
  padding: 0.25rem 0.75rem;
  background-color: #3498db;
  color: white;
  border-radius: 12px;
  font-size: 0.85rem;
}

.delay-warning { color: #e74c3c; font-weight: bold; }

.btn-action {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1.2rem;
  text-decoration: none;
  min-width: 32px;
  text-align: center;
  display: inline-block;
  margin-right: 5px;
}
.btn-action:hover { background-color: #ecf0f1; border-color: #3498db; }
</style>