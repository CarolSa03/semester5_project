<template>
  <div class="dashboard-container">
    <div class="dashboard-content">
      <div class="card" style="max-width: 900px; margin: 2rem auto">
        <h2 style="margin-bottom: 1.5rem">New Dock</h2>
        <div v-if="message" :class="message.type">{{ message.text }}</div>
        <form @submit.prevent="handleSubmit">
          <div class="form-row">
            <div class="form-group">
              <label for="name">Dock Name <span class="required">*</span></label>
              <input
                id="name"
                v-model="form.name"
                type="text"
                :class="{ invalid: errors.name }"
                required
                placeholder="e.g., Dock A1"
                autocomplete="off"
              />
              <div class="field-error">{{ errors.name }}</div>
            </div>
            <div class="form-group">
              <label for="location">Location <span class="required">*</span></label>
              <input
                id="location"
                v-model="form.location"
                type="text"
                :class="{ invalid: errors.location }"
                required
                placeholder="e.g., North Terminal"
                autocomplete="off"
              />
              <div class="field-error">{{ errors.location }}</div>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label for="length">Length (meters) <span class="required">*</span></label>
              <input
                id="length"
                v-model.number="form.length"
                type="number"
                :class="{ invalid: errors.length }"
                required
                min="1"
                placeholder="e.g., 300"
              />
              <div class="field-error">{{ errors.length }}</div>
            </div>
            <div class="form-group">
              <label for="depth">Depth (meters) <span class="required">*</span></label>
              <input
                id="depth"
                v-model.number="form.depth"
                type="number"
                :class="{ invalid: errors.depth }"
                required
                min="1"
                placeholder="e.g., 15"
              />
              <div class="field-error">{{ errors.depth }}</div>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label for="maxDraft">Max Draft (meters) <span class="required">*</span></label>
              <input
                id="maxDraft"
                v-model.number="form.maxDraft"
                type="number"
                :class="{ invalid: errors.maxDraft }"
                required
                min="1"
                placeholder="e.g., 12"
              />
              <div class="field-error">{{ errors.maxDraft }}</div>
            </div>
            <div class="form-group">
              <label for="maxSTS">Max STS Operations <span class="required">*</span></label>
              <input
                id="maxSTS"
                v-model.number="form.maxSTS"
                type="number"
                :class="{ invalid: errors.maxSTS }"
                required
                min="0"
                placeholder="e.g., 2"
              />
              <div class="help-text">Maximum simultaneous ship-to-ship operations</div>
              <div class="field-error">{{ errors.maxSTS }}</div>
            </div>
          </div>
          <div class="form-group full-width">
            <label>Allowed Vessel Types <span class="required">*</span></label>
            <div class="vessel-types-grid">
              <div v-for="vt in vesselTypes" :key="vt.id" class="checkbox-item">
                <input
                  :id="'vt-' + vt.id"
                  v-model="form.allowedVesselTypes"
                  type="checkbox"
                  :value="vt.id"
                />
                <label :for="'vt-' + vt.id">{{ vt.name }}</label>
              </div>
            </div>
            <div class="field-error">{{ errors.allowedVesselTypes }}</div>
          </div>
          <div class="form-actions">
            <button type="submit" class="btn btn-primary" :disabled="submitting">
              {{ submitting ? 'Creating...' : 'Create Dock' }}
            </button>
            <router-link to="/dock/list" class="btn btn-secondary">Cancel</router-link>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useDockApi, type VesselType, type Dock } from '../../services/useDockApi'
import { validateDockForm, type DockFormData } from '../../utils/validation'

const router = useRouter()
const { fetchVesselTypes, createDock } = useDockApi()

const vesselTypes = ref<VesselType[]>([])
const form = ref<DockFormData>({
  name: '',
  location: '',
  length: undefined,
  depth: undefined,
  maxDraft: undefined,
  maxSTS: undefined,
  allowedVesselTypes: []
})
const errors = ref<{ [k: string]: string }>({})
const message = ref<{ type: string; text: string } | null>(null)
const submitting = ref(false)

onMounted(async () => {
  try {
    vesselTypes.value = await fetchVesselTypes()
  } catch (e) {
    message.value = { type: 'error', text: 'Failed to load vessel types' }
  }
})

async function handleSubmit() {
  errors.value = validateDockForm(form.value)
  if (Object.keys(errors.value).length > 0) return

  submitting.value = true
  message.value = null
  try {
    const dockPayload: Dock = {
      name: form.value.name.trim(),
      location: form.value.location.trim(),
      length: form.value.length!,
      depth: form.value.depth!,
      maxDraft: form.value.maxDraft!,
      maxSTS: form.value.maxSTS!,
      allowedVesselTypes: form.value.allowedVesselTypes.map(id => String(id)),
      stsCranes: [],
      isActive: true
    }
    
    const result = await createDock(dockPayload)
    message.value = { type: 'success', text: 'Dock created successfully!' }

    if (result && result.id) {
      setTimeout(() => router.push(`/dock/list`), 1500)
    } else {
      setTimeout(() => router.push('/dock/list'), 1500)
    }
  } catch (err: any) {
    console.error('Create dock error:', err)
    message.value = { type: 'error', text: err.message || 'Server error' }
    submitting.value = false
  }
}
</script>

<style scoped>
@import '../../assets/dashboard.css';
</style>
