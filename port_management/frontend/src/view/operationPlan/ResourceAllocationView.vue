<template>
  <div class="allocation-container">
    <h1>Resource Allocation Check</h1>
    <p class="subtitle">US 4.1.6 - Monitor resource usage and workload distribution</p>

    <div class="form-card">
      <h2>Query Parameters</h2>
      <div class="form-grid">
        <div class="form-group">
          <label for="resourceId">Resource ID (Dock/Crane)</label>
          <input 
            id="resourceId" 
            v-model="viewModel.resourceId.value" 
            type="text" 
            placeholder="e.g. DOCK-01 or CRANE-A"
            :disabled="viewModel.isLoading.value"
          />
        </div>

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

      <div class="button-wrapper">
        <button 
          class="btn-primary" 
          @click="viewModel.fetchAllocation()"
          :disabled="viewModel.isLoading.value"
        >
          <span v-if="viewModel.isLoading.value" class="spinner"></span>
          {{ viewModel.isLoading.value ? 'Analyzing...' : 'Check Allocation' }}
        </button>
      </div>
    </div>

    <div v-if="viewModel.errorMessage.value" class="error-message">
      <span class="icon">⚠️</span>
      {{ viewModel.errorMessage.value }}
    </div>

    <div v-if="viewModel.result.value" class="info-card result-card">
      <h2>Allocation Results</h2>
      <div class="stats-grid">
        <div class="stat-box">
          <span class="stat-label">Resource</span>
          <span class="stat-value">{{ viewModel.result.value.resourceId }}</span>
        </div>
        <div class="stat-box">
          <span class="stat-label">Total Time Allocated</span>
          <span class="stat-value highlight">
            {{ viewModel.formatDuration(viewModel.result.value.totalAllocatedTime) }}
          </span>
        </div>
        <div class="stat-box">
          <span class="stat-label">Total Operations</span>
          <span class="stat-value">{{ viewModel.result.value.totalOperations }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ResourceAllocationViewModel } from '@/viewmodels/operationPlan/ResourceAllocationViewModel';

const viewModel = new ResourceAllocationViewModel();
</script>

<style scoped>
/* Reusing your existing styles structure */
.allocation-container { max-width: 1000px; margin: 0 auto; padding: 2rem; }
.subtitle { color: #7f8c8d; margin-bottom: 2rem; }
.form-card, .info-card { background: white; border-radius: 8px; padding: 2rem; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1); margin-bottom: 2rem; }
.form-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; margin-bottom: 1.5rem; }
.form-group label { display: block; margin-bottom: 0.5rem; font-weight: 600; color: #2c3e50; }
.form-group input { width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 4px; }
.btn-primary { background-color: #3498db; color: white; padding: 0.75rem 1.5rem; border: none; border-radius: 4px; cursor: pointer; display: flex; align-items: center; gap: 0.5rem; }
.btn-primary:disabled { opacity: 0.7; cursor: not-allowed; }
.error-message { background-color: #fee; color: #c33; padding: 1rem; border-radius: 4px; margin-bottom: 1rem; }

/* Result specific styles */
.stats-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 2rem; text-align: center; }
.stat-box { display: flex; flex-direction: column; padding: 1.5rem; background: #f8f9fa; border-radius: 8px; }
.stat-label { color: #7f8c8d; font-size: 0.9rem; margin-bottom: 0.5rem; text-transform: uppercase; letter-spacing: 1px; }
.stat-value { font-size: 1.8rem; font-weight: bold; color: #2c3e50; }
.highlight { color: #3498db; }
.spinner { width: 16px; height: 16px; border: 2px solid rgba(255,255,255,0.3); border-top-color: white; border-radius: 50%; animation: spin 0.6s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
</style>
