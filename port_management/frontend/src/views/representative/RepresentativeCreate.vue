<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import DashboardHeader from '../../components/layout/DashboardHeader.vue'
import { validateRequired } from '../../utils/validation'
import { useRepresentativeApi } from '../../services/useRepresentativeApi'
import { useOrganizationApi } from '../../services/useOrganizationApi'

const router = useRouter()
const { createRepresentative } = useRepresentativeApi()
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

onMounted(async () => {
  try {
    organizations.value = await listOrganizations()
  } catch (err) {
    errorMessage.value = 'Failed to load organizations'
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
    const result = await createRepresentative({
      name: name.value.trim(),
      email: email.value.trim(),
      phone: phone.value.trim(),
      shippingAgentOrganizationId: shippingAgentOrganizationId.value
    })

    successMessage.value = 'Representative created successfully!'
    setTimeout(() => {
      router.push(`/representative/${result.id}`)
    }, 1500)
  } catch (err) {
    errorMessage.value = err instanceof Error ? err.message : 'Failed to create representative'
    submitting.value = false
  }
}
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <h2 style="margin-bottom: 1.5rem;">New Representative</h2>

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
            {{ submitting ? 'Creating...' : 'Create Representative' }}
          </button>
          <button type="button" @click="router.push('/representative/list')" class="btn btn-secondary">
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
