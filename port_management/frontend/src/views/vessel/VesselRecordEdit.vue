<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useVesselRecordApi, type VesselRecord } from '../../services/useVesselRecordApi'
import { useVesselTypeApi, type VesselType } from '../../services/useVesselTypeApi'
import { validateRequired } from '../../utils/validation'

const route = useRoute()
const router = useRouter()
const { getVesselRecord, updateVesselRecord } = useVesselRecordApi()
const { listVesselTypes } = useVesselTypeApi()

const imo = ref('')
const name = ref('')
const vesselTypeId = ref<number | null>(null)
const owner = ref('')
const isActive = ref(true)

const vesselTypes = ref<VesselType[]>([])
const errorMessage = ref('')
const successMessage = ref('')
const fieldErrors = ref<Record<string, string>>({})
const isLoading = ref(true)
const isSubmitting = ref(false)

const imoParam = computed(() => route.params.imo as string)

const renderImo = (imoValue: string | { value: string }) => {
  if (typeof imoValue === 'object' && imoValue !== null && 'value' in imoValue) {
    return imoValue.value
  }
  return imoValue
}

const clearErrors = () => {
  errorMessage.value = ''
  successMessage.value = ''
  fieldErrors.value = {}
}

const handleSubmit = async () => {
  clearErrors()

  const nameError = validateRequired(name.value, 'Vessel name')
  const vesselTypeError = vesselTypeId.value ? null : 'Vessel type is required'
  const ownerError = validateRequired(owner.value, 'Owner')

  if (nameError || vesselTypeError || ownerError) {
    if (nameError) fieldErrors.value.name = nameError
    if (vesselTypeError) fieldErrors.value.vesselType = vesselTypeError
    if (ownerError) fieldErrors.value.owner = ownerError
    return
  }

  isSubmitting.value = true

  try {
    await updateVesselRecord(imoParam.value, {
      name: name.value.trim(),
      vesselTypeId: vesselTypeId.value!,
      owner: owner.value.trim(),
      isActive: isActive.value,
      imo: imo.value
    })

    successMessage.value = 'Vessel record updated successfully!'
    setTimeout(() => {
      router.push(`/vessel-record/${encodeURIComponent(imoParam.value)}`)
    }, 1500)
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to update vessel record'
  } finally {
    isSubmitting.value = false
  }
}

onMounted(async () => {
  try {
    const [vesselRecord, types] = await Promise.all([
      getVesselRecord(imoParam.value),
      listVesselTypes()
    ])

    imo.value = renderImo(vesselRecord.imo)
    name.value = vesselRecord.name
    vesselTypeId.value = vesselRecord.vesselTypeId ?? null
    owner.value = vesselRecord.owner
    isActive.value = vesselRecord.isActive
    vesselTypes.value = types
  } catch (error) {
    alert('Failed to load vessel record')
    router.push('/vessel-record/list')
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="isLoading" class="loading">Loading vessel record...</div>

      <div v-else>
        <h2 style="margin-bottom: 1.5rem;">Edit Vessel Record</h2>

        <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
        <div v-if="successMessage" class="success">{{ successMessage }}</div>

        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="imo">IMO Number <span class="required">*</span></label>
            <input id="imo" v-model="imo" type="text" disabled required />
            <div class="help-text">IMO number cannot be changed</div>
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

          <div class="form-group">
            <div class="checkbox-group">
              <input id="isActive" v-model="isActive" type="checkbox" />
              <label for="isActive" style="margin-bottom: 0;">Vessel is Active</label>
            </div>
          </div>

          <div class="form-actions">
            <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
              {{ isSubmitting ? 'Saving...' : 'Save Changes' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="router.push('/vessel-record/list')">
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';

.checkbox-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.checkbox-group input[type='checkbox'] {
  width: 20px;
  height: 20px;
  cursor: pointer;
}
.help-text {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.25rem;
}
</style>
