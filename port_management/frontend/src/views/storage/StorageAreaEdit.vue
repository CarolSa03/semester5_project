<template>
    <div class="dashboard-content">
      <div class="card">
        <h2 style="margin-bottom: 1.5rem">Edit Storage Area</h2>

        <div v-if="successMessage" class="success">{{ successMessage }}</div>
        <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

        <div v-if="initialLoading" class="loading">
          <span class="spinner"></span> Loading storage area details...
        </div>

        <form v-else-if="formData.id" @submit.prevent="handleSubmit">
          <div class="form-group">
            <label>Storage Area ID</label>
            <div class="id-display">{{ formData.id }}</div>
            <div class="help-text">ID cannot be changed</div>
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
              :class="{ invalid: errors.type }"
            />
            <div class="help-text">Type of storage (e.g., container, bulk)</div>
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
              :class="{ invalid: errors.location }"
            />
            <div class="help-text">Physical location in the port</div>
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
            ></textarea>
            <div class="help-text">Additional notes or comments (max 500 characters)</div>
            <div v-if="errors.notes" class="field-error">{{ errors.notes }}</div>
          </div>

          <div class="form-actions">
            <button type="submit" class="btn btn-primary" :disabled="loading">
              {{ loading ? 'Updating...' : 'Update Storage Area' }}
            </button>
            <router-link to="/storage-area/list" class="btn btn-secondary">Cancel</router-link>
          </div>
        </form>
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useStorageAreaApi } from '../../services/useStorageAreaApi'
import { useDockApi, type Dock } from '../../services/useDockApi'

const route = useRoute()
const router = useRouter()
const { loading, getStorageArea, updateStorageArea } = useStorageAreaApi()
const { fetchDocks } = useDockApi()

const allDocks = ref<Dock[]>([])
const initialLoading = ref(true)
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
  const id = route.params.id as string
  if (!id) {
    errorMessage.value = 'Storage area ID is required'
    initialLoading.value = false
    return
  }

  try {
    const [storageArea, docks] = await Promise.all([getStorageArea(id), fetchDocks()])

    formData.value = {
      id: storageArea.id,
      type: storageArea.type,
      location: storageArea.location,
      maxCapacityTEU: storageArea.maxCapacityTEU,
      currentCapacityTEU: storageArea.currentCapacityTEU,
      servedDockIds: storageArea.servedDockIds || [],
      notes: storageArea.notes || '',
    }

    allDocks.value = docks
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load data'
  } finally {
    initialLoading.value = false
  }
})

async function handleSubmit() {
  clearErrors()

  // Validate
  let isValid = true

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

  if (!isValid) return

  try {
    const payload = {
      id: formData.value.id,
      type: formData.value.type.trim(),
      location: formData.value.location.trim(),
      maxCapacityTEU: formData.value.maxCapacityTEU,
      currentCapacityTEU: formData.value.currentCapacityTEU,
      servedDockIds: formData.value.servedDockIds,
      notes: formData.value.notes.trim() || undefined,
    }

    await updateStorageArea(formData.value.id, payload)
    successMessage.value = 'Storage area updated successfully!'
    setTimeout(() => {
      router.push(`/storage-area/view/${encodeURIComponent(formData.value.id)}`)
    }, 1500)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to update storage area'
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

.id-display {
  background: #f9fafb;
  padding: 0.75rem;
  border-radius: 4px;
  color: #6b7280;
  font-family: monospace;
}
</style>
