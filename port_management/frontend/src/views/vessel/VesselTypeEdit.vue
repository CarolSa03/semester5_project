<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useVesselTypeApi, type VesselType } from '../../services/useVesselTypeApi'
import { validateRequired } from '../../utils/validation'

const route = useRoute()
const router = useRouter()
const { getVesselType, updateVesselType, deleteVesselType } = useVesselTypeApi()

const name = ref('')
const description = ref('')
const capacityTEU = ref<number | null>(null)
const maxRows = ref<number | null>(null)
const maxBays = ref<number | null>(null)
const maxTiers = ref<number | null>(null)
const isActive = ref(true)

const errorMessage = ref('')
const successMessage = ref('')
const fieldErrors = ref<Record<string, string>>({})
const isLoading = ref(true)
const isSubmitting = ref(false)
const isDeleting = ref(false)

const vesselTypeId = computed(() => route.params.id as string)

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
    await updateVesselType(vesselTypeId.value, {
      name: name.value.trim(),
      description: description.value.trim() || null,
      capacityTEU: capacityTEU.value,
      maxRows: maxRows.value,
      maxBays: maxBays.value,
      maxTiers: maxTiers.value,
      isActive: isActive.value
    })

    successMessage.value = 'Vessel type updated successfully!'
    setTimeout(() => {
      router.push(`/vessel-type/${vesselTypeId.value}`)
    }, 1500)
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to update vessel type'
  } finally {
    isSubmitting.value = false
  }
}

onMounted(async () => {
  try {
    const vesselType = await getVesselType(vesselTypeId.value)
    name.value = vesselType.name
    description.value = vesselType.description || ''
    capacityTEU.value = vesselType.capacityTEU ?? null
    maxRows.value = vesselType.maxRows ?? null
    maxBays.value = vesselType.maxBays ?? null
    maxTiers.value = vesselType.maxTiers ?? null
    isActive.value = vesselType.isActive
  } catch (error) {
    alert('Failed to load vessel type')
    router.push('/vessel-type/list')
  } finally {
    isLoading.value = false
  }
})

const handleDelete = async () => {
  if (!confirm('Are you sure you want to delete this vessel type? This action cannot be undone.')) {
    return
  }
  isDeleting.value = true
  try {
    await deleteVesselType(vesselTypeId.value)
    router.push('/vessel-type/list')
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Failed to delete vessel type'
  } finally {
    isDeleting.value = false
  }
}
</script>

<<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="isLoading" class="loading">Loading vessel type...</div>

      <div v-else>
        <h2 style="margin-bottom: 1.5rem;">Edit Vessel Type</h2>

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
              required
            />
            <div v-if="fieldErrors.name" class="field-error">{{ fieldErrors.name }}</div>
          </div>

          <div class="form-group">
            <label for="description">Description</label>
            <textarea id="description" v-model="description"></textarea>
          </div>

          <h3 style="margin-top: 2rem; margin-bottom: 1rem;">Container Specifications</h3>

          <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
            <div class="form-group" style="flex: 1;">
              <label for="capacityTEU">Capacity (TEU)</label>
              <input id="capacityTEU" v-model.number="capacityTEU" type="number" min="0" />
            </div>

            <div class="form-group" style="flex: 1;">
              <label for="maxRows">Max Rows</label>
              <input id="maxRows" v-model.number="maxRows" type="number" min="0" />
            </div>

            <div class="form-group" style="flex: 1;">
              <label for="maxBays">Max Bays</label>
              <input id="maxBays" v-model.number="maxBays" type="number" min="0" />
            </div>

            <div class="form-group" style="flex: 1;">
              <label for="maxTiers">Max Tiers</label>
              <input id="maxTiers" v-model.number="maxTiers" type="number" min="0" />
            </div>
          </div>

          <div class="form-group">
            <div class="checkbox-group">
              <input id="isActive" v-model="isActive" type="checkbox" />
              <label for="isActive" style="margin-bottom: 0;">Vessel Type is Active</label>
            </div>
          </div>

          <div class="form-actions">
            <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
              {{ isSubmitting ? 'Saving...' : 'Save Changes' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="router.push('/vessel-type/list')">
              Cancel
            </button>
          </div>
            <div class="danger-zone" style="margin-top: 3rem; padding-top: 2rem; border-top: 1px solid #e5e7eb;">
            <h3 style="color: #dc2626; margin-bottom: 0.5rem;">Danger Zone</h3>
            <p style="color: #6b7280; margin-bottom: 1rem;">
              Once you delete this vessel type, there is no going back. Please be certain.
            </p>
            <button
              class="btn btn-danger"
              @click="handleDelete"
              :disabled="isDeleting"
            >
              {{ isDeleting ? 'Deleting...' : 'Delete Vessel Type' }}
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

.danger-zone {
  margin-top: 3rem;
  padding-top: 2rem;
  border-top: 1px solid #e5e7eb;
}

.btn-danger {
  background: #dc2626;
  color: white;
  border: none;
  padding: 0.625rem 1.25rem;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-danger:hover {
  background: #b91c1c;
}

</style>
