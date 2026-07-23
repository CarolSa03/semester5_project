<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useVesselRecordApi } from '../../services/useVesselRecordApi'
import { useVesselTypeApi, type VesselType } from '../../services/useVesselTypeApi'
import { validateRequired } from '../../utils/validation'

const router = useRouter()
const { createVesselRecord } = useVesselRecordApi()
const { listVesselTypes } = useVesselTypeApi()

const imoValue = ref('')
const name = ref('')
const vesselTypeId = ref<number | null>(null)
const owner = ref('')

const vesselTypes = ref<VesselType[]>([])
const errorMessage = ref('')
const successMessage = ref('')
const fieldErrors = ref<Record<string, string>>({})
const isSubmitting = ref(false)

const clearErrors = () => {
  errorMessage.value = ''
  successMessage.value = ''
  fieldErrors.value = {}
}

const handleSubmit = async () => {
  clearErrors()

  const imoError = validateRequired(imoValue.value, 'IMO number')
  const nameError = validateRequired(name.value, 'Vessel name')
  const vesselTypeError = vesselTypeId.value ? null : 'Vessel type is required'
  const ownerError = validateRequired(owner.value, 'Owner')

  if (imoError || nameError || vesselTypeError || ownerError) {
    if (imoError) fieldErrors.value.imo = imoError
    if (nameError) fieldErrors.value.name = nameError
    if (vesselTypeError) fieldErrors.value.vesselType = vesselTypeError
    if (ownerError) fieldErrors.value.owner = ownerError
    return
  }

  if (!imoValue.value.match(/^\d{7}$/)) {
    fieldErrors.value.imo = 'IMO must be exactly 7 digits (with valid checksum)'
    return
  }

  isSubmitting.value = true

  try {
    const cleanedImo = imoValue.value.replace(/^IMO/, '')
    const result = await createVesselRecord({
      imoValue: cleanedImo,
      name: name.value.trim(),
      vesselTypeId: vesselTypeId.value!,
      owner: owner.value.trim()
    })

    successMessage.value = 'Vessel record created successfully!'
    const imoForRedirect = typeof result.imo === 'object' ? result.imo.value : result.imo
    setTimeout(() => {
      router.push(`/vessel-record/${encodeURIComponent(imoForRedirect)}`)
    }, 1500)
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to create vessel record'
  } finally {
    isSubmitting.value = false
  }
}

onMounted(async () => {
  try {
    vesselTypes.value = await listVesselTypes()
  } catch (error) {
    errorMessage.value = 'Failed to load vessel types'
  }
})
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <h2 style="margin-bottom: 1.5rem;">New Vessel Record</h2>

      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
      <div v-if="successMessage" class="success">{{ successMessage }}</div>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="imo">IMO Number <span class="required">*</span></label>
          <input
            id="imo"
            v-model="imoValue"
            type="text"
            pattern="\d{7}"
            maxlength="7"
            :class="{ invalid: fieldErrors.imo }"
            placeholder="e.g., 9074729"
            required
          />
          <div class="help-text">7 digits with valid IMO checksum</div>
          <div v-if="fieldErrors.imo" class="field-error">{{ fieldErrors.imo }}</div>
        </div>

        <div class="form-group">
          <label for="name">Vessel Name <span class="required">*</span></label>
          <input
            id="name"
            v-model="name"
            type="text"
            :class="{ invalid: fieldErrors.name }"
            placeholder="e.g., Ocean Explorer"
            required
          />
          <div v-if="fieldErrors.name" class="field-error">{{ fieldErrors.name }}</div>
        </div>

        <div class="form-group">
          <label for="vesselType">Vessel Type <span class="required">*</span></label>
          <select
            id="vesselType"
            v-model="vesselTypeId"
            :class="{ invalid: fieldErrors.vesselType }"
            required
          >
            <option :value="null">Select vessel type</option>
            <option v-for="type in vesselTypes" :key="type.id" :value="type.id">
              {{ type.name }}
            </option>
          </select>
          <div v-if="fieldErrors.vesselType" class="field-error">{{ fieldErrors.vesselType }}</div>
        </div>

        <div class="form-group">
          <label for="owner">Owner / Operator <span class="required">*</span></label>
          <input
            id="owner"
            v-model="owner"
            type="text"
            :class="{ invalid: fieldErrors.owner }"
            placeholder="e.g., Maersk Line"
            required
          />
          <div v-if="fieldErrors.owner" class="field-error">{{ fieldErrors.owner }}</div>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
            {{ isSubmitting ? 'Creating...' : 'Create Vessel Record' }}
          </button>
          <button type="button" class="btn btn-secondary" @click="router.push('/vessel-record/list')">
            Cancel
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';

.help-text {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.25rem;
}
</style>
