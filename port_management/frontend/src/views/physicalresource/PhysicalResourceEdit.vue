<template>
  <main class="dashboard-content">
    <div v-if="loadError" class="error-message">{{ loadError }}</div>
    <div v-if="loadingResource" class="loading">Loading resource...</div>

    <div v-else class="form-container">
      <div v-if="successMessage" class="success-message">{{ successMessage }}</div>
      <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

      <form @submit.prevent="handleSubmit">
        <div class="form-section">
          <h3>Basic Information</h3>
          <div class="form-group">
            <label for="code">Code *</label>
            <input
              v-model="formData.code"
              type="text"
              id="code"
              required
              placeholder="e.g., TRK-001"
            />
            <span v-if="errors.code" class="error-text">{{ errors.code }}</span>
          </div>

          <div class="form-group">
            <label for="description">Description *</label>
            <textarea
              v-model="formData.description"
              id="description"
              required
              rows="3"
              placeholder="Enter resource description"
            ></textarea>
            <span v-if="errors.description" class="error-text">{{ errors.description }}</span>
          </div>

          <div class="form-group">
            <label for="area">Area *</label>
            <input
              v-model="formData.area"
              type="text"
              id="area"
              required
              placeholder="e.g., North Yard"
            />
            <span v-if="errors.area" class="error-text">{{ errors.area }}</span>
          </div>

          <div class="form-group">
            <label for="status">Status *</label>
            <select v-model="formData.status" id="status" required>
              <option value="Active">Active</option>
              <option value="Maintenance">Maintenance</option>
              <option value="Inactive">Inactive</option>
            </select>
          </div>
        </div>

        <div class="form-section">
          <h3>Operational Details</h3>
          <div class="form-group">
            <label for="setupTime">Setup Time (minutes) *</label>
            <input
              v-model.number="formData.setupTime"
              type="number"
              id="setupTime"
              required
              min="0"
              placeholder="0"
            />
          </div>

          <div class="form-group">
            <label for="operationalWindow">Operational Window *</label>
            <input
              v-model="formData.operationalWindow"
              type="text"
              id="operationalWindow"
              required
              placeholder="e.g., 24/7, 06:00-22:00"
            />
          </div>

          <div class="form-group">
            <label for="capacity">Capacity *</label>
            <input
              v-model="formData.capacity"
              type="text"
              id="capacity"
              required
              placeholder="e.g., 20"
            />
          </div>

          <div class="form-group">
            <label for="capacityUnit">Capacity Unit *</label>
            <input
              v-model="formData.capacityUnit"
              type="text"
              id="capacityUnit"
              required
              placeholder="e.g., tons, TEU"
            />
          </div>

          <div v-if="isTruck" class="form-group">
            <label for="speed">Speed *</label>
            <input
              v-model="formData.speed"
              type="text"
              id="speed"
              placeholder="e.g., 40"
            />
          </div>

          <div v-if="isTruck" class="form-group">
            <label for="speedUnit">Speed Unit *</label>
            <input
              v-model="formData.speedUnit"
              type="text"
              id="speedUnit"
              placeholder="e.g., km/h, mph"
            />
          </div>
        </div>

        <div class="form-section">
          <h3>Required Qualifications</h3>
          <div class="form-group">
            <label for="qualifications">Qualification IDs (comma-separated)</label>
            <input
              v-model="qualificationsInput"
              type="text"
              id="qualifications"
              placeholder="e.g., qual-1, qual-2, qual-3"
            />
            <small>Enter qualification IDs separated by commas</small>
          </div>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Updating...' : 'Update Resource' }}
          </button>
          <router-link to="/resource/list" class="btn btn-secondary">
            Cancel
          </router-link>
        </div>
      </form>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { validateRequired } from '../../utils/validation'
import { usePhysicalResourceApi, type PhysicalResource } from '../../services/usePhysicalResourceApi'

const route = useRoute()
const router = useRouter()
const { getById, update } = usePhysicalResourceApi()

const resource = ref<PhysicalResource | null>(null)
const loadingResource = ref(true)
const loadError = ref('')

const formData = ref({
  code: '',
  description: '',
  area: '',
  setupTime: 0,
  operationalWindow: '',
  status: '',
  capacity: '',
  capacityUnit: '',
  speed: '',
  speedUnit: ''
})

const qualificationsInput = ref('')
const errors = ref<Record<string, string>>({})
const loading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const isTruck = computed(() => resource.value && 'speed' in resource.value)

const qualificationIds = computed(() => {
  if (!qualificationsInput.value.trim()) return []
  return qualificationsInput.value
    .split(',')
    .map(id => id.trim())
    .filter(id => id)
})

const loadResource = async () => {
  loadingResource.value = true
  loadError.value = ''
  try {
    const id = route.params.id as string
    resource.value = await getById(id)
    formData.value = {
      code: resource.value.code,
      description: resource.value.description,
      area: resource.value.area,
      setupTime: resource.value.setupTime,
      operationalWindow: resource.value.operationalWindow,
      status: resource.value.status,
      capacity: resource.value.capacity,
      capacityUnit: resource.value.capacityUnit,
      speed: 'speed' in resource.value ? resource.value.speed : '',
      speedUnit: 'speedUnit' in resource.value ? resource.value.speedUnit : ''
    }
    qualificationsInput.value = resource.value.requiredQualificationIds.join(', ')
  } catch (err) {
    loadError.value = 'Failed to load resource'
    console.error(err)
  } finally {
    loadingResource.value = false
  }
}

const validateForm = (): boolean => {
  errors.value = {}

  const codeError = validateRequired(formData.value.code, 'Code')
  if (codeError) errors.value.code = codeError

  const descError = validateRequired(formData.value.description, 'Description')
  if (descError) errors.value.description = descError

  const areaError = validateRequired(formData.value.area, 'Area')
  if (areaError) errors.value.area = areaError

  return Object.keys(errors.value).length === 0
}

const handleSubmit = async () => {
  if (!validateForm() || !resource.value) return

  loading.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    let typeDiscriminator = (resource.value as any).resourceType;

    if (!typeDiscriminator) {
      if (isTruck.value) {
        typeDiscriminator = 'truck';
      } else {
         typeDiscriminator = 'yardcrane';
      }
    }

    const updateData: any = {
      resourceType: typeDiscriminator,

      code: formData.value.code,
      description: formData.value.description,
      area: formData.value.area,
      setupTime: formData.value.setupTime,
      operationalWindow: formData.value.operationalWindow,
      requiredQualificationIds: qualificationIds.value,
      status: formData.value.status,
      capacity: formData.value.capacity,
      capacityUnit: formData.value.capacityUnit
    }

    // Include truck-specific fields only if truck
    if (isTruck.value) {
      updateData.speed = formData.value.speed
      updateData.speedUnit = formData.value.speedUnit
    }

    await update(resource.value.id, updateData)

    successMessage.value = 'Physical resource updated successfully!'
    setTimeout(() => {
      router.push(`/resource/list`)
    }, 1500)
  } catch (err) {

    const isSuccessError = 
        (err.response && err.response.status >= 200 && err.response.status < 300) ||
        (err.message && err.message.includes('Unexpected end of JSON'));

    if (isSuccessError) {
       console.log("Update actually succeeded (caught 204 issue).");
       successMessage.value = 'Physical resource updated successfully!'
       setTimeout(() => {
         router.push(`/resource/list`)
       }, 1500)
       return;
    }

    errorMessage.value = 'Failed to update physical resource'
    console.error(err)
  } finally {
    loading.value = false
  }
}

onMounted(() => loadResource())
</script>

<style scoped>
@import '../../assets/dashboard.css';
</style>
