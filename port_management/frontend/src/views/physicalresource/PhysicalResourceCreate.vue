<template>
    
    <main class="dashboard-content">
      <div class="form-container">
        <div v-if="successMessage" class="success-message">{{ successMessage }}</div>
        <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

        <form @submit.prevent="handleSubmit">
          <div class="form-section">
            <h3>Resource Type</h3>
            <div class="form-group">
              <label for="resourceType">Type *</label>
              <select v-model="formData.resourceType" id="resourceType" required>
                <option value="">Select type...</option>
                <option value="truck">Truck</option>
                <option value="yardcrane">Yard Crane</option>
                <option value="stscrane">STS Crane</option>
              </select>
            </div>
          </div>

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
                <option value="">Select status...</option>
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

            <div v-if="formData.resourceType === 'truck'" class="form-group">
              <label for="speed">Speed *</label>
              <input
                v-model="formData.speed"
                type="text"
                id="speed"
                placeholder="e.g., 40"
              />
            </div>

            <div v-if="formData.resourceType === 'truck'" class="form-group">
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
              {{ loading ? 'Creating...' : 'Create Resource' }}
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
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { validateRequired } from '../../utils/validation'
import { usePhysicalResourceApi } from '../../services/usePhysicalResourceApi'

const router = useRouter()
const { createTruck, createYardCrane, createSTSCrane } = usePhysicalResourceApi()

const formData = ref({
  resourceType: '',
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

const qualificationIds = computed(() => {
  if (!qualificationsInput.value.trim()) return []
  return qualificationsInput.value
    .split(',')
    .map(id => id.trim())
    .filter(id => id)
})

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
  if (!validateForm()) return

  loading.value = true
  successMessage.value = ''
  errorMessage.value = ''

  try {
    const baseData = {
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

    let result

    switch (formData.value.resourceType) {
      case 'truck':
        result = await createTruck({
          ...baseData,
          speed: formData.value.speed,
          speedUnit: formData.value.speedUnit
        })
        break
      case 'yardcrane':
        result = await createYardCrane(baseData)
        break
      case 'stscrane':
        result = await createSTSCrane(baseData)
        break
      default:
        throw new Error('Invalid resource type')
    }

    if (!result || !result.id) {
      throw new Error('No ID returned from create operation')
    }

    successMessage.value = 'Physical resource created successfully!'
    setTimeout(() => {
      router.push(`/resource/list`)
    }, 1500)
  } catch (err) {
    errorMessage.value = 'Failed to create physical resource'
    console.error(err)
  } finally {
    loading.value = false
  }
}

</script>

<style scoped>
@import '../../assets/dashboard.css';
</style>
