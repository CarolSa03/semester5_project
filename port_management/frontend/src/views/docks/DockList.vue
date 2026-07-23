<template>
  <div class="dashboard-container">
    <div class="dashboard-content">
      <div class="actions">
        <div class="left-actions">
          <h2>All Docks</h2>
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Search name, vessel type, location..."
            class="search-input"
            @keydown.enter="searchDocks"
          />
          <button class="btn btn-primary" @click="searchDocks">Search</button>
          <div class="filter-group">
            <label>
              <input v-model="showInactive" type="checkbox" />
              Show Inactive Docks
            </label>
          </div>
        </div>
        <router-link to="/dock/create" class="btn btn-primary">+ Create Dock</router-link>
      </div>

      <div v-if="error" class="error">{{ error }}</div>

      <div class="card">
        <div v-if="loading" class="loading">
          <span class="spinner"></span> Loading docks...
        </div>
        <div v-else>
          <template v-if="filteredDocks.length">
            <table>
              <thead>
                <tr>
                  <th>NAME</th>
                  <th>LOCATION</th>
                  <th>DIMENSIONS</th>
                  <th>MAX DRAFT</th>
                  <th>MAX STS</th>
                  <th>VESSEL TYPES</th>
                  <th>STATUS</th>
                  <th>ACTIONS</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="dock in filteredDocks"
                  :key="dock.id"
                  :class="{ 'inactive-row': !dock.isActive }"
                >
                  <td><strong>{{ extractValue(dock.name) }}</strong></td>
                  <td>{{ extractValue(dock.location) }}</td>
                  <td>{{ extractValue(dock.length) }}m × {{ extractValue(dock.depth) }}m</td>
                  <td>{{ extractValue(dock.maxDraft) }}m</td>
                  <td>{{ extractValue(dock.maxSTS) }}</td>
                  <td>
                    <span v-if="dock.allowedVesselTypes && dock.allowedVesselTypes.length" class="vessel-types">
                      {{ dock.allowedVesselTypes.length }} type{{ dock.allowedVesselTypes.length !== 1 ? 's' : '' }}
                    </span>
                    <span v-else class="text-muted">None</span>
                  </td>
                  <td>
                    <span class="badge" :class="dock.isActive ? 'badge-active' : 'badge-inactive'">
                      {{ dock.isActive ? "Active" : "Inactive" }}
                    </span>
                  </td>
                  <td>
                    <div class="actions-cell">
                      <router-link :to="`/dock/view/${dock.id}`" class="btn btn-secondary btn-small">
                        View
                      </router-link>
                      <router-link :to="`/dock/edit/${dock.id}`" class="btn btn-primary btn-small">
                        Edit
                      </router-link>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </template>
          <template v-else>
            <div class="empty-state">
              <h3>No Docks Found</h3>
              <p v-if="searchTerm">
                No docks match your search term "{{ searchTerm }}". Try a different search or clear the filter.
              </p>
              <p v-else>Create your first dock to get started.</p>
              <br />
              <router-link to="/dock/create" class="btn btn-primary">+ Create Dock</router-link>
              <button v-if="searchTerm" class="btn btn-secondary" @click="clearSearch" style="margin-left: 0.5rem">
                Clear Search
              </button>
            </div>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue"
import { Dock, useDockApi } from "../../services/useDockApi"

const searchTerm = ref("")
const showInactive = ref(true)
const { loading, error, fetchDocks } = useDockApi()
const docks = ref<Dock[]>([])

const filteredDocks = computed(() => {
  return showInactive.value ? docks.value : docks.value.filter((d) => d.isActive)
})

// Helper function to extract value from nested objects
function extractValue(val: any): any {
  if (val && typeof val === 'object' && 'value' in val) {
    return val.value
  }
  return val
}

onMounted(() => {
  loadDocks()
})

async function loadDocks() {
  try {
    docks.value = await fetchDocks(searchTerm.value || undefined)
  } catch (err) {
    console.error('Failed to load docks:', err)
  }
}

function searchDocks() {
  loadDocks()
}

function clearSearch() {
  searchTerm.value = ""
  loadDocks()
}
</script>

<style scoped>
@import "../../assets/dashboard.css";

.actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.left-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.search-input {
  padding: 0.5rem 0.75rem;
  border: 1px solid var(--color-border, #ddd);
  border-radius: 4px;
  font-size: 0.9rem;
  min-width: 250px;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filter-group label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  cursor: pointer;
  user-select: none;
}

.filter-group input[type="checkbox"] {
  width: 18px;
  height: 18px;
  cursor: pointer;
}

.vessel-types {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  background: var(--color-bg-secondary, #f5f5f5);
  border-radius: 4px;
  font-size: 0.85rem;
  color: var(--color-text-secondary, #666);
}

.text-muted {
  color: var(--color-text-muted, #999);
  font-style: italic;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
}

.inactive-row {
  opacity: 0.6;
  background-color: var(--color-bg-inactive, #f9f9f9);
}

.badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 500;
}

.badge-active {
  background-color: #d4edda;
  color: #155724;
}

.badge-inactive {
  background-color: #f8d7da;
  color: #721c24;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
}

.empty-state h3 {
  margin-bottom: 0.5rem;
  color: var(--color-text-primary, #333);
}

.empty-state p {
  color: var(--color-text-secondary, #666);
  margin-bottom: 0;
}
</style>
