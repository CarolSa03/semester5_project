<template>
  <div class="generate-operation-plan-container">
    <h1>Generate Operation Plan</h1>
    <p class="subtitle">US 4.1.2 - Generate and store operation plans for vessel visits</p>

    <div class="form-card">
      <h2>Plan Generation Parameters</h2>

      <form @submit.prevent="viewModel.generatePlan()">
        <div class="form-group">
          <label for="vvnId">Vessel Visit Notification ID *</label>
          <input
            id="vvnId"
            v-model="viewModel.vvnId.value"
            type="text"
            placeholder="e.g., 2026-LEIXOES-000001"
            :disabled="viewModel.isLoading.value"
            required
          />
        </div>

        <div class="form-group">
          <label for="date">Target Date *</label>
          <input
            id="date"
            v-model="viewModel.date.value"
            type="date"
            :disabled="viewModel.isLoading.value"
            required
          />
        </div>

        <div class="form-group">
          <label for="algorithm">Scheduling Algorithm *</label>
          <select
            id="algorithm"
            v-model="viewModel.algorithm.value"
            :disabled="viewModel.isLoading.value"
            required
          >
            <option
              v-for="algo in viewModel.availableAlgorithms"
              :key="algo.value"
              :value="algo.value"
            >
              {{ algo.label }}
            </option>
          </select>
        </div>

        <div v-if="viewModel.errorMessage.value" class="error-message">
          <span class="icon">Error: </span>
          {{ viewModel.errorMessage.value }}
        </div>

        <div v-if="viewModel.successMessage.value" class="success-message">
          <span class="icon">Success: </span>
          {{ viewModel.successMessage.value }}
        </div>

        <div class="button-group">
          <button
            type="submit"
            class="btn-primary"
            :disabled="!viewModel.isFormValid.value || viewModel.isLoading.value"
          >
            <span v-if="viewModel.isLoading.value" class="spinner"></span>
            {{ viewModel.isLoading.value ? 'Generating...' : 'Generate Plan' }}
          </button>

          <button
            type="button"
            class="btn-secondary"
            @click="viewModel.resetForm()"
            :disabled="viewModel.isLoading.value"
          >
            Reset
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { GenerateOperationPlanViewModel } from '@/viewmodels/operationPlan/GenerateOperationPlanViewModel';
import OperationPlanTable from '@/components/OperationPlanTable.vue';

const viewModel = new GenerateOperationPlanViewModel();

onMounted(() => {
  const today = new Date().toISOString().split('T')[0];
  viewModel.date.value = today;
});
</script>

<style scoped>
.generate-operation-plan-container {
  max-width: 900px;
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

.form-card {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
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
.form-group select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #3498db;
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
}

.btn-primary {
  background-color: #3498db;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #2980b9;
}

.btn-secondary {
  background-color: #95a5a6;
  color: white;
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  max-width: 90vw;
  max-height: 90vh;
  overflow: auto;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #eee;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #95a5a6;
}

.close-btn:hover {
  color: #2c3e50;
}

.modal-body {
  padding: 1.5rem;
}

.metadata-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.metadata-item {
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 4px;
}

.warnings-section {
  background: #fff3cd;
  border: 1px solid #ffc107;
  border-radius: 4px;
  padding: 1rem;
  margin-bottom: 2rem;
}

.warnings-section h3 {
  margin-top: 0;
  color: #856404;
}

.warnings-section ul {
  margin: 0.5rem 0 0 1.5rem;
  color: #856404;
}

.schedule-section h3 {
  margin-bottom: 1rem;
}

.modal-footer {
  padding: 1.5rem;
  border-top: 1px solid #eee;
  display: flex;
  justify-content: flex-end;
}
</style>
