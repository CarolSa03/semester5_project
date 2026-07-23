<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useVesselTypeApi } from '../../services/useVesselTypeApi'
import { validateRequired } from '../../utils/validation'

const router = useRouter()
const { createVesselType } = useVesselTypeApi()

const name = ref('')
const description = ref('')
const capacityTEU = ref<number | null>(null)
const maxRows = ref<number | null>(null)
const maxBays = ref<number | null>(null)
const maxTiers = ref<number | null>(null)

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

  const nameError = validateRequired(name.value, 'Vessel type name')
  if (nameError) {
    fieldErrors.value.name = nameError
    return
  }

  isSubmitting.value = true

  try {
    const result = await createVesselType({
      name: name.value.trim(),
      description: description.value.trim() || null,
      capacityTEU: capacityTEU.value,
      maxRows: maxRows.value,
      maxBays: maxBays.value,
      maxTiers: maxTiers.value,
      isActive: true
    })

    successMessage.value = 'Vessel type created successfully!'
    setTimeout(() => {
      router.push(`/vessel-type/${result.id}`)
    }, 1500)
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to create vessel type'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <h2 style="margin-bottom: 1.5rem;">New Vessel Type</h2>

      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
      <div v-if="successMessage" class="success">{{ successMessage }}</div>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="name">Vessel Type Name <span class="required">*</span></label>
          <input
            id="name"
            v-model="name"
            type="text"
            :class="{ invalid: fieldErrors.name }"
            placeholder="e.g., Container Ship, Bulk Carrier"
            required
          />
          <div v-if="fieldErrors.name" class="field-error">{{ fieldErrors.name }}</div>
        </div>

        <div class="form-group">
          <label for="description">Description</label>
          <textarea
            id="description"
            v-model="description"
            placeholder="Enter a description of this vessel type"
          ></textarea>
        </div>

        <h3 style="margin-top: 2rem; margin-bottom: 1rem;">Container Specifications</h3>

        <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
          <div class="form-group" style="flex: 1;">
            <label for="capacityTEU">Capacity (TEU)</label>
            <input
              id="capacityTEU"
              v-model.number="capacityTEU"
              type="number"
              min="0"
              placeholder="e.g., 5000"
            />
          </div>

          <div class="form-group" style="flex: 1;">
            <label for="maxRows">Max Rows</label>
            <input id="maxRows" v-model.number="maxRows" type="number" min="0" placeholder="e.g., 20" />
          </div>

          <div class="form-group" style="flex: 1;">
            <label for="maxBays">Max Bays</label>
            <input id="maxBays" v-model.number="maxBays" type="number" min="0" placeholder="e.g., 15" />
          </div>

          <div class="form-group" style="flex: 1;">
            <label for="maxTiers">Max Tiers</label>
            <input id="maxTiers" v-model.number="maxTiers" type="number" min="0" placeholder="e.g., 8" />
          </div>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
            {{ isSubmitting ? 'Creating...' : 'Create Vessel Type' }}
          </button>
          <button type="button" class="btn btn-secondary" @click="router.push('/vessel-type/list')">
            Cancel
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';
</style>
