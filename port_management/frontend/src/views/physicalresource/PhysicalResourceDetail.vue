<template>
    <main class="dashboard-content">
      <div v-if="error" class="error-message">{{ error }}</div>
      <div v-if="loading" class="loading">Loading resource details...</div>

      <div v-else-if="resource" class="detail-container">
        <div class="detail-header">
          <h2>{{ resource.code }} - {{ resource.description }}</h2>
          <span 
            class="badge" 
            :class="{
              'badge-success': resource.status === 'Active',
              'badge-warning': resource.status === 'Maintenance',
              'badge-secondary': resource.status === 'Inactive'
            }"
          >
            {{ resource.status }}
          </span>
        </div>

        <div class="detail-grid">
          <div class="detail-section">
            <h3>Basic Information</h3>
            <div class="detail-row">
              <span class="detail-label">Code:</span>
              <span class="detail-value">{{ resource.code }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Description:</span>
              <span class="detail-value">{{ resource.description }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Area:</span>
              <span class="detail-value">{{ resource.area }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Status:</span>
              <span class="detail-value">{{ resource.status }}</span>
            </div>
          </div>

          <div class="detail-section">
            <h3>Operational Details</h3>
            <div class="detail-row">
              <span class="detail-label">Setup Time:</span>
              <span class="detail-value">{{ resource.setupTime }} minutes</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Operational Window:</span>
              <span class="detail-value">{{ resource.operationalWindow }}</span>
            </div>
            <div class="detail-row">
              <span class="detail-label">Capacity:</span>
              <span class="detail-value">{{ resource.capacity }} {{ resource.capacityUnit }}</span>
            </div>
            <div v-if="'speed' in resource" class="detail-row">
              <span class="detail-label">Speed:</span>
              <span class="detail-value">{{ resource.speed }} {{ resource.speedUnit }}</span>
            </div>
          </div>

          <div class="detail-section full-width">
            <h3>Required Qualifications</h3>
            <div v-if="resource.requiredQualificationIds.length > 0" class="qualification-list">
                <span 
                  v-for="qualId in resource.requiredQualificationIds" 
                  :key="qualId" 
                  class="badge badge-info"
                >
                  {{ qualifications[qualId]?.description || qualId }}
                </span>
              </div>  
            <div v-else class="detail-value">No required qualifications</div>
          </div>
        </div>

        <div class="action-buttons">
          <router-link :to="`/resource/${resource.id}/edit`" class="btn btn-primary">
            Edit Resource
          </router-link>
          <button 
            @click="handleDeactivate" 
            class="btn btn-danger"
            :disabled="resource.status === 'Inactive'"
          >
            Deactivate
          </button>
          <router-link to="/resource/list" class="btn btn-secondary">
            Back to List
          </router-link>
        </div>
      </div>
    </main>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { usePhysicalResourceApi, type PhysicalResource } from '../../services/usePhysicalResourceApi'
import { Qualification, useQualificationApi } from '@/services/useQualificationApi'

const route = useRoute()
const router = useRouter()
const { getById, deactivate } = usePhysicalResourceApi()

const resource = ref<PhysicalResource | null>(null)
const loading = ref(true)
const error = ref('')

const qualifications = ref<Record<string, Qualification>>({})
const { listQualifications } = useQualificationApi()

const loadResource = async () => {
  loading.value = true
  error.value = ''
  try {
    const id = route.params.id as string
    resource.value = await getById(id)
  } catch (err) {
    error.value = 'Failed to load resource details'
    console.error(err)
  } finally {
    loading.value = false
  }
}

const handleDeactivate = async () => {
  if (!resource.value) return
  
  if (!confirm('Are you sure you want to deactivate this resource?')) return

  try {
    await deactivate(resource.value.id)
    router.push('/resource/list')
  } catch (err) {
    error.value = 'Failed to deactivate resource'
    console.error(err)
  }
}

onMounted(async () => {
  try {
    const quals = await listQualifications()
    qualifications.value = quals.reduce((map, q) => {
      map[q.id] = q
      return map
    }, {} as Record<string, Qualification>)
  } catch (err) {
    console.error("Failed to load qualifications", err)
  }
})

onMounted(() => {
  loadResource()
})
</script>

<style scoped>
@import '../../assets/dashboard.css';

.qualification-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}
</style>
