<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useQualificationApi, type Qualification } from '../../services/useQualificationApi'
import { validateRequired } from '../../utils/validation'

const router = useRouter()
const { loading, createQualification } = useQualificationApi()

const formData = ref<Qualification>({
    id: '',
  code: '',
  description: '',
})

const fieldErrors = ref({
  code: '',
  description: '',
})

const successMessage = ref('')
const errorMessage = ref('')

function clearErrors() {
  fieldErrors.value = {
    code: '',
    description: ''
  }
}

function validateCodeFormat(code: string): string {
  if (!code) {
    return 'Code is required'
  }

  const codePattern = /^[A-Z]{3}-\d{3}$/
  if (!codePattern.test(code)) {
    return 'Code must follow format XXX-### (e.g., TRK-001)'
  }
  
  return ''
}

function validateForm(): boolean {
  clearErrors()
  let isValid = true

  const codeError = validateCodeFormat(formData.value.code)
  if (codeError) {
    fieldErrors.value.code = codeError
    isValid = false
  }

  const nameError = validateRequired(formData.value.description, 'Descriptive name')
  if (nameError) {
    fieldErrors.value.description = nameError
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validateForm()) {
    return
  }

  try {
    const result = await createQualification(formData.value)
    successMessage.value = `Qualification created successfully! Code: ${result.code}`
    
    setTimeout(() => {
      router.push('/qualification/list')
    }, 1500)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to create qualification. Please try again.'
    window.scrollTo({ top: 0, behavior: 'smooth' })
    if (
      err.message?.toLowerCase().includes('already exists') ||
      err.message?.toLowerCase().includes('qualification with code')
    ) {
      errorMessage.value = 'A qualification with this code already exists. Please choose a different code.'
    } else {
      errorMessage.value = err.message || 'Failed to create qualification. Please try again.'
    }
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

function handleCancel() {
  router.push('/qualification/list')
}

function handleCodeInput(event: Event) {
  const target = event.target as HTMLInputElement
  formData.value.code = target.value.toUpperCase()
}
</script>

<template>
    <div class="dashboard-content">
      <div class="card">
        <h2 style="margin-bottom: 1.5rem;">New Qualification</h2>

        <div v-if="successMessage" class="success">
          {{ successMessage }}
        </div>

        <div v-if="errorMessage" class="error">
          {{ errorMessage }}
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="code">
              Qualification Code <span class="required">*</span>
            </label>
            <input
              :value="formData.code"
              @input="handleCodeInput"
              type="text"
              id="code"
              placeholder="e.g., TRK-001"
              maxlength="7"
              :class="{ invalid: fieldErrors.code }"
            />
            <div class="help-text">Format: XXX-### (3 uppercase letters, hyphen, 3 digits)</div>
            <div v-if="fieldErrors.code" class="field-error">
              {{ fieldErrors.code }}
            </div>
          </div>

          <div class="form-group">
            <label for="description">
                Description <span class="required">*</span>
            </label>
            <input
                v-model="formData.description"
                type="text"
                id="description"
                placeholder="e.g., Software Engineer"
                maxlength="200"
                :class="{ invalid: fieldErrors.description }"
            />
            <div class="help-text">Clear description of the qualification</div>
        <div v-if="fieldErrors.description" class="field-error">
    {{ fieldErrors.description }}
  </div>
</div>


          <div class="form-actions">
            <button type="submit" class="btn btn-primary" :disabled="loading">
              {{ loading ? 'Creating...' : 'Create Qualification' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="handleCancel">
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