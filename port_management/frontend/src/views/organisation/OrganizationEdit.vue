<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useOrganizationApi, type Organization } from '../../services/useOrganizationApi'
import { validateRequired } from '../../utils/validation'

const route = useRoute()
const router = useRouter()
const { loading, getOrganization, updateOrganization, deleteOrganization } = useOrganizationApi()

const orgId = route.params.id as string

const formData = ref<Organization>({
  legalName: '',
  alternativeName: '',
  taxNumber: '',
})

const fieldErrors = ref({
  legalName: '',
  taxNumber: '',
})

const successMessage = ref('')
const errorMessage = ref('')
const isLoading = ref(true)

onMounted(async () => {
  try {
    const org = await getOrganization(orgId)
    formData.value = {
      legalName: org.legalName,
      alternativeName: org.alternativeName,
      taxNumber: org.taxNumber,
    }
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load organization'
  } finally {
    isLoading.value = false
  }
})

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

  try {
    await updateOrganization(orgId, formData.value)
    successMessage.value = 'Organization updated successfully!'
    
    setTimeout(() => {
      router.push(`/organization/${orgId}`)
    }, 1500)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to update organization. Please try again.'
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

async function handleDelete() {
  if (!confirm('Are you sure you want to delete this organization? This action cannot be undone.')) {
    return
  }

  try {
    await deleteOrganization(orgId)
    alert('Organization deleted successfully')
    router.push('/organization/list')
  } catch (err: any) {
    alert(err.message || 'Failed to delete organization')
  }
}

function handleCancel() {
  router.push('/organization/list')
}
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="isLoading" class="loading">Loading organization...</div>

      <div v-else>
        <h2 style="margin-bottom: 1.5rem;">Edit Organization</h2>

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
              {{ loading ? 'Saving...' : 'Save Changes' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="handleCancel">
              Cancel
            </button>
          </div>

          <div class="danger-zone">
            <h3>Danger Zone</h3>
            <p>Once you delete this organization, there is no going back. Please be certain.</p>
            <button @click="handleDelete" type="button" class="btn btn-danger">
              Delete Organization
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';

.danger-zone {
  margin-top: 3rem;
  padding-top: 2rem;
  border-top: 1px solid #e5e7eb;
}
.danger-zone h3 {
  color: #dc2626;
  margin-bottom: 0.5rem;
  font-size: 1.125rem;
}
.danger-zone p {
  color: #6b7280;
  margin-bottom: 1rem;
  font-size: 0.875rem;
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

