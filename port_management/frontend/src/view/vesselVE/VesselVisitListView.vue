<template>
  <div class="page-container">
    <h1>Vessel Visits Log</h1>
    <p class="subtitle">US 4.1.10 - Execution history and status</p>

    <section class="card filter-card">
      <div class="filters-row">
        <input v-model="viewModel.startDate.value" type="date" class="form-input" placeholder="Start Date">
        <input v-model="viewModel.endDate.value" type="date" class="form-input" placeholder="End Date">
        <input v-model="viewModel.vesselId.value" type="text" class="form-input" placeholder="Vessel ID">

        <select v-model="viewModel.status.value" class="form-input">
          <option value="">All Statuses</option>
          <option value="PENDING">Pending</option>
          <option value="IN_PROGRESS">In Progress</option>
          <option value="COMPLETED">Completed</option>
        </select>

        <button @click="viewModel.loadVisits()" class="btn-primary">Search</button>
        <button @click="viewModel.resetFilters()" class="btn-secondary">Reset</button>
      </div>
    </section>

    <section class="card">
      <div class="table-container">

        <div v-if="viewModel.isLoading.value" class="loading-state">Loading visits...</div>

        <table v-else class="modern-table">
          <thead>
          <tr>
            <th>VVN ID</th>
            <th>Vessel Name</th>
            <th>Status</th>
            <th>Arrival (Actual / Est.)</th>
            <th>Departure</th>
            <th>Dock</th>
            <th>Progress</th>
            <th>Actions</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="visit in viewModel.vesselVisits.value" :key="visit.id">

            <td class="font-mono">{{ visit.vvnId }}</td>
            <td>{{ visit.vesselName || 'Unknown' }}</td>

            <td>
              <span :class="['status-badge', visit.status ? visit.status.toLowerCase() : 'pending']">
                {{ visit.status }}
              </span>
            </td>

            <td>
              <div v-if="visit.arrivalDate">
                {{ viewModel.formatDate(visit.arrivalDate) }}
                <small class="text-success">(Actual)</small>
              </div>
              <div v-else-if="visit.expectedArrival">
                {{ viewModel.formatDate(visit.expectedArrival) }}
                <small class="text-muted">(Est.)</small>
              </div>
              <div v-else>-</div>
            </td>

            <td>{{ visit.departureDate ? viewModel.formatDate(visit.departureDate) : '-' }}</td>
            <td>{{ visit.assignedDock || '-' }}</td>

            <td style="width: 150px;">
              <div v-if="visit.metrics" class="progress-container">
                <div class="progress-bar-bg">
                  <div class="progress-bar-fill"
                       :style="{ width: visit.metrics.progressPercentage + '%' }">
                  </div>
                </div>
                <small>{{ visit.metrics.progressPercentage }}%</small>
              </div>
              <span v-else>-</span>
            </td>

            <td>
              <button @click="viewModel.viewDetails(visit.id)" class="btn-icon" title="View Details">
                👁️ Manage
              </button>
            </td>
          </tr>

          <tr v-if="viewModel.vesselVisits.value.length === 0">
            <td colspan="8" class="text-center">No visits found matching filters.</td>
          </tr>
          </tbody>
        </table>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { VesselVisitListViewModel } from '@/viewmodels/vesselVE/VesselVisitListViewModel';

const viewModel = new VesselVisitListViewModel();

onMounted(() => {
  viewModel.loadVisits();
});
</script>

<style scoped>
.page-container { max-width: 1400px; margin: 0 auto; padding: 2rem; font-family: 'Inter', sans-serif; color: #374151; }
.card { background: white; border-radius: 12px; padding: 1.5rem; border: 1px solid #e5e7eb; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); margin-bottom: 2rem;}
.filter-card .filters-row { display: flex; gap: 1rem; align-items: center; flex-wrap: wrap; }
.form-input { padding: 0.6rem; border: 1px solid #d1d5db; border-radius: 0.5rem; min-width: 150px;}

.modern-table { width: 100%; border-collapse: collapse; min-width: 1000px; }
.modern-table th { text-align: left; padding: 1rem; border-bottom: 2px solid #eee; color: #6b7280; font-size: 0.9rem; text-transform: uppercase; letter-spacing: 0.05em; }
.modern-table td { padding: 1rem; border-bottom: 1px solid #f9fafb; vertical-align: middle; }
.font-mono { font-family: monospace; color: #2563eb; font-weight: 500;}

.status-badge { padding: 4px 10px; border-radius: 20px; font-weight: 600; font-size: 0.75rem; text-transform: uppercase; }
.status-badge.pending { background: #fef3c7; color: #92400e; }
.status-badge.in_progress { background: #dbeafe; color: #1e40af; }
.status-badge.completed { background: #dcfce7; color: #166534; }

.text-muted { color: #9ca3af; font-size: 0.8em; margin-left: 4px; }
.text-success { color: #166534; font-size: 0.8em; margin-left: 4px; }
.text-center { text-align: center; color: #6b7280; padding: 2rem; }

.btn-primary, .btn-secondary { padding: 0.6rem 1.2rem; border-radius: 0.5rem; cursor: pointer; border: none; font-weight: 500; }
.btn-primary { background: #2563eb; color: white; }
.btn-secondary { background: #f3f4f6; color: #374151; }
.btn-icon { background: none; border: 1px solid #e5e7eb; padding: 0.4rem 0.8rem; border-radius: 0.5rem; cursor: pointer; color: #4b5563; transition: all 0.2s; }
.btn-icon:hover { background: #f9fafb; border-color: #d1d5db; }

.progress-container { display: flex; align-items: center; gap: 8px; }
.progress-bar-bg { flex-grow: 1; background: #e5e7eb; height: 8px; border-radius: 4px; overflow: hidden; }
.progress-bar-fill { background: #3b82f6; height: 100%; transition: width 0.5s ease; }
</style>