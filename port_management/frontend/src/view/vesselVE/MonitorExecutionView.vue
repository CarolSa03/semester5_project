<template>
  <div class="page-container">
    <h1>Monitor Execution</h1>
    <p class="subtitle">US 4.1.9 - Update VVE with actual executed operations</p>

    <section class="card">
      <div class="card-header"><h2>Select Vessel Visit</h2></div>
      <div class="input-grid">
        <div class="input-group">
          <label>Vessel Visit ID (VVN)</label>
          <div class="input-wrapper">
            <input v-model="viewModel.vvnId.value" class="form-input" placeholder="e.g. VVN-2026-001" @keyup.enter="viewModel.loadExecutionData()" />
            <button class="btn-primary" @click="viewModel.loadExecutionData()" :disabled="viewModel.isLoading.value">
              <span v-if="viewModel.isLoading.value">Loading...</span>
              <span v-else>🔍 Load</span>
            </button>
          </div>
        </div>
      </div>
      <div v-if="viewModel.errorMessage.value" class="status-error">{{ viewModel.errorMessage.value }}</div>
    </section>

    <section class="card" v-if="viewModel.operations.value.length > 0">
      <div class="card-header">
        <h2>Operations Progress</h2>
      </div>
      <div class="table-container">
        <table class="modern-table">
          <thead>
          <tr>
            <th>Op ID</th>
            <th>Type</th>
            <th>Planned Resource</th>
            <th>Schedule</th>
            <th>Status</th>
            <th>Action</th>
          </tr>
          </thead>
          <tbody>
          <tr v-for="op in viewModel.operations.value" :key="op.operationId">
            <td class="font-mono">{{ op.operationId }}</td>
            <td>{{ op.type }}</td>
            <td><span class="resource-badge">{{ op.resourceId }}</span></td>
            <td>
              <div class="date-cell">
                {{ viewModel.formatDate(op.startTime) }}
                <span class="text-secondary">to {{ viewModel.formatDate(op.endTime) }}</span>
              </div>
            </td>
            <td>
                <span :class="['status-badge', op.isExecuted ? 'success' : 'warning']">
                  {{ op.isExecuted ? 'EXECUTED' : 'PENDING' }}
                </span>
            </td>
            <td class="actions-cell">
              <button v-if="!op.isExecuted" class="btn-secondary small" @click="viewModel.openRegisterModal(op)">
                ⚡ Register
              </button>
              <span v-else class="text-success">✅ Done</span>
            </td>
          </tr>
          </tbody>
        </table>
      </div>
    </section>

    <div v-if="viewModel.showRegisterModal.value" class="modal-overlay">
      <div class="modal-card">
        <div class="modal-header">
          <h3>Register Actual Execution</h3>
          <button class="close-btn" @click="viewModel.showRegisterModal.value = false">✕</button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="viewModel.confirmExecution()">
            <div class="input-group">
              <label>Real Resource Used *</label>
              <input v-model="viewModel.executionForm.value.realResource" class="form-input" required />
            </div>
            <div class="input-grid">
              <div class="input-group">
                <label>Actual Start *</label>
                <input type="datetime-local" v-model="viewModel.executionForm.value.realStartTime" class="form-input" required />
              </div>
              <div class="input-group">
                <label>Actual End *</label>
                <input type="datetime-local" v-model="viewModel.executionForm.value.realEndTime" class="form-input" required />
              </div>
            </div>
            <div class="input-group">
              <label>Outcome</label>
              <select v-model="viewModel.executionForm.value.status" class="form-input">
                <option value="COMPLETED">Completed Successfully</option>
                <option value="DELAYED">Completed with Delay</option>
              </select>
            </div>
            <div class="actions-row">
              <button type="button" class="btn-secondary" @click="viewModel.showRegisterModal.value = false" style="margin-right: 1rem;">Cancel</button>
              <button type="submit" class="btn-primary" :disabled="viewModel.isSubmitting.value">Confirm</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { MonitorExecutionViewModel } from '@/viewmodels/vesselVE/MonitorExecutionViewModel';
const viewModel = new MonitorExecutionViewModel();
</script>

<style scoped>
/* Copiar estilos globais (card, btn-primary, modern-table) do GenerateOperationPlanView.vue */
.page-container { max-width: 1200px; margin: 0 auto; padding: 2rem; font-family: 'Inter', sans-serif; color: #374151; }
.card { background: white; border-radius: 12px; padding: 1.5rem; border: 1px solid #e5e7eb; margin-bottom: 2rem; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
.card-header { border-bottom: 1px solid #f3f4f6; padding-bottom: 1rem; margin-bottom: 1rem; display: flex; justify-content: space-between; }
.input-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 1rem; }
.form-input { width: 100%; padding: 0.75rem; border: 1px solid #d1d5db; border-radius: 0.5rem; }
.btn-primary { background: #2563eb; color: white; padding: 0.75rem 1.5rem; border-radius: 0.5rem; border: none; cursor: pointer; }
.btn-secondary { background: white; border: 1px solid #d1d5db; padding: 0.75rem 1.5rem; border-radius: 0.5rem; cursor: pointer; }
.modern-table { width: 100%; border-collapse: collapse; }
.modern-table th { text-align: left; padding: 0.75rem; border-bottom: 1px solid #eee; color: #6b7280; }
.modern-table td { padding: 0.75rem; border-bottom: 1px solid #f9fafb; }
.status-badge { padding: 2px 8px; border-radius: 6px; font-size: 0.8rem; font-weight: bold; }
.status-badge.success { background: #dcfce7; color: #166534; }
.status-badge.warning { background: #fff7ed; color: #9a3412; }
.modal-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 50; }
.modal-card { background: white; width: 500px; padding: 1.5rem; border-radius: 12px; }
.input-wrapper { display: flex; gap: 0.5rem; }
.status-error { background: #fee2e2; color: #b91c1c; padding: 1rem; border-radius: 0.5rem; margin-top: 1rem; }
</style>