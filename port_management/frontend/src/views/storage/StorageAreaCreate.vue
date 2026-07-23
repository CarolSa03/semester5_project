<template>
    <div class="dashboard-content">
      <div class="card">
        <h2 style="margin-bottom: 1.5rem">Create Storage Area</h2>

        <div v-if="successMessage" class="success">{{ successMessage }}</div>
        <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="id"
              >Storage Area ID <span class="required">*</span></label
            >
            <input
              id="id"
              v-model="formData.id"
              type="text"
              required
              maxlength="50"
              placeholder="Unique identifier (max 50 chars)"
              :class="{ invalid: errors.id }"
            />
            <div v-if="errors.id" class="field-error">{{ errors.id }}</div>
          </div>

          <div class="form-group">
            <label for="type"
              >Type <span class="required">*</span></label
            >
            <input
              id="type"
              v-model="formData.type"
              type="text"
              required
              maxlength="20"
              placeholder="Type of storage (e.g., container, bulk)"
              :class="{ invalid: errors.type }"
            />
            <div v-if="errors.type" class="field-error">{{ errors.type }}</div>
          </div>

          <div class="form-group">
            <label for="location"
              >Location <span class="required">*</span></label
            >
            <input
              id="location"
              v-model="formData.location"
              type="text"
              required
              maxlength="100"
              placeholder="Physical location in the port"
              :class="{ invalid: errors.location }"
            />
            <div v-if="errors.location" class="field-error">{{ errors.location }}</div>
          </div>

          <div class="form-group">
            <label for="maxCapacityTEU"
              >Maximum Capacity (TEU) <span class="required">*</span></label
            >
            <input
              id="maxCapacityTEU"
              v-model.number="formData.maxCapacityTEU"
              type="number"
              required
              min="0"
              placeholder="Maximum capacity in TEU"
              :class="{ invalid: errors.maxCapacityTEU }"
            />
            <div v-if="errors.maxCapacityTEU" class="field-error">{{ errors.maxCapacityTEU }}</div>
          </div>

          <div class="form-group">
            <label for="currentCapacityTEU"
              >Current Capacity (TEU) <span class="required">*</span></label
            >
            <input
              id="currentCapacityTEU"
              v-model.number="formData.currentCapacityTEU"
              type="number"
              required
              min="0"
              placeholder="Current occupancy in TEU"
              :class="{ invalid: errors.currentCapacityTEU }"
            />
            <div v-if="errors.currentCapacityTEU" class="field-error">
              {{ errors.currentCapacityTEU }}
            </div>
          </div>

          <div class="form-group">
            <label for="servedDocks">Served Docks</label>
            <select id="servedDocks" v-model="formData.servedDockIds" multiple>
              <option v-for="dock in allDocks" :key="dock.id" :value="dock.id">
                {{ dock.name }} - {{ dock.location }}
              </option>
            </select>
            <div class="help-text">Hold Ctrl (Cmd on Mac) to select multiple docks</div>
          </div>

          <div class="form-group">
            <label for="notes">Notes</label>
            <textarea
              id="notes"
              v-model="formData.notes"
              maxlength="500"
              rows="4"
              placeholder="Additional comments (max 500 chars)"
            ></textarea>
            <div v-if="errors.notes" class="field-error">{{ errors.notes }}</div>
          </div>

          <div class="form-actions">
            <button type="submit" class="btn btn-primary" :disabled="loading">
              {{ loading ? 'Creating...' : 'Create Storage Area' }}
            </button>
            <router-link to="/storage-area/list" class="btn btn-secondary">Cancel</router-link>
          </div>
        </form>
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useStorageAreaApi } from '../../services/useStorageAreaApi'
import { useDockApi, type Dock } from '../../services/useDockApi'

const router = useRouter()
const { loading, createStorageArea } = useStorageAreaApi()
const { fetchDocks } = useDockApi()

const allDocks = ref<Dock[]>([])
const successMessage = ref('')
const errorMessage = ref('')

const formData = ref({
  id: '',
  type: '',
  location: '',
  maxCapacityTEU: 0,
  currentCapacityTEU: 0,
  servedDockIds: [] as number[],
  notes: '',
})

const errors = ref<Record<string, string>>({})

onMounted(async () => {
  try {
    allDocks.value = await fetchDocks()
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load docks'
  }
})

async function handleSubmit() {
  clearErrors()

  // Validate
  let isValid = true

  if (!formData.value.id || formData.value.id.length > 50) {
    errors.value.id = 'ID is required and must be 1-50 characters'
    isValid = false
  }

  if (!formData.value.type || formData.value.type.length > 20) {
    errors.value.type = 'Type is required and max 20 characters'
    isValid = false
  }

  if (!formData.value.location || formData.value.location.length > 100) {
    errors.value.location = 'Location is required and max 100 characters'
    isValid = false
  }

  if (isNaN(formData.value.maxCapacityTEU) || formData.value.maxCapacityTEU < 0) {
    errors.value.maxCapacityTEU = 'Max capacity must be non-negative'
    isValid = false
  }

  if (isNaN(formData.value.currentCapacityTEU) || formData.value.currentCapacityTEU < 0) {
    errors.value.currentCapacityTEU = 'Current capacity must be non-negative'
    isValid = false
  }

  if (formData.value.notes && formData.value.notes.length > 500) {
    errors.value.notes = 'Notes cannot exceed 500 characters'
    isValid = false
  }

  if (!isValid) return

  try {
    const payload = {
      id: formData.value.id.trim(),
      type: formData.value.type.trim(),
      location: formData.value.location.trim(),
      maxCapacityTEU: formData.value.maxCapacityTEU,
      currentCapacityTEU: formData.value.currentCapacityTEU,
      servedDockIds: formData.value.servedDockIds,
      notes: formData.value.notes.trim() || undefined,
    }

    await createStorageArea(payload)
    successMessage.value = 'Storage area created successfully!'
    setTimeout(() => {
      router.push('/storage-area/list')
    }, 1500)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to create storage area'
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

function clearErrors() {
  errors.value = {}
  successMessage.value = ''
  errorMessage.value = ''
}
</script>

<style scoped>
@import '../../assets/dashboard.css';
</style>
