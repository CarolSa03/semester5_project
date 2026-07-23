<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { useQualificationApi, type Qualification } from '../../services/useQualificationApi'
import { validateRequired } from '../../utils/validation'

const route = useRoute()
const router = useRouter()
const { loading, getQualification, updateQualification } = useQualificationApi()

const qualCode = route.params.code as string;

const formData = ref<Qualification>({
    id: '',
    code: '',
    description: '',
})

const fieldErrors = ref({
  descriptiveName: '',
})

const successMessage = ref('')
const errorMessage = ref('')
const isLoading = ref(true)

onMounted(async () => {
  try {
    const qual = await getQualification(qualCode)
    formData.value = {
        id: qual.id,
        code: qual.code,
        description: qual.description,
    }
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load qualification'
  } finally {
    isLoading.value = false
  }
})

function clearErrors() {
  fieldErrors.value = {
    descriptiveName: '',
  }
  errorMessage.value = ''
  successMessage.value = ''
}

function validateForm(): boolean {
  clearErrors()
  let isValid = true

  const nameError = validateRequired(formData.value.description, 'Descriptive name')
  if (nameError) {
    fieldErrors.value.descriptiveName = nameError
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validateForm()) {
    return
  }

  try {
    await updateQualification(qualCode, formData.value)
    successMessage.value = 'Qualification updated successfully!'
    
    setTimeout(() => {
      router.push(`/qualification/${qualCode}`)
    }, 1500)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to update qualification. Please try again.'
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

function handleCancel() {
  router.push('/qualification/list')
}
</script>

<template>
  <div class="dashboard-container">
    <DashboardHeader />
    
    <div class="dashboard-content">
      <div class="card">
        <div v-if="isLoading" class="loading">Loading qualification details...</div>

        <div v-else>
          <h2 style="margin-bottom: 1.5rem;">Edit Qualification</h2>

          <div v-if="successMessage" class="success">
            {{ successMessage }}
          </div>

          <div v-if="errorMessage" class="error">
            {{ errorMessage }}
          </div>

          <form @submit.prevent="handleSubmit">
            <div class="form-group">
              <label>Qualification Code</label>
              <div style="background: #f9fafb; padding: 0.75rem; border-radius: 4px; color: #6b7280; font-family: monospace; font-weight: 600;">
                {{ qualCode }}
              </div>
              <div class="help-text">Code cannot be changed</div>
            </div>

            <div class="form-group">
              <label for="descriptiveName">
                Descriptive Name <span class="required">*</span>
              </label>
              <input
                v-model="formData.description"
                type="text"
                id="descriptiveName"
                maxlength="100"
                :class="{ invalid: fieldErrors.descriptiveName }"
              />
              <div class="help-text">Clear description of the qualification</div>
              <div v-if="fieldErrors.descriptiveName" class="field-error">
                {{ fieldErrors.descriptiveName }}
              </div>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn btn-primary" :disabled="loading">
                {{ loading ? 'Updating...' : 'Update Qualification' }}
              </button>
              <button type="button" class="btn btn-secondary" @click="handleCancel">
                Cancel
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';
</style>
