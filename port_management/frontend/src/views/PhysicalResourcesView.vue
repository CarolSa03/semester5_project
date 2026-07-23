<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  getTrucks,
  getYardCranes,
  getSTSCranes,
  type TruckDto,
  type YardCraneDto,
  type STSCraneDto
} from '../services/physicalResourceService'

const trucks = ref<TruckDto[]>([])
const yards = ref<YardCraneDto[]>([])
const stsc = ref<STSCraneDto[]>([])

const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    const [t, y, s] = await Promise.all([
      getTrucks(),
      getYardCranes(),
      getSTSCranes()
    ])

    trucks.value = t
    yards.value = y
    stsc.value = s
  } catch (e: any) {
    error.value = e.message || 'Failed to load resources'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <section>
    <h2>Physical Resources</h2>

    <div v-if="loading">Loading…</div>

    <div v-if="error" style="color: red">
      {{ error }}
    </div>

    <template v-else>
      <h3>Trucks</h3>
      <ul>
        <li v-for="t in trucks" :key="t.id">
          <strong>{{ t.code }}</strong> — {{ t.description }}
          ({{ t.capacity }} {{ t.capacityUnit }}, {{ t.speed }} {{ t.speedUnit }})
        </li>
      </ul>

      <h3>Yard Cranes</h3>
      <ul>
        <li v-for="y in yards" :key="y.id">
          <strong>{{ y.code }}</strong> — {{ y.description }}
          ({{ y.capacity }} {{ y.capacityUnit }})
        </li>
      </ul>

      <h3>STS Cranes</h3>
      <ul>
        <li v-for="s in stsc" :key="s.id">
          <strong>{{ s.code }}</strong> — {{ s.description }}
          ({{ s.capacity }} {{ s.capacityUnit }})
        </li>
      </ul>
    </template>
  </section>
</template>

<style scoped>
h3 {
  margin-top: 1.5rem;
}
</style>

