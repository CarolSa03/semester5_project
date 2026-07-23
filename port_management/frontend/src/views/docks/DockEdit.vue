<template>
  <div class="dashboard-container">
    <div class="dashboard-content">
      <div class="card" :style="{ maxWidth: '900px', margin: '2rem auto' }">
        <div v-if="loading" class="loading">
          <span class="spinner"></span> Loading dock...
        </div>
        <div v-else>
          <h2 style="margin-bottom: 1.5rem">Edit Dock</h2>
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
                />
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
            <div class="form-group status-toggle">
              <input id="isActive" v-model="form.isActive" type="checkbox" />
              <label for="isActive" style="margin: 0">Dock is Active</label>
            </div>
            <div class="form-actions">
              <button type="submit" class="btn btn-primary" :disabled="submitting">
                {{ submitting ? 'Saving...' : 'Save Changes' }}
              </button>
              <router-link to="/dock/list" class="btn btn-secondary">Cancel</router-link>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useDockApi, type VesselType } from '../../services/useDockApi'
import { validateDockForm, type DockFormData } from '../../utils/validation'

const { loading, fetchDockById, fetchVesselTypes, updateDock } = useDockApi()
const route = useRoute()
const router = useRouter()

const vesselTypes = ref<VesselType[]>([])
const form = ref<DockFormData & { isActive: boolean }>({
  name: '',
  location: '',
  length: undefined,
  depth: undefined,
  maxDraft: undefined,
  maxSTS: undefined,
  allowedVesselTypes: [],
  isActive: true
})
const errors = ref<{ [k: string]: string }>({})
const message = ref<{ type: string; text: string } | null>(null)
const submitting = ref(false)

onMounted(async () => {
  const dockId = (route.params.id || route.query.id) as string
  if (!dockId) {
    router.push('/dock/list')
    return
  }
  try {
    const [dock, types] = await Promise.all([fetchDockById(dockId), fetchVesselTypes()])
    vesselTypes.value = types
    form.value = {
      name: dock.name || '',
      location: dock.location || '',
      length: dock.length || undefined,
      depth: dock.depth || undefined,
      maxDraft: dock.maxDraft || undefined,
      maxSTS: dock.maxSTS || undefined,
      isActive: !!dock.isActive,
      allowedVesselTypes: (dock.allowedVesselTypes ?? []).map((vt: any) => typeof vt === 'string' ? vt : vt.id)
    }
  } catch (e: any) {
    message.value = { type: 'error', text: e.message || 'Failed to load dock' }
    setTimeout(() => router.push('/dock/list'), 1800)
  }
})

async function handleSubmit() {
  errors.value = validateDockForm(form.value)
  if (Object.keys(errors.value).length > 0) return

  submitting.value = true
  message.value = null
  const dockId = (route.params.id || route.query.id) as string
  try {
    await updateDock(dockId, {
      name: form.value.name,
      location: form.value.location,
      length: form.value.length!,
      depth: form.value.depth!,
      maxDraft: form.value.maxDraft!,
      maxSTS: form.value.maxSTS!,
      allowedVesselTypes: form.value.allowedVesselTypes.map(id => String(id)),
      stsCranes: [],
      isActive: form.value.isActive
    })
    message.value = { type: 'success', text: 'Dock updated successfully!' }
    setTimeout(() => router.push(`/dock/view/${dockId}`), 1500)
  } catch (err: any) {
    message.value = { type: 'error', text: err.message || 'Server error' }
    submitting.value = false
  }
}
</script>

<style scoped>
@import '../../assets/dashboard.css';
</style>
