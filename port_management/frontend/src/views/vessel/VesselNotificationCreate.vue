<template>
  <div class="dashboard-content">
    <div class="card" style="max-width: 1200px; margin: 2rem auto;">
      <h2 style="margin-bottom: 1.5rem;">New Vessel Visit Notification</h2>

      <div v-if="message" :class="message.type" style="margin-bottom: 1rem;">
        {{ message.text }}
      </div>

      <form @submit.prevent="handleSubmit">
        <!-- Required Section -->
        <fieldset style="border: 1px solid #e5e7eb; padding: 1.5rem; border-radius: 0.5rem; margin-bottom: 1.5rem;">
          <legend style="font-weight: 600; font-size: 1rem; padding: 0 0.5rem;">Required Information</legend>
          
          <div class="form-row" style="gap: 1rem; flex-wrap: wrap; display: flex;">
            <div class="form-group" style="flex: 1 1 45%;">
              <label for="businessId">Business ID *</label>
              <input 
                id="businessId" 
                v-model="form.businessId" 
                type="text" 
                required 
                placeholder="2026-LEIXOES-000001"
              />
              <small style="font-size: 0.875rem; color: #6b7280;">Format: YYYY-PORTCODE-NNNNNN</small>
            </div>

            <div class="form-group" style="flex: 1 1 45%;">
              <label for="vesselId">Vessel ID *</label>
              <input 
                id="vesselId" 
                v-model="form.vesselId" 
                type="text" 
                required 
                placeholder="Vessel UUID"
              />
            </div>

            <div class="form-group" style="flex: 1 1 45%;">
              <label for="shippingAgentRepresentativeId">Shipping Agent Representative ID *</label>
              <input
                id="shippingAgentRepresentativeId"
                v-model.number="form.shippingAgentRepresentativeId"
                type="number"
                required
                placeholder="Agent Rep ID (integer)"
                min="1"
              />
            </div>

            <div class="form-group" style="flex: 1 1 45%;">
              <label for="eta">ETA (Estimated Time of Arrival) *</label>
              <input 
                id="eta" 
                v-model="form.eta" 
                type="datetime-local" 
                required 
              />
            </div>

            <div class="form-group" style="flex: 1 1 45%;">
              <label for="etd">ETD (Estimated Time of Departure) *</label>
              <input 
                id="etd" 
                v-model="form.etd" 
                type="datetime-local" 
                required 
              />
              <small style="font-size: 0.875rem; color: #6b7280;">ETD must be after ETA</small>
            </div>
          </div>
        </fieldset>

        <!-- Optional Section -->
        <fieldset style="border: 1px solid #e5e7eb; padding: 1.5rem; border-radius: 0.5rem; margin-bottom: 1.5rem;">
          <legend style="font-weight: 600; font-size: 1rem; padding: 0 0.5rem;">Optional Information</legend>
          
          <div class="form-row" style="gap: 1rem; flex-wrap: wrap; display: flex;">
            <div class="form-group" style="flex: 1 1 45%;">
              <label for="crewId">Crew ID</label>
              <input 
                id="crewId" 
                v-model="form.crewId" 
                type="text" 
                placeholder="Crew UUID (optional)"
              />
              <small style="font-size: 0.875rem; color: #6b7280;">Optional: Can be added later</small>
            </div>

            <div class="form-group" style="flex: 1 1 45%;">
              <label for="cargoManifests">Cargo Manifests</label>
              <input 
                id="cargoManifests" 
                v-model="cargoManifestsInput" 
                type="text" 
                placeholder="Comma-separated UUIDs (optional)"
                @blur="parseCargoManifests"
              />
              <small style="font-size: 0.875rem; color: #6b7280;">Optional: Separate multiple IDs with commas</small>
            </div>

            <div class="form-group" style="flex: 1 1 45%;">
              <label for="assignedDockId">Assigned Dock ID</label>
              <input 
                id="assignedDockId" 
                v-model="form.assignedDockId" 
                type="text" 
                placeholder="Dock UUID (optional)"
              />
              <small style="font-size: 0.875rem; color: #6b7280;">Usually assigned during approval</small>
            </div>
          </div>
        </fieldset>

        <div class="form-actions" style="margin-top: 1.5rem; display: flex; gap: 1rem;">
          <button type="submit" class="btn btn-primary" :disabled="submitting">
            {{ submitting ? 'Submitting...' : 'Create Notification' }}
          </button>
          <button type="button" class="btn btn-secondary" @click="resetForm" :disabled="submitting">
            Reset
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useVesselVisitNotificationApi } from '@/services/useVesselVisitNotificationApi'

const submitting = ref(false)
const message = ref<{ type: string; text: string } | null>(null)

const form = ref({
  businessId: '',
  vesselId: '',
  shippingAgentRepresentativeId: 0,
  eta: '',
  etd: '',
  crewId: '',
  cargoManifestsId: [] as string[],
  assignedDockId: '',
})

const cargoManifestsInput = ref('')

function parseCargoManifests() {
  if (cargoManifestsInput.value.trim()) {
    form.value.cargoManifestsId = cargoManifestsInput.value
      .split(',')
      .map(id => id.trim())
      .filter(id => id.length > 0)
  } else {
    form.value.cargoManifestsId = []
  }
}

function resetForm() {
  form.value = {
    businessId: '',
    vesselId: '',
    shippingAgentRepresentativeId: 0,
    eta: '',
    etd: '',
    crewId: '',
    cargoManifestsId: [],
    assignedDockId: '',
  }
  cargoManifestsInput.value = ''
  message.value = null
}

async function handleSubmit() {
  submitting.value = true
  message.value = null

  // Validate ETD > ETA
  const etaDate = new Date(form.value.eta)
  const etdDate = new Date(form.value.etd)
  
  if (etdDate <= etaDate) {
    message.value = { type: 'error', text: 'ETD must be after ETA' }
    submitting.value = false
    return
  }

  // Build payload - only include optional fields if they have values
  const payload: any = {
    businessId: form.value.businessId,
    vesselId: form.value.vesselId,
    shippingAgentRepresentativeId: form.value.shippingAgentRepresentativeId,
    eta: etaDate.toISOString(),
    etd: etdDate.toISOString(),
  }

  // Add optional fields only if provided
  if (form.value.crewId) {
    payload.crewId = form.value.crewId
  }

  if (form.value.cargoManifestsId.length > 0) {
    payload.cargoManifestsId = form.value.cargoManifestsId
  }

  if (form.value.assignedDockId) {
    payload.assignedDockId = form.value.assignedDockId
  }

  try {
    const { createNotification } = useVesselVisitNotificationApi()
    await createNotification(payload)
    message.value = { type: 'success', text: 'Vessel visit notification created successfully!' }
    
    // Reset form after successful creation
    setTimeout(() => {
      resetForm()
    }, 2000)
  } catch (err: any) {
    const errorMsg = err.response?.data?.message || err.message || 'Server error'
    message.value = { type: 'error', text: errorMsg }
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
fieldset {
  margin-bottom: 1.5rem;
}

legend {
  color: #374151;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #374151;
}

.form-group input {
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
}

.form-group input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-group input:required {
  border-left: 3px solid #3b82f6;
}

.form-group small {
  margin-top: 0.25rem;
  font-style: italic;
}

.btn {
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-weight: 500;
  cursor: pointer;
  border: none;
  transition: all 0.2s;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #2563eb;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #4b5563;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.success {
  padding: 1rem;
  background-color: #d1fae5;
  color: #065f46;
  border-radius: 0.375rem;
  border: 1px solid #10b981;
  font-weight: 500;
}

.error {
  padding: 1rem;
  background-color: #fee2e2;
  color: #991b1b;
  border-radius: 0.375rem;
  border: 1px solid #ef4444;
  font-weight: 500;
}
</style>
