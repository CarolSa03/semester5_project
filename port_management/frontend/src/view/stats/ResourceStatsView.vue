<template>
  <div class="page-container">
    <h1>Resource Stats</h1>
    <p class="subtitle">US 4.1.6 - Resource allocation analysis</p>

    <div class="card">
      <div class="input-grid">
        <div class="input-group">
          <label>Resource ID</label>
          <input v-model="resourceId" placeholder="e.g. CRANE-01" class="form-input">
        </div>
        <div class="input-group"><label>Start</label><input type="date" v-model="start" class="form-input"></div>
        <div class="input-group"><label>End</label><input type="date" v-model="end" class="form-input"></div>
        <button class="btn-primary" @click="loadStats">Analyze</button>
      </div>
    </div>

    <div class="card" v-if="stats">
      <h2>Results: {{ resourceId }}</h2>
      <div class="kpi-grid">
        <div class="info-box"><strong>Allocated Time:</strong> {{ stats.totalMinutes }} min</div>
        <div class="info-box"><strong>Operations:</strong> {{ stats.count }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import VesselVisitService from '@/service/VesselVisitService';

const resourceId = ref('');
const start = ref('');
const end = ref('');
const stats = ref<any>(null);

async function loadStats() {
  stats.value = await VesselVisitService.getResourceStats(resourceId.value, start.value, end.value);
}
</script>

<style scoped>
/* Reutilização dos estilos globais */
.page-container { max-width: 1000px; margin: 0 auto; padding: 2rem; font-family: 'Inter', sans-serif; }
.card { background: white; border-radius: 12px; padding: 2rem; margin-bottom: 2rem; border: 1px solid #e5e7eb; }
.input-grid { display: grid; grid-template-columns: 1fr 1fr 1fr auto; gap: 1rem; align-items: end; }
.form-input { padding: 0.75rem; border: 1px solid #d1d5db; border-radius: 0.5rem; width: 100%; }
.btn-primary { background: #2563eb; color: white; padding: 0.75rem 1.5rem; border-radius: 0.5rem; border: none; cursor: pointer; }
.kpi-grid { display: flex; gap: 2rem; margin-top: 1rem; }
.info-box { font-size: 1.2rem; }
</style>