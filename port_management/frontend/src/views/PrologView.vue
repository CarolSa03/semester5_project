<template>
  <div class="prolog-page">
    <h1>Vessel Scheduling & Optimization</h1>
    <p class="subtitle">
      Sprint C - US 4.3 (Genetic Algorithms, Auto-Selection & Load Balancing)
    </p>

    <div class="mock-toggle-section">
      <label class="toggle-label">
        <input type="checkbox" v-model="useMockData" />
        <span class="toggle-text">
          {{ useMockData ? 'Using Mock Data' : 'Using Real Backend' }}
        </span>
      </label>
      <div v-if="useMockData" class="toggle-hint">
        Running in simulation mode. No backend connection required.
      </div>
    </div>

    <div v-if="!useMockData">
      <div v-if="healthStatus" class="health-status">
        <span class="status-dot online"></span>
        Backend Online: {{ healthStatus.version }} ({{ healthStatus.sprint }})
      </div>
      <div v-else-if="!loading && !healthStatus" class="health-status">
        <span class="status-dot offline"></span> Backend Offline (Check console)
      </div>
    </div>

    <section class="card">
      <div class="card-header">
        <h2>1. Operation Settings</h2>
      </div>

      <div class="input-grid">
        <div class="input-group">
          <label>Target Date</label>
          <input
              v-model="targetDate"
              type="date"
              class="date-input"
              @change="onDateChange"
          />
        </div>

        <div class="input-group">
          <div class="label-row">
            <label>Algorithm Strategy</label>
            <label class="manual-override" title="Force specific algorithm for demo">
              <input type="checkbox" v-model="manualOverride">
              <span class="override-text">Manual Override</span>
            </label>
          </div>

          <div v-if="!manualOverride" class="strategy-display-readonly">
            <span class="auto-icon">🤖</span>
            <div class="text-col">
              <strong>Auto-Select (Active)</strong>
              <span class="sub-text">System optimizes based on input size</span>
            </div>
          </div>

          <select v-else v-model="selectedStrategy" class="strategy-select">
            <option value="auto">🤖 Auto-Select (Smart)</option>
            <option value="genetic">🧬 Genetic Algorithm (US 4.3.1)</option>
            <option value="optimal">⚡ Optimal (Brute Force)</option>
            <option value="greedy">🚀 Greedy Heuristic</option>
          </select>
        </div>
      </div>

      <div class="scenarios-container" v-if="useMockData">
        <label>Load Test Scenario (Threshold Triggers):</label>
        <div class="scenario-buttons">

          <button
              @click="loadScenario('small')"
              class="btn-scenario small"
              :disabled="loadingData"
              title="Loads 5 vessels (Trigger Brute Force)"
          >
            🟢 Small (N=5)
            <span class="algo-hint">Expect: Optimal</span>
          </button>

          <button
              @click="loadScenario('medium')"
              class="btn-scenario medium"
              :disabled="loadingData"
              title="Loads 20 vessels (Trigger Genetic)"
          >
            🟡 Medium (N=20)
            <span class="algo-hint">Expect: Genetic</span>
          </button>

          <button
              @click="loadScenario('large')"
              class="btn-scenario large"
              :disabled="loadingData"
              title="Loads 60 vessels (Trigger Greedy)"
          >
            🔴 Large (N=60)
            <span class="algo-hint">Expect: Greedy</span>
          </button>
        </div>
      </div>

      <div v-if="scheduleData" class="data-summary">
        Loaded: <strong>{{ scheduleData.vessels.length }} vessels</strong>
        ({{ scheduleData.scenarioLabel }})
      </div>
    </section>

    <section class="card rebalance-section">
      <div class="card-header">
        <h2>2. Dock Load Balancing</h2>
        <p class="section-desc">Distribute vessels across docks to minimize congestion before scheduling.</p>
      </div>

      <button @click="runRebalance" class="btn-secondary" :disabled="loading || !scheduleData">
        ⚖️ Optimize Dock Distribution
      </button>

      <div v-if="rebalanceResult" class="rebalance-results">
        <div v-for="alloc in rebalanceResult.allocations" :key="alloc.dockId" class="dock-card">
          <div class="dock-header">
            <h4>Dock {{ alloc.dockId }}</h4>
            <span class="load-badge">{{ alloc.totalLoadMinutes }} min</span>
          </div>
          <ul class="vessel-list">
            <li v-for="v in alloc.assignedVessels" :key="v">🚢 {{ v }}</li>
          </ul>
        </div>
      </div>
    </section>

    <section class="card">
      <div class="card-header">
        <h2>3. Schedule Generation</h2>
      </div>

      <div class="controls">
        <button @click="runSchedule" class="btn-primary" :disabled="loading || !scheduleData">
          {{ loading ? 'Processing...' : '▶ Generate Schedule' }}
        </button>
      </div>

      <div v-if="error" class="status-error">
        <strong>Error:</strong> {{ error }}
      </div>

      <div v-if="scheduleResult" class="results-area">
        <div class="result-header">
          <h3>Plan Generated Successfully</h3>
          <div class="metrics-tags">
            <span class="tag algorithm">Algorithm: {{ scheduleResult.algorithm }}</span>

            <span class="tag delay" :class="{ 'high-delay': scheduleResult.totalDelay > 0 }">
              Total Delay: {{ (Number(scheduleResult.totalDelay) / 60).toFixed(2) }}h
            </span>

            <span class="tag time">Time: {{ formatTime(scheduleResult.performanceMetrics?.computationTimeSeconds) }}s</span>
          </div>

          <div v-if="scheduleResult.performanceMetrics" class="tech-metrics">
            <small>Processing Mode: {{ scheduleResult.mode || 'Single' }} | Vessels: {{ scheduleResult.performanceMetrics.problemSize.vessels }}</small>
          </div>
        </div>

        <ScheduleTimeline
            v-if="scheduleResult.schedule"
            :schedule="scheduleResult.schedule"
        />

        <ScheduleTable
            v-if="scheduleResult.schedule"
            :schedule="scheduleResult.schedule"
        />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { usePrologApi } from '@/services/usePrologApi'
import ScheduleTable from '@/components/scheduling/ScheduleTable.vue'
import ScheduleTimeline from '@/components/scheduling/ScheduleTimeline.vue'

// -- STATE --
const useMockData = ref(false)
const targetDate = ref(new Date().toISOString().split('T')[0])
const loadingData = ref(false)
const scheduleData = ref<any>(null)
const healthStatus = ref<any>(null)

// US 4.3 State
const manualOverride = ref(false) // Começa falso (Auto por defeito)
const selectedStrategy = ref('auto')
const scheduleResult = ref<any>(null)
const rebalanceResult = ref<any>(null)

// API Hooks
const { loading, error, getHealth, schedule, rebalanceDocks } = usePrologApi()

// -- LIFECYCLE --
onMounted(async () => {
  // Check backend only if not in mock mode
  if (!useMockData.value) {
    try {
      healthStatus.value = await getHealth()
    } catch (e) {
      console.warn("Backend offline or unreachable")
    }
  }
  // Load default scenario (Medium)
  loadScenario('medium')
})

// -- DATA GENERATION LOGIC --

const onDateChange = () => {
  // Reload current scenario logic if needed
}

const generateVessels = (count: number) => {
  const vessels = []
  for (let i = 1; i <= count; i++) {
    const id = `V${i.toString().padStart(3, '0')}`
    const cargo = 50 + Math.floor(Math.random() * 400) // 50-450 containers
    const eta = Math.floor(Math.random() * 200)        // Chegada 0-200 min

    // 1. Calcula a duração estimada no backend (fator 4.8 como vimos antes)
    const backendDuration = Math.ceil(cargo * 4.8)

    // 2. Fator de Congestionamento:
    // Se temos muitos navios, os últimos vão demorar muito a ser atendidos.
    // Adicionamos (i * 30) minutos extra ao prazo para cada navio na fila.
    const congestionSlack = count * 30

    // 3. Define o ETD com margem suficiente
    const etd = eta + backendDuration + congestionSlack + Math.floor(Math.random() * 100)

    vessels.push({
      imo: id,
      eta: eta,
      etd: etd,
      cargo: cargo
    })
  }
  return vessels.sort((a, b) => a.eta - b.eta)
}

const loadScenario = (scenario: 'small' | 'medium' | 'large') => {
  loadingData.value = true

  let numVessels = 5
  let label = ""

  // Configurar Thresholds para corresponder ao Backend
  if (scenario === 'small') {
    numVessels = 5  // N <= 8 -> Optimal
    label = "Small Dataset (Triggers Optimal)"
  } else if (scenario === 'medium') {
    numVessels = 20 // 8 < N <= 50 -> Genetic
    label = "Medium Dataset (Triggers Genetic)"
  } else {
    numVessels = 60 // N > 50 -> Greedy
    label = "Large Dataset (Triggers Greedy)"
  }

  scheduleData.value = {
    requestId: `req-${scenario}-${Date.now()}`,
    scenarioLabel: label,
    vessels: generateVessels(numVessels),

    // Docas e Recursos (Fixos)
    docks: [
      { id: "D1", cranes: 2 },
      { id: "D2", cranes: 1 },
      { id: "D3", cranes: 1 }
    ],
    resources: [
      { code: "C1", status: "available" }, { code: "C2", status: "available" },
      { code: "C3", status: "available" }, { code: "C4", status: "available" }
    ],
    staff: [
      { id: "S1", role: "operator" }, { id: "S2", role: "operator" },
      { id: "S3", role: "operator" }
    ],
    storage_areas: [{ code: "Y1", capacity: 5000 }]
  }

  loadingData.value = false
  scheduleResult.value = null
  rebalanceResult.value = null
}

// ---------------------------------------------------------
// US 4.3.3: REBALANCE EXECUTION
// ---------------------------------------------------------
const runRebalance = async () => {
  if (!scheduleData.value) return
  rebalanceResult.value = null

  // MOCK SIMULATION (Opcional: Podes manter para Rebalance ou ligar ao Prolog também)
  if (useMockData.value) {
    loading.value = true
    setTimeout(() => {
      // Distribuir navios em 3 listas fictícias
      const allV = scheduleData.value.vessels.map((v:any) => v.imo)
      const chunk = Math.ceil(allV.length / 3)

      rebalanceResult.value = {
        allocations: [
          { dockId: "D1", totalLoadMinutes: 420, assignedVessels: allV.slice(0, chunk) },
          { dockId: "D2", totalLoadMinutes: 390, assignedVessels: allV.slice(chunk, chunk*2) },
          { dockId: "D3", totalLoadMinutes: 400, assignedVessels: allV.slice(chunk*2) }
        ]
      }
      loading.value = false
    }, 600)
    return
  }

  // REAL API CALL
  try {
    const res = await rebalanceDocks(scheduleData.value)
    rebalanceResult.value = res
  } catch (err) {
    console.error("Rebalance error", err)
  }
}

// ---------------------------------------------------------
// US 4.3.2 & 4.3.1: SCHEDULE EXECUTION
// ---------------------------------------------------------
const runSchedule = async () => {
  if (!scheduleData.value) return
  scheduleResult.value = null
  error.value = null

  // 1. Preparar os navios base
  let vesselsToSend = [...scheduleData.value.vessels]

  // 2. Se houver um Balanceamento feito (Rebalance Result),
  // vamos forçar essa atribuição nos dados que enviamos para o Prolog.
  if (rebalanceResult.value && rebalanceResult.value.allocations) {
    // Cria um mapa rápido: { "V001": "D3", "V002": "D1", ... }
    const vesselMap: Record<string, string> = {}

    rebalanceResult.value.allocations.forEach((alloc: any) => {
      alloc.assignedVessels.forEach((vId: string) => {
        vesselMap[vId] = alloc.dockId
      })
    })

    // Injeta o "preAssignedDock" em cada navio
    vesselsToSend = vesselsToSend.map(v => ({
      ...v,
      preAssignedDock: vesselMap[v.imo] || null // O Prolog vai ler isto!
    }))

    console.log("Sending vessels with forced dock assignments:", vesselsToSend)
  }

  const strategyToSend = manualOverride.value ? selectedStrategy.value : 'auto'

  const payload = {
    ...scheduleData.value,
    vessels: vesselsToSend, // Usa a lista modificada com as docas
    strategy: strategyToSend,
    population: 40,
    generations: 50,
    mutationRate: 0.15
  }

  try {
    const res = await schedule(payload)
    scheduleResult.value = res
  } catch (err) {
    console.error("Execution failed", err)
  }
}

// Helpers
const formatTime = (seconds: number) => {
  return seconds ? seconds.toFixed(3) : '0.00'
}

const formatMin = (m: number) => {
  const h = Math.floor(m / 60)
  const min = m % 60
  return `${h.toString().padStart(2, '0')}:${min.toString().padStart(2, '0')}`
}
</script>

<style scoped>
.prolog-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  font-family: 'Inter', sans-serif;
  color: #374151;
}

h1 { font-size: 2rem; color: #111827; margin-bottom: 0.5rem; }
.subtitle { color: #6b7280; margin-bottom: 2rem; }

/* Mock Toggle */
.mock-toggle-section {
  margin-bottom: 1rem;
  padding: 1rem;
  background: #f3f4f6;
  border-radius: 8px;
  border: 1px dashed #9ca3af;
}
.toggle-label { display: flex; align-items: center; gap: 0.5rem; cursor: pointer; font-weight: bold;}
.toggle-hint { font-size: 0.85rem; color: #6b7280; margin-top: 0.5rem; margin-left: 1.5rem; }

.card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  margin-bottom: 2rem;
  border: 1px solid #e5e7eb;
}

.card-header h2 { margin: 0; font-size: 1.25rem; color: #1f2937; }
.section-desc { font-size: 0.9rem; color: #6b7280; margin-top: 0.25rem;}
.data-summary { font-size: 0.85rem; color: #059669; margin-top: 1rem; font-weight: 500;}

/* Input Styling */
.input-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 1rem;
}

.input-group label {
  display: block;
  font-weight: 600;
  margin-bottom: 0.5rem;
  font-size: 0.9rem;
}

/* Strategy Selector Styles */
.label-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.manual-override {
  font-size: 0.75rem;
  color: #9ca3af;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 4px;
}
.manual-override input { margin: 0; }

.strategy-display-readonly {
  background: #f3f4f6;
  border: 1px solid #e5e7eb;
  padding: 0.75rem;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.auto-icon { font-size: 1.5rem; }
.text-col { display: flex; flex-direction: column; }
.sub-text { font-size: 0.8rem; color: #6b7280; }

.date-input, .strategy-select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
}

/* Scenarios Buttons */
.scenarios-container {
  margin-top: 1rem;
  border-top: 1px solid #e5e7eb;
  padding-top: 1rem;
}
.scenarios-container label { font-size: 0.85rem; font-weight: 600; color: #4b5563; margin-bottom: 0.5rem; display:block;}

.scenario-buttons {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.btn-scenario {
  flex: 1;
  min-width: 140px;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background: white;
  cursor: pointer;
  display: flex;
  flex-direction: column;
  align-items: center;
  transition: all 0.2s;
}

.btn-scenario:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.btn-scenario.small:hover { border-color: #10b981; background: #ecfdf5; }
.btn-scenario.medium:hover { border-color: #f59e0b; background: #fffbeb; }
.btn-scenario.large:hover { border-color: #ef4444; background: #fef2f2; }

.algo-hint {
  font-size: 0.7rem;
  color: #6b7280;
  margin-top: 4px;
  font-weight: 500;
}

/* Action Buttons */
.btn-primary {
  background: #2563eb; color: white; border: none;
  padding: 0.75rem 1.5rem; border-radius: 0.5rem; font-weight: 600; cursor: pointer;
}
.btn-primary:disabled { background: #93c5fd; cursor: not-allowed; }

.btn-secondary {
  background: #f3f4f6; color: #374151; border: 1px solid #d1d5db;
  padding: 0.75rem 1.5rem; border-radius: 0.5rem; font-weight: 600; cursor: pointer;
}
.btn-secondary:hover:not(:disabled) { background: #e5e7eb; }

/* Rebalance */
.rebalance-results {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
  flex-wrap: wrap;
}
.dock-card {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 1rem;
  flex: 1;
  min-width: 200px;
}
.dock-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
  border-bottom: 1px solid #e5e7eb;
  padding-bottom: 0.5rem;
}
.dock-header h4 { margin: 0; color: #2563eb; }
.load-badge {
  background: #dbeafe; color: #1e40af; padding: 2px 8px;
  border-radius: 12px; font-size: 0.8rem; font-weight: bold;
}
.vessel-list { list-style: none; padding: 0; margin: 0; font-size: 0.9rem; }
.vessel-list li { padding: 2px 0; border-bottom: 1px dashed #e5e7eb; }

/* Metrics Tags */
.metrics-tags { display: flex; gap: 0.5rem; margin-top: 0.5rem; margin-bottom: 1rem; flex-wrap: wrap;}
.tag { padding: 4px 8px; border-radius: 4px; font-size: 0.85rem; font-weight: bold; }
.tag.algorithm { background: #e0e7ff; color: #4338ca; }
.tag.delay { background: #d1fae5; color: #065f46; }
.tag.delay.high-delay { background: #fee2e2; color: #b91c1c; }
.tag.time { background: #f3f4f6; color: #374151; }

.tech-metrics { color: #6b7280; font-size: 0.8rem; margin-top: -0.5rem; margin-bottom: 1rem; }

.status-error {
  background: #fee2e2; color: #b91c1c; padding: 1rem;
  border-radius: 0.5rem; margin-top: 1rem;
}

.health-status {
  margin-bottom: 1rem; font-size: 0.9rem; font-weight: bold;
  display: flex; align-items: center; gap: 0.5rem;
}
.status-dot { width: 10px; height: 10px; border-radius: 50%; display: inline-block; }
.status-dot.online { background: #10b981; }
.status-dot.offline { background: #ef4444; }
</style>