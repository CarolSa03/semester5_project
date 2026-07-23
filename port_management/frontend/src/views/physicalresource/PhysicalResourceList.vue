<template>
  <main class="dashboard-content">
    <div class="content-header">
      <div class="filter-tabs">
        <button 
          @click="changeFilter('trucks')" 
          :class="{ active: currentFilter === 'trucks' }"
          class="filter-tab"
        >
          Trucks
        </button>
        <button 
          @click="changeFilter('yardcranes')" 
          :class="{ active: currentFilter === 'yardcranes' }"
          class="filter-tab"
        >
          Yard Cranes
        </button>
        <button 
          @click="changeFilter('stscranes')" 
          :class="{ active: currentFilter === 'stscranes' }"
          class="filter-tab"
        >
          STS Cranes
        </button>
      </div>
      <router-link to="/resource/create" class="btn btn-primary">
        Add Physical Resource
      </router-link>
    </div>

    <div v-if="error" class="error-message">{{ error }}</div>

    <div v-else-if="loading" class="loading">Loading physical resources...</div>

    <div v-else-if="filteredResources.length > 0" class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Code</th>
            <th>Description</th>
            <th>Area</th>
            <th>Type</th>
            <th>Status</th>
            <th>Capacity</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="resource in filteredResources" :key="resource.id">
            <td>{{ resource.code }}</td>
            <td>{{ resource.description }}</td>
            <td>{{ resource.area }}</td>
            <td><span class="badge badge-info">{{ getResourceType(resource) }}</span></td>
            <td>
              <span
                class="badge"
                :class="{
                  'badge-success': resource.status === 'Active',
                  'badge-warning': resource.status === 'Maintenance',
                  'badge-secondary': resource.status === 'Inactive'
                }"
              >
                {{ resource.status }}
              </span>
            </td>
            <td>{{ resource.capacity }} {{ resource.capacityUnit }}</td>
            <td>
              <div class="action-buttons">
                <router-link 
                  :to="`/resource/${resource.id}`" 
                  class="btn btn-sm btn-secondary"
                >
                  View
                </router-link>
                <router-link 
                  :to="`/resource/${resource.id}/edit`" 
                  class="btn btn-sm btn-secondary"
                >
                  Edit
                </router-link>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else>
      <p>No physical resources found for this category.</p>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from "vue"
import {
  usePhysicalResourceApi,
  type PhysicalResource,
} from "../../services/usePhysicalResourceApi"

const { listTrucks, listYardCrane, listSTSCranes } = usePhysicalResourceApi()

const resources = ref<PhysicalResource[]>([])
const loading = ref(false)
const error = ref("")

const currentFilter = ref<"trucks" | "yardcranes" | "stscranes">("trucks")

const loadResourcesByType = async (type: typeof currentFilter.value) => {
  loading.value = true
  error.value = ""
  try {
    switch (type) {
      case "trucks":
        resources.value = await listTrucks()
        break
      case "yardcranes":
        resources.value = await listYardCrane()
        break
      case "stscranes":
        resources.value = await listSTSCranes()
        break
    }
  } catch (err) {
    error.value = "Failed to load physical resources"
    console.error(err)
    resources.value = []
  } finally {
    loading.value = false
  }
}

const filteredResources = computed(() => resources.value)

const getResourceType = (resource: PhysicalResource): string => {
  if ("speed" in resource) return "Truck"
  if (!("speed" in resource)) return "Crane"
  return "Unknown"
}

const changeFilter = (filter: typeof currentFilter.value) => {
  currentFilter.value = filter
}

watch(currentFilter, () => {
  loadResourcesByType(currentFilter.value)
})

onMounted(() => {
  loadResourcesByType(currentFilter.value)
})
</script>

<style scoped>
@import "../../assets/dashboard.css";

.filter-tabs {
  display: flex;
  gap: 0.5rem;
}

.filter-tab {
  padding: 0.5rem 1rem;
  border: 1px solid #ddd;
  background: white;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.2s;
}

.filter-tab:hover {
  background: #f3f4f6;
}

.filter-tab.active {
  background: #2563eb;
  color: white;
  border-color: #2563eb;
}
</style>
