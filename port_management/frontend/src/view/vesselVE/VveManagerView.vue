<template>
  <div class="manager-container">
    <div class="header-section">
      <h1>VVE Manager (Standalone)</h1>
      <p class="subtitle">Emergency Interface for US 4.1.11 (Complete Vessel Visit)</p>
    </div>

    <div class="grid-layout">
      <div class="card debug-card">
        <div class="card-header warning-header">
          <h2>Step 1: Force Create VVE</h2>
        </div>
        <div class="card-body">
          <p>Create a "fake" visit execution record to test completion.</p>
          
          <div class="form-group">
            <label>Fake VVN ID</label>
            <input v-model="fakeVvnId" type="text" placeholder="e.g. VVN-TEST-01" />
          </div>

          <div class="form-group">
            <label>Arrival Date</label>
            <input v-model="fakeArrivalDate" type="datetime-local" />
          </div>

          <button class="btn btn-warning" @click="createFakeVve" :disabled="loading">
            <span v-if="loading"></span>
            {{ loading ? 'Creating...' : 'Force Create Record' }}
          </button>

          <div v-if="fakeMessage" :class="['alert', fakeSuccess ? 'alert-success' : 'alert-error']">
            {{ fakeMessage }}
          </div>
        </div>
      </div>

      <div class="card action-card">
        <div class="card-header primary-header">
          <h2>Step 2: Complete Visit</h2>
        </div>
        <div class="card-body">
          <p><strong>US 4.1.11:</strong> Record departure time and close the visit lifecycle.</p>
          
          <div class="form-group">
            <label>Target VVN ID</label>
            <input v-model="targetVvnId" type="text" placeholder="Enter VVN ID to complete" />
            <small>Must match an existing "In Progress" visit.</small>
          </div>

          <div class="form-group">
            <label>Actual Departure Time</label>
            <input v-model="departureTime" type="datetime-local" />
          </div>

          <button class="btn btn-primary" @click="completeVisit" :disabled="loading">
            <span v-if="loading"></span>
            {{ loading ? 'Processing...' : 'Complete Visit (US 11)' }}
          </button>

          <div v-if="resultMessage" :class="['alert', resultSuccess ? 'alert-success' : 'alert-error']">
            {{ resultMessage }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';

// --- State ---
const loading = ref(false);

// Step 1 Inputs
const fakeVvnId = ref('VVN-TEST-01');
const fakeArrivalDate = ref(new Date().toISOString().slice(0, 16));
const fakeMessage = ref('');
const fakeSuccess = ref(false);

// Step 2 Inputs
const targetVvnId = ref('VVN-TEST-01');
const departureTime = ref(new Date().toISOString().slice(0, 16));
const resultMessage = ref('');
const resultSuccess = ref(false);

// --- Backend URL Helper ---
const getBaseUrl = () => import.meta.env.VITE_OEM_BASE_URL; 

// --- FUNCTION 1: Create Fake Data (Bypass US 4.1.7) ---
async function createFakeVve() {
  loading.value = true;
  fakeMessage.value = '';
  
  try {
    // POST /vessel-visit-executions
    const url = `${getBaseUrl()}/vessel-visit-executions`;
    
    await axios.post(url, {
      vvnId: fakeVvnId.value,
      arrivalDate: new Date(fakeArrivalDate.value).toISOString()
    });

    fakeSuccess.value = true;
    fakeMessage.value = `Success! Created VVE record for ${fakeVvnId.value}`;
    
    // Auto-fill Step 2 for convenience
    targetVvnId.value = fakeVvnId.value; 
  } catch (e: any) {
    fakeSuccess.value = false;
    fakeMessage.value = e.response?.data?.message || e.message || "Failed to create.";
  } finally {
    loading.value = false;
  }
}

// --- FUNCTION 2: US 4.1.11 Complete Visit ---
async function completeVisit() {
  if (!targetVvnId.value) {
    resultMessage.value = "Please enter a VVN ID.";
    resultSuccess.value = false;
    return;
  }

  loading.value = true;
  resultMessage.value = '';

  try {
    // PATCH /vessel-visit-executions/:id/complete
    const url = `${getBaseUrl()}/vessel-visit-executions/${targetVvnId.value}/complete`;

    const response = await axios.patch(url, {
      departureTime: new Date(departureTime.value).toISOString()
    });

    resultSuccess.value = true;
    resultMessage.value = `US 11 SUCCESS: Visit ${response.data.vvnId} is now COMPLETED.`;
  } catch (e: any) {
    resultSuccess.value = false;
    // This will catch the "Pending Operations" business rule error
    resultMessage.value = e.response?.data?.message || e.message || "Failed to complete visit.";
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.manager-container { max-width: 1000px; margin: 0 auto; padding: 2rem; font-family: 'Segoe UI', sans-serif; }
.header-section { margin-bottom: 2rem; text-align: center; }
.subtitle { color: #64748b; }

.grid-layout { display: grid; grid-template-columns: 1fr 1fr; gap: 2rem; }
@media (max-width: 768px) { .grid-layout { grid-template-columns: 1fr; } }

.card { background: white; border-radius: 12px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); overflow: hidden; border: 1px solid #e2e8f0; }
.card-header { padding: 1.5rem; }
.card-body { padding: 1.5rem; }

.warning-header { background: #fff7ed; border-bottom: 1px solid #fed7aa; }
.warning-header h2 { color: #9a3412; margin: 0; font-size: 1.25rem; }

.primary-header { background: #eff6ff; border-bottom: 1px solid #bfdbfe; }
.primary-header h2 { color: #1e40af; margin: 0; font-size: 1.25rem; }

.form-group { margin-bottom: 1.25rem; }
.form-group label { display: block; font-weight: 600; color: #334155; margin-bottom: 0.5rem; }
.form-group input { width: 100%; padding: 0.75rem; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 1rem; box-sizing: border-box; }
.form-group small { color: #64748b; font-size: 0.875rem; }

.btn { width: 100%; padding: 0.75rem; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; transition: opacity 0.2s; display: flex; justify-content: center; gap: 0.5rem; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-warning { background-color: #f97316; color: white; }
.btn-warning:hover:not(:disabled) { background-color: #ea580c; }
.btn-primary { background-color: #2563eb; color: white; }
.btn-primary:hover:not(:disabled) { background-color: #1d4ed8; }

.alert { margin-top: 1.5rem; padding: 1rem; border-radius: 6px; font-weight: 500; }
.alert-success { background-color: #dcfce7; color: #166534; border: 1px solid #bbf7d0; }
.alert-error { background-color: #fee2e2; color: #991b1b; border: 1px solid #fecaca; }
</style>
