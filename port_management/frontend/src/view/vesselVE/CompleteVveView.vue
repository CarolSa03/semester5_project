<template>
  <div class="complete-vve-container">
    <h1>Complete Vessel Visit</h1>
    <p class="subtitle">US 4.1.11 - Record departure and close visit lifecycle</p>

    <div v-if="!viewModel.successMessage.value">
      <div class="form-card">
        <div class="warning-banner">
          <span class="icon">ℹ️</span>
          This action will lock the Vessel Visit. Ensure all operations are finished.
        </div>

        <div class="form-group">
          <label>VVN ID (Visit Reference)</label>
          <input 
            type="text" 
            :value="viewModel.vvnId.value" 
            disabled 
            class="readonly-input"
          />
        </div>

        <div class="form-group">
          <label for="departureTime">Actual Departure Time *</label>
          <input 
            id="departureTime" 
            v-model="viewModel.departureTime.value" 
            type="datetime-local" 
            :disabled="viewModel.isLoading.value"
            required
          />
          <small>The exact time the vessel left port limits.</small>
        </div>

        <div v-if="viewModel.errorMessage.value" class="error-message">
          <span class="icon">⛔</span>
          <strong>Error:</strong> {{ viewModel.errorMessage.value }}
        </div>

        <div class="button-group">
          <button 
            class="btn-primary" 
            @click="handleCompletion"
            :disabled="viewModel.isLoading.value"
          >
            <span v-if="viewModel.isLoading.value" class="spinner"></span>
            {{ viewModel.isLoading.value ? 'Processing...' : 'Confirm Completion' }}
          </button>
          
          <button 
            class="btn-secondary" 
            @click="goBack"
            :disabled="viewModel.isLoading.value"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>

    <div v-else class="success-card">
      <div class="success-icon">✅</div>
      <h2>Visit Completed</h2>
      <p>{{ viewModel.successMessage.value }}</p>
      <button class="btn-primary" @click="goBack">Return to List</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { CompleteVveViewModel } from '@/viewmodels/vesselVE/CompleteVveViewModel';

const route = useRoute();
const router = useRouter();
const viewModel = new CompleteVveViewModel();

onMounted(() => {
  const vvnId = route.params.id as string;
  if (vvnId) {
    viewModel.initialize(vvnId);
  } else {
    viewModel.errorMessage.value = "No VVN ID provided.";
  }
});

async function handleCompletion() {
  const success = await viewModel.completeVisit();
  if (success) {
    // Optional: Auto redirect after a few seconds
    setTimeout(() => {
      // router.push({ name: 'vve-list' });
    }, 2000);
  }
}

function goBack() {
  router.back();
}
</script>

<style scoped>
/* Consistent Styles */
.complete-vve-container { max-width: 600px; margin: 0 auto; padding: 2rem; }
.subtitle { color: #7f8c8d; margin-bottom: 2rem; }
.form-card, .success-card { background: white; border-radius: 8px; padding: 2rem; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1); }
.form-group { margin-bottom: 1.5rem; }
.form-group label { display: block; margin-bottom: 0.5rem; font-weight: 600; color: #2c3e50; }
.form-group input { width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 4px; font-size: 1rem; }
.readonly-input { background-color: #f8f9fa; color: #666; cursor: not-allowed; }
.warning-banner { background-color: #e3f2fd; color: #0d47a1; padding: 1rem; border-radius: 4px; margin-bottom: 1.5rem; display: flex; align-items: center; gap: 0.5rem; border: 1px solid #bbdefb; }
.error-message { background-color: #fee; color: #c33; padding: 1rem; border-radius: 4px; margin-bottom: 1.5rem; border: 1px solid #fcc; }
.button-group { display: flex; gap: 1rem; margin-top: 2rem; }
.btn-primary { background-color: #3498db; color: white; padding: 0.75rem 1.5rem; border: none; border-radius: 4px; cursor: pointer; flex: 1; display: flex; justify-content: center; align-items: center; gap: 0.5rem; }
.btn-secondary { background-color: #95a5a6; color: white; padding: 0.75rem 1.5rem; border: none; border-radius: 4px; cursor: pointer; }
.spinner { width: 16px; height: 16px; border: 2px solid rgba(255,255,255,0.3); border-top-color: white; border-radius: 50%; animation: spin 0.6s linear infinite; }

/* Success State Styles */
.success-card { text-align: center; padding: 3rem 2rem; }
.success-icon { font-size: 4rem; margin-bottom: 1rem; }
.success-card h2 { color: #2ecc71; margin-bottom: 0.5rem; }
.success-card button { margin-top: 2rem; width: 100%; }

@keyframes spin { to { transform: rotate(360deg); } }
</style>
