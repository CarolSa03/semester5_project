<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { validateRequired } from '../../utils/validation'
import { useStaffApi } from '../../services/useStaffApi'
import { useQualificationApi } from '../../services/useQualificationApi'

const router = useRouter()
const { createStaff } = useStaffApi()
const { listQualifications } = useQualificationApi()

const staffMemberId = ref('')
const shortName = ref('')
const email = ref('')
const phoneNumber = ref('')
const selectedQualifications = ref<string[]>([]);
const isAvailable = ref(true)

const qualifications = ref<any[]>([])
const errors = ref<Record<string, string>>({})
const successMessage = ref('')
const errorMessage = ref('')
const submitting = ref(false)

const clearErrors = () => {
  errors.value = {}
  errorMessage.value = ''
  successMessage.value = ''
}

const validateForm = () => {
  clearErrors()
  let isValid = true

  const staffIdError = validateRequired(staffMemberId.value, 'Staff Member ID')
  if (staffIdError) {
    errors.value.staffMemberId = staffIdError
    isValid = false
  }

  const nameError = validateRequired(shortName.value, 'Short name')
  if (nameError) {
    errors.value.shortName = nameError
    isValid = false
  }

  const emailError = validateRequired(email.value, 'Email')
  if (emailError) {
    errors.value.email = emailError
    isValid = false
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
    errors.value.email = 'Please enter a valid email address'
    isValid = false
  }

  const phoneError = validateRequired(phoneNumber.value, 'Phone number')
  if (phoneError) {
    errors.value.phoneNumber = phoneError
    isValid = false
  }

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  submitting.value = true

  const staffId = crypto.randomUUID()
  
  const qualificationObjs = qualifications.value
  .filter(q => selectedQualifications.value.includes(q.id));

  try {
    const result = await createStaff({
      id: staffId,
      staffMemberId: staffMemberId.value.trim(),
      shortName: shortName.value.trim(),
      email: email.value.trim(),
      phoneNumber: phoneNumber.value.trim(),
      qualifications: selectedQualifications.value,
      isAvailable: isAvailable.value
    })

    successMessage.value = 'Staff member created successfully!'
    setTimeout(() => {
      router.push(`/staff/list`)
    }, 1500)
  } catch (err) {
    errorMessage.value = err instanceof Error ? err.message : 'Failed to create staff member'
    submitting.value = false
  }
}

onMounted(async () => {
  try {
    qualifications.value = await listQualifications()
  } catch (err) {
    errorMessage.value = 'Failed to load qualifications'
  }
})
</script>

<template>
  <div class="dashboard-content">
    <div class="card">
      <h2 style="margin-bottom: 1.5rem;">New Staff Member</h2>

      <div v-if="successMessage" class="success">{{ successMessage }}</div>
      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="staffMemberId">Staff Member ID <span class="required">*</span></label>
          <input
            v-model="staffMemberId"
            type="text"
            id="staffMemberId"
            placeholder="e.g., 1"
            :class="{ invalid: errors.staffMemberId }"
          />
          <div v-if="errors.staffMemberId" class="field-error">{{ errors.staffMemberId }}</div>
        </div>

        <div class="form-group">
          <label for="shortName">Short Name <span class="required">*</span></label>
          <input
            v-model="shortName"
            type="text"
            id="shortName"
            placeholder="e.g., Carolina"
            maxlength="100"
            :class="{ invalid: errors.shortName }"
          />
          <div v-if="errors.shortName" class="field-error">{{ errors.shortName }}</div>
        </div>

        <div class="form-group">
          <label for="email">Email <span class="required">*</span></label>
          <input
            v-model="email"
            type="email"
            id="email"
            placeholder="e.g., carolina@gmail.com"
            :class="{ invalid: errors.email }"
          />
          <div v-if="errors.email" class="field-error">{{ errors.email }}</div>
        </div>

        <div class="form-group">
          <label for="phoneNumber">Phone Number <span class="required">*</span></label>
          <input
            v-model="phoneNumber"
            type="tel"
            id="phoneNumber"
            placeholder="e.g., 926133969"
            :class="{ invalid: errors.phoneNumber }"
          />
          <div v-if="errors.phoneNumber" class="field-error">{{ errors.phoneNumber }}</div>
        </div>

        <div class="form-group">
          <label for="qualifications">Qualifications</label>
          <select v-model="selectedQualifications" id="qualifications" multiple>
            <option v-for="qual in qualifications" :key="qual.id" :value="qual.id">
              {{ qual.description }}
            </option>
          </select>
          <div class="help-text">Hold Ctrl (Cmd on Mac) to select multiple</div>
        </div>

        <div class="form-group">
          <div class="checkbox-group">
            <input v-model="isAvailable" type="checkbox" id="isAvailable" />
            <label for="isAvailable" style="margin-bottom: 0;">Available for assignment</label>
          </div>
        </div>

        <div class="form-actions">
          <button type="submit" class="btn btn-primary" :disabled="submitting">
            {{ submitting ? 'Creating...' : 'Create Staff Member' }}
          </button>
          <button type="button" @click="router.push('/staff/list')" class="btn btn-secondary">
            Cancel
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';

select[multiple] {
  min-height: 150px;
}

.checkbox-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.checkbox-group input[type="checkbox"] {
  width: auto;
}

.help-text {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.25rem;
}
</style>
