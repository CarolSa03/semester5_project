<template>
  <div class="operation-plan-detail-container">
    <div class="header-actions">
      <button @click="goBack" class="btn-secondary">← Back to List</button>
      <div v-if="plan" class="actions">
        <button @click="goToEdit" class="btn-primary">Edit Plan</button>
      </div>
    </div>

    <div v-if="isLoading" class="loading-state">
      <div class="spinner-large"></div>
      <p>Loading plan details...</p>
    </div>

    <div v-else-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </div>

    <div v-else-if="plan" class="content">
      <div class="plan-header">
        <h1>Operation Plan Details</h1>
        <div class="meta-grid">
          <div class="meta-item">
            <span class="label">Vessel Visit (VVN):</span>
            <span class="value">{{ plan.vvnId }}</span>
          </div>
          <div class="meta-item">
            <span class="label">Date:</span>
            <span class="value">{{ formatDate(plan.date) }}</span>
          </div>
          <div class="meta-item">
            <span class="label">Algorithm:</span>
            <span class="value badge">{{ plan.metrics?.algorithm || 'N/A' }}</span>
          </div>
          <div class="meta-item">
            <span class="label">Plan ID:</span>
            <span class="value small">{{ plan.id }}</span>
          </div>
        </div>
      </div>

      <div class="card metrics-card">
        <h2>Performance Metrics</h2>
        <div class="metrics-grid">
          <div class="metric">
            <span class="metric-value" :class="{ 'text-danger': plan.metrics?.totalDelay > 0 }">
              {{ plan.metrics?.totalDelay }} <small>min</small>
            </span>
            <span class="metric-label">Total Delay</span>
          </div>
          <div class="metric">
            <span class="metric-value">{{ plan.metrics?.computationTime.toFixed(2) }} <small>s</small></span>
            <span class="metric-label">Computation Time</span>
          </div>
          <div class="metric">
            <span class="metric-value">{{ plan.schedule?.length || 0 }}</span>
            <span class="metric-label">Total Operations</span>
          </div>
        </div>
      </div>

      <div class="card schedule-card">
        <h2>Scheduled Operations</h2>
        <div class="table-wrapper">
          <table class="schedule-table">
            <thead>
            <tr>
              <th>Operation Type</th>
              <th>Assigned Resource</th>
              <th>Start Time</th>
              <th>End Time</th>
              <th>Duration</th>
              <th>Delay</th>
            </tr>
            </thead>
            <tbody>
            <tr v-for="(op, index) in plan.schedule" :key="index">
              <td>Unloading / Loading</td>
              <td>{{ op.dockId || op.resourceId }}</td>
              <td>{{ formatDateTime(op.startTime) }}</td>
              <td>{{ formatDateTime(op.endTime) }}</td>
              <td>{{ op.durationMinutes }} min</td>
              <td :class="{ 'text-danger': op.delay > 0 }">
                {{ op.delay > 0 ? `+${op.delay} min` : '-' }}
              </td>
            </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import OperationPlanService from '@/service/OperationPlanService';
// Nota: Ajuste a importação da interface conforme o seu projeto (IOperationPlan ou IOperationPlanDTO)
import { IOperationPlan } from '@/entities/OperationPlan';

const route = useRoute();
const router = useRouter();

const plan = ref<IOperationPlan | null>(null);
const isLoading = ref(true);
const errorMessage = ref('');

onMounted(async () => {
  const planId = route.params.id as string;
  if (!planId) {
    errorMessage.value = "No Plan ID provided";
    isLoading.value = false;
    return;
  }

  try {
    // O metodo getOperationPlanById deve existir no seu Service
    // Se retornar Result<T>, temos de tratar. Se retornar Promise<T>, é direto.
    // Assumindo a estrutura do Service fornecido anteriormente:
    const result = await OperationPlanService.getOperationPlanById(planId);
    plan.value = result; // Ou result.getValue() se vier num Result wrapper no frontend
  } catch (error: any) {
    errorMessage.value = error.message || "Failed to load plan details";
    console.error(error);
  } finally {
    isLoading.value = false;
  }
});

function goBack() {
  router.push({ name: 'operation-plan-list' });
}

function goToEdit() {
  if (plan.value?.id) {
    router.push({ name: 'operation-plan-edit', params: { id: plan.value.id } });
  }
}

function formatDate(dateStr: string | Date): string {
  if (!dateStr) return 'N/A';
  return new Date(dateStr).toLocaleDateString();
}

function formatDateTime(val: number | string | Date): string {
  if (!val) return 'N/A';
  // Se for number (timestamp) ou string
  return new Date(val).toLocaleString([], { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit' });
}
</script>

<style scoped>
.operation-plan-detail-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.header-actions {
  display: flex;
  justify-content: space-between;
  margin-bottom: 2rem;
}

.card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  margin-bottom: 2rem;
}

.plan-header {
  margin-bottom: 2rem;
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.meta-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-top: 1rem;
}

.meta-item {
  display: flex;
  flex-direction: column;
}

.label {
  font-size: 0.85rem;
  color: #7f8c8d;
  font-weight: 600;
  text-transform: uppercase;
}

.value {
  font-size: 1.1rem;
  color: #2c3e50;
  font-weight: 500;
}

.badge {
  display: inline-block;
  background: #3498db;
  color: white;
  padding: 0.2rem 0.6rem;
  border-radius: 4px;
  font-size: 0.9rem;
  width: fit-content;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 2rem;
  text-align: center;
}

.metric {
  display: flex;
  flex-direction: column;
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 8px;
}

.metric-value {
  font-size: 2rem;
  font-weight: bold;
  color: #2c3e50;
}

.metric-label {
  color: #7f8c8d;
}

.text-danger { color: #e74c3c; }

.schedule-table {
  width: 100%;
  border-collapse: collapse;
}

.schedule-table th, .schedule-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.schedule-table th {
  background: #f8f9fa;
  font-weight: 600;
  color: #2c3e50;
}

.btn-primary, .btn-secondary {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
}

.btn-primary { background: #3498db; color: white; }
.btn-secondary { background: #95a5a6; color: white; }
</style>