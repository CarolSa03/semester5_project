<template>
  <div class="page-container">
    <h1>Report Incidents</h1>
    <p class="subtitle">US 4.1.13 - Manage operational disruptions</p>

    <div v-if="viewModel.errorMessage.value" class="status-error">
      {{ viewModel.errorMessage.value }}
    </div>

    <section class="card">
      <div class="card-header"><h2>Search Scope</h2></div>
      <div class="input-group">
        <label>Vessel Visit ID</label>
        <div class="input-wrapper">
          <input
              v-model="viewModel.searchInput.value"
              class="form-input"
              placeholder="Enter VVN (e.g. VVN-2026-001)"
              @keyup.enter="viewModel.searchByVessel()"
          />
          <button class="btn-primary" @click="viewModel.searchByVessel()">Search</button>
        </div>
      </div>
    </section>

    <section class="card" v-if="viewModel.vvnId.value">
      <div class="card-header">
        <h2>Active Incidents for <span class="highlight">{{ viewModel.vvnId.value }}</span></h2>

        <button class="btn-primary" @click="viewModel.openCreateModal()">+ New Incident</button>
      </div>

      <div class="table-container">
        <div v-if="viewModel.isLoading.value" class="text-center">Loading...</div>

        <table class="modern-table" v-else-if="viewModel.activeIncidents.value.length > 0">
          <thead><tr><th>Type</th><th>Description</th><th>Severity</th><th>Status</th><th>Actions</th></tr></thead>
          <tbody>
          <tr v-for="inc in viewModel.activeIncidents.value" :key="inc.id">
            <td><strong>{{ inc.incidentTypeId }}</strong></td>
            <td>{{ inc.description }}</td>
            <td><span :class="['severity-badge', inc.severity.toLowerCase()]">{{ inc.severity }}</span></td>
            <td><span v-if="!inc.endTime" class="text-danger">Active</span><span v-else class="text-success">Resolved</span></td>
            <td><button v-if="!inc.endTime" class="btn-secondary small" @click="viewModel.resolveIncident(inc.id)">Resolve</button></td>
          </tr>
          </tbody>
        </table>

        <div v-else class="empty-state">
          No active incidents found for this visit.
        </div>
      </div>
    </section>

    <div v-if="viewModel.showCreateModal.value" class="modal-overlay">
      <div class="modal-card">
        <div class="modal-header"><h3>Report New Incident</h3><button class="close-btn" @click="viewModel.showCreateModal.value=false">✕</button></div>
        <div class="modal-body">
          <form @submit.prevent="viewModel.submitIncident()">
            <div class="input-group">
              <label>Type *</label>
              <select v-model="viewModel.incidentForm.value.typeCode" class="form-input" required>
                <option v-for="t in viewModel.availableTypes.value" :key="t.code" :value="t.code">{{ t.name }}</option>
              </select>
            </div>
            <div class="input-group">
              <label>Description *</label>
              <textarea v-model="viewModel.incidentForm.value.description" class="form-input" required></textarea>
            </div>
            <div class="input-group">
              <label>Severity</label>
              <select v-model="viewModel.incidentForm.value.severity" class="form-input">
                <option value="">Default</option>
                <option value="MINOR">Minor</option>
                <option value="MAJOR">Major</option>
                <option value="CRITICAL">Critical</option>
              </select>
            </div>
            <div class="actions-row">
              <button class="btn-primary" type="submit" :disabled="!viewModel.isFormValid.value">Submit</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ReportIncidentViewModel } from '@/viewmodels/incident/ReportIncidentViewModel';
const viewModel = new ReportIncidentViewModel();
</script>

<style scoped>
/* Estilos mantidos, adicionado highlight e status-error */
.page-container { max-width: 1200px; margin: 0 auto; padding: 2rem; font-family: 'Inter', sans-serif; color: #374151; }
.card { background: white; border-radius: 12px; padding: 1.5rem; margin-bottom: 2rem; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); border: 1px solid #e5e7eb; }
.card-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #f3f4f6; padding-bottom: 1rem; margin-bottom: 1rem; }
.form-input { width: 100%; padding: 0.75rem; border: 1px solid #d1d5db; border-radius: 0.5rem; margin-bottom: 1rem; }
.btn-primary { background: #2563eb; color: white; padding: 0.75rem 1.5rem; border-radius: 0.5rem; border: none; cursor: pointer; }
.btn-secondary { background: white; border: 1px solid #d1d5db; padding: 0.5rem 1rem; border-radius: 0.5rem; cursor: pointer; }
.modern-table { width: 100%; border-collapse: collapse; }
.modern-table th { text-align: left; padding: 0.75rem; border-bottom: 1px solid #eee; }
.modern-table td { padding: 0.75rem; border-bottom: 1px solid #f9fafb; }
.severity-badge { padding: 2px 8px; border-radius: 6px; font-weight: bold; font-size: 0.8rem; }
.severity-badge.critical { background: #fee2e2; color: #991b1b; }
.severity-badge.major { background: #ffedd5; color: #9a3412; }
.severity-badge.minor { background: #dbeafe; color: #1e40af; }
.text-danger { color: #dc2626; font-weight: bold; }
.text-success { color: #166534; font-weight: bold; }
.modal-overlay { position: fixed; top:0; left:0; right:0; bottom:0; background:rgba(0,0,0,0.5); display:flex; justify-content:center; align-items:center; z-index:50; }
.modal-card { background:white; width:500px; padding:1.5rem; border-radius:12px; }
.input-wrapper { display: flex; gap: 0.5rem; }
.highlight { color: #2563eb; font-weight: 700; }
.status-error { background: #fee2e2; color: #b91c1c; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem; }
.text-center { text-align: center; color: #666; padding: 2rem; }
.empty-state { text-align: center; color: #9ca3af; padding: 2rem; font-style: italic; }
</style>