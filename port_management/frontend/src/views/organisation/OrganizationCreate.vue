<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useOrganizationApi, type Organization } from '../../services/useOrganizationApi'
import { validateRequired } from '../../utils/validation'

const router = useRouter()
const { loading, createOrganization } = useOrganizationApi()

const formData = ref<Organization>({
  id: crypto.randomUUID(),
  legalName: '',
  alternativeName: '',
  taxNumber: '',
  createdAt: new Date().toISOString(),
});


const fieldErrors = ref({
  legalName: '',
  taxNumber: '',
})

const successMessage = ref('')
const errorMessage = ref('')

function clearErrors() {
  fieldErrors.value = {
    legalName: '',
    taxNumber: '',
  }
  errorMessage.value = ''
  successMessage.value = ''
}

function validateForm(): boolean {
  clearErrors()
  let isValid = true

  const legalNameError = validateRequired(formData.value.legalName, 'Legal name')
  if (legalNameError) {
    fieldErrors.value.legalName = legalNameError
    isValid = false
  }

  const taxNumberError = validateRequired(formData.value.taxNumber, 'Tax number')
  if (taxNumberError) {
    fieldErrors.value.taxNumber = taxNumberError
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validateForm()) {
    return
  }

  formData.value.id = crypto.randomUUID();
  formData.value.createdAt = new Date().toISOString();

  try {
    const result = await createOrganization(formData.value)
    successMessage.value = 'Organization created successfully!'
    
    setTimeout(() => {
      router.push(`/organization/${result.id}`)
    }, 1500)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to create organization. Please try again.'
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

function handleCancel() {
  router.push('/organization/list')
}
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <h2 style="margin-bottom: 1.5rem;">New Organization</h2>

      <div v-if="successMessage" class="success">{{ successMessage }}</div>
      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="legalName">Legal Name <span class="required">*</span></label>
          <input
            v-model="formData.legalName"
            type="text"
            id="legalName"
            placeholder="Enter official legal name"
            :class="{ invalid: fieldErrors.legalName }"
          />
          <div v-if="fieldErrors.legalName" class="field-error">{{ fieldErrors.legalName }}</div>
        </div>

        <div class="form-group">
          <label for="alternativeName">Alternative Name</label>
          <input
            v-model="formData.alternativeName"
            type="text"
            id="alternativeName"
            placeholder="Enter trade or alternative name"
          />
        </div>

        <div class="form-group">
          <label for="taxNumber">Tax Number <span class="required">*</span></label>
          <input
            v-model="formData.taxNumber"
            type="text"
            id="taxNumber"
            placeholder="Enter tax identification number"
            :class="{ invalid: fieldErrors.taxNumber }"
          />
          <div v-if="fieldErrors.taxNumber" class="field-error">{{ fieldErrors.taxNumber }}</div>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Creating...' : 'Create Organization' }}
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
