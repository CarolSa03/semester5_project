<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { validateRequired } from '../../utils/validation'
import { useRepresentativeApi } from '../../services/useRepresentativeApi'
import { useOrganizationApi } from '../../services/useOrganizationApi'

const route = useRoute()
const router = useRouter()
const { fetchRepresentativeById, updateRepresentative, deleteRepresentative } = useRepresentativeApi()
const { listOrganizations } = useOrganizationApi()

const name = ref('')
const email = ref('')
const phone = ref('')
const shippingAgentOrganizationId = ref('')

const organizations = ref<any[]>([])
const errors = ref<Record<string, string>>({})
const successMessage = ref('')
const errorMessage = ref('')
const submitting = ref(false)
const loading = ref(true)

onMounted(async () => {
  try {
    const id = route.params.id as string
    const [repData, orgsData] = await Promise.all([
      fetchRepresentativeById(id),
      listOrganizations()
    ])

    name.value = repData.name
    email.value = repData.email
    phone.value = repData.phone
    shippingAgentOrganizationId.value = repData.shippingAgentOrganizationId
    organizations.value = orgsData
    loading.value = false
  } catch (err) {
    alert('Failed to load representative')
    router.push('/representative/list')
  }
})

const clearErrors = () => {
  errors.value = {}
  errorMessage.value = ''
  successMessage.value = ''
}

const validateForm = () => {
  clearErrors()
  let isValid = true

  const nameError = validateRequired(name.value, 'Name')
  if (nameError) {
    errors.value.name = nameError
    isValid = false
  }

  const emailError = validateRequired(email.value, 'Email')
  if (emailError) {
    errors.value.email = emailError
    isValid = false
  }

  const phoneError = validateRequired(phone.value, 'Phone')
  if (phoneError) {
    errors.value.phone = phoneError
    isValid = false
  }

  const orgError = validateRequired(shippingAgentOrganizationId.value, 'Organization')
  if (orgError) {
    errors.value.organization = orgError
    isValid = false
  }

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  submitting.value = true

  try {
    const id = route.params.id as string
    await updateRepresentative(id, {
      name: name.value.trim(),
      email: email.value.trim(),
      phone: phone.value.trim(),
      shippingAgentOrganizationId: shippingAgentOrganizationId.value
    })

    successMessage.value = 'Representative updated successfully!'
    setTimeout(() => {
      router.push(`/representative/${id}`)
    }, 1500)
  } catch (err) {
    errorMessage.value = err instanceof Error ? err.message : 'Failed to update representative'
    submitting.value = false
  }
}

async function handleDelete() {
  if (!confirm('Are you sure you want to delete this representative? This action cannot be undone.')) {
    return
  }

  try {
    const id = route.params.id as string
    await deleteRepresentative(id)
    alert('Representative deleted successfully')
    router.push('/representative/list')
  } catch (err: any) {
    alert(err.message || 'Failed to delete representative')
  }
}

</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <div v-if="loading" class="loading">Loading representative...</div>

      <div v-else>
        <h2 style="margin-bottom: 1.5rem;">Edit Representative</h2>

        <div v-if="successMessage" class="success">{{ successMessage }}</div>
        <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="name">Name <span class="required">*</span></label>
            <input
              v-model="name"
              type="text"
              id="name"
              placeholder="Enter full name"
              :class="{ invalid: errors.name }"
            />
            <div v-if="errors.name" class="field-error">{{ errors.name }}</div>
          </div>

          <div class="form-group">
            <label for="email">Email <span class="required">*</span></label>
            <input
              v-model="email"
              type="email"
              id="email"
              placeholder="Enter email address"
              :class="{ invalid: errors.email }"
            />
            <div v-if="errors.email" class="field-error">{{ errors.email }}</div>
          </div>

          <div class="form-group">
            <label for="phone">Phone <span class="required">*</span></label>
            <input
              v-model="phone"
              type="tel"
              id="phone"
              placeholder="Enter phone number"
              :class="{ invalid: errors.phone }"
            />
            <div v-if="errors.phone" class="field-error">{{ errors.phone }}</div>
          </div>

          <div class="form-group">
            <label for="organization">Organization <span class="required">*</span></label>
            <select
              v-model="shippingAgentOrganizationId"
              id="organization"
              :class="{ invalid: errors.organization }"
            >
              <option value="">Select an organization</option>
              <option v-for="org in organizations" :key="org.id" :value="org.id">
                {{ org.legalName }}
              </option>
            </select>
            <div v-if="errors.organization" class="field-error">{{ errors.organization }}</div>
          </div>

          <div class="form-actions">
            <button type="submit" class="btn btn-primary" :disabled="submitting">
              {{ submitting ? 'Updating...' : 'Update Representative' }}
            </button>
            <button 
              type="button" 
              @click="router.push(`/representative/${route.params.id}`)" 
              class="btn btn-secondary"
            >
              Cancel
            </button>
          </div>

          <div class="danger-zone">
            <h3>Danger Zone</h3>
            <p>Once you delete this representative, there is no going back. Please be certain.</p>
            <button @click="handleDelete" type="button" class="btn btn-danger">
              Delete Representative
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
