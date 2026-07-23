<template>
  <div class="page-container">

    <div v-if="viewModel.isLoading.value" class="state-box loading">
      <div class="spinner"></div>
      <p>Loading visit details...</p>
    </div>

    <div v-else-if="viewModel.errorMessage.value" class="state-box error">
      <h3>Error</h3>
      <p>{{ viewModel.errorMessage.value }}</p>
      <router-link to="/oem/vessel-visits" class="btn-secondary">Back to List</router-link>
    </div>

    <div v-else-if="viewModel.visit.value">
      <h1>Visit Manager: {{ viewModel.visit.value.vesselName || 'Unknown Vessel' }}</h1>
      <p class="subtitle">Managing execution lifecycle for VVE: {{ viewModel.visit.value.id }}</p>

      <div v-if="viewModel.message.value" class="status-success">{{ viewModel.message.value }}</div>

      <section class="card" v-if="viewModel.visit.value.status === 'PENDING'">
        <div class="card-header"><h2>1. Register Actual Arrival</h2></div>
        <div class="input-grid">
          <div class="input-group">
            <label>Arrival Time</label>
            <input type="datetime-local" v-model="viewModel.arrivalForm.value.date" class="form-input">
          </div>
          <button class="btn-primary" @click="viewModel.registerArrival()">Confirm Arrival</button>
        </div>
      </section>

      <div v-else>
        <div class="kpi-grid">
          <div class="card info-box">
            <strong>Status:</strong> {{ viewModel.visit.value.status }}
          </div>
          <div class="card info-box">
            <strong>Arrived:</strong>
            {{ viewModel.visit.value.arrivalDate ? new Date(viewModel.visit.value.arrivalDate).toLocaleString() : '-' }}
          </div>
          <div class="card info-box" v-if="viewModel.visit.value.dockId || viewModel.visit.value.assignedDock">
            <strong>Dock:</strong> {{ viewModel.visit.value.dockId || viewModel.visit.value.assignedDock }}
          </div>
        </div>

        <section class="card" v-if="viewModel.visit.value.status === 'IN_PROGRESS' && !viewModel.visit.value.dockId">
          <div class="card-header"><h2>2. Register Berthing</h2></div>
          <div class="input-grid">
            <div class="input-group"><label>Time</label><input type="datetime-local" v-model="viewModel.berthForm.value.date" class="form-input"></div>
            <div class="input-group"><label>Dock ID</label><input type="text" v-model="viewModel.berthForm.value.dockId" class="form-input"></div>
            <button class="btn-primary" @click="viewModel.registerBerth()">Confirm</button>
          </div>
        </section>

        <section class="card" v-if="viewModel.visit.value.status === 'IN_PROGRESS'">
          <div class="card-header"><h2>Non-Cargo Tasks</h2></div>
          <div class="input-grid">
            <input v-model="viewModel.taskForm.value.category" placeholder="Category Code" class="form-input">
            <input v-model="viewModel.taskForm.value.description" placeholder="Desc" class="form-input">
            <input type="datetime-local" v-model="viewModel.taskForm.value.start" class="form-input">
            <input type="datetime-local" v-model="viewModel.taskForm.value.end" class="form-input">
            <button class="btn-primary" @click="viewModel.logTask()">Add Task</button>
          </div>
          <table class="modern-table">
            <thead><tr><th>Cat</th><th>Desc</th><th>Time</th></tr></thead>
            <tbody>
            <tr v-for="t in viewModel.tasks.value" :key="t.id">
              <td><span class="algo-badge">{{ t.categoryCode }}</span></td>
              <td>{{ t.description }}</td>
              <td>{{ new Date(t.startTime).toLocaleString() }}</td>
            </tr>
            <tr v-if="viewModel.tasks.value.length === 0">
              <td colspan="3" style="text-align:center; color:#999;">No tasks registered.</td>
            </tr>
            </tbody>
          </table>
        </section>

        <section class="card" v-if="viewModel.visit.value.status === 'IN_PROGRESS'">
          <div class="card-header"><h2>Close Visit</h2></div>
          <div class="input-grid">
            <div class="input-group"><label>Unberth</label><input type="datetime-local" v-model="viewModel.departForm.value.unberthDate" class="form-input"></div>
            <div class="input-group"><label>Departure</label><input type="datetime-local" v-model="viewModel.departForm.value.departDate" class="form-input"></div>
            <button class="btn-primary bg-green" @click="viewModel.registerDeparture()">Complete Visit</button>
          </div>
        </section>

        <section class="card" v-if="viewModel.visit.value.status === 'COMPLETED'">
          <h2>✅ Visit Completed</h2>
          <p>This vessel visit is closed.</p>
          <p><strong>Departure:</strong> {{ viewModel.visit.value.departureDate ? new Date(viewModel.visit.value.departureDate).toLocaleString() : '-' }}</p>
          <router-link to="/oem/vessel-visits" class="btn-secondary">Back to List</router-link>
        </section>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { VesselVisitManagerViewModel } from '@/viewmodels/vesselVE/VesselVisitManagerViewModel';

const route = useRoute();
const viewModel = new VesselVisitManagerViewModel();

onMounted(() => {
  // Garante que passamos o ID da rota
  if (route.params.id) {
    viewModel.loadData(route.params.id as string);
  } else {
    viewModel.errorMessage.value = "No ID provided in URL";
  }
});
</script>

<style scoped>
/* Layout */
.page-container { max-width: 1200px; margin: 0 auto; padding: 2rem; font-family: 'Inter', sans-serif; color: #374151; }
.card { background: white; border-radius: 12px; padding: 1.5rem; border: 1px solid #e5e7eb; margin-bottom: 2rem; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
.card-header { border-bottom: 1px solid #f3f4f6; padding-bottom: 1rem; margin-bottom: 1rem; }
.input-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; align-items: end; margin-bottom: 1rem; }
.kpi-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; margin-bottom: 2rem; }

/* Componentes */
.info-box { padding: 1rem; background: #f9fafb; border: 1px solid #e5e7eb; text-align: center; }
.form-input { width: 100%; padding: 0.75rem; border: 1px solid #d1d5db; border-radius: 0.5rem; }
.btn-primary { background: #2563eb; color: white; padding: 0.75rem 1.5rem; border-radius: 0.5rem; border: none; cursor: pointer; font-weight: 500;}
.btn-secondary { background: #6b7280; color: white; padding: 0.75rem 1.5rem; border-radius: 0.5rem; text-decoration: none; display: inline-block; cursor: pointer;}
.bg-green { background: #059669; }

/* Feedback */
.status-success { background: #dcfce7; color: #15803d; padding: 1rem; border-radius: 0.5rem; margin-bottom: 1rem; border: 1px solid #bbf7d0; }
.state-box { padding: 3rem; text-align: center; background: #f9fafb; border-radius: 12px; margin-top: 2rem; }
.state-box.error { color: #dc2626; border: 1px solid #fecaca; background: #fef2f2; }

/* Tabela */
.modern-table { width: 100%; border-collapse: collapse; margin-top: 1rem; }
.modern-table td, .modern-table th { padding: 0.75rem; border-bottom: 1px solid #f3f4f6; text-align: left; }
.algo-badge { background: #eff6ff; color: #1d4ed8; padding: 2px 8px; border-radius: 6px; font-weight: bold; font-size: 0.8rem; }

/* Spinner */
.spinner { border: 4px solid #f3f3f3; border-top: 4px solid #2563eb; border-radius: 50%; width: 40px; height: 40px; animation: spin 1s linear infinite; margin: 0 auto 1rem; }
@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
</style>