<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useQualificationApi, type Qualification } from '../../services/useQualificationApi'

const route = useRoute()
const router = useRouter()
const { loading, getQualification } = useQualificationApi()

const qualification = ref<Qualification | null>(null)
const errorMessage = ref('')

const qualCode = route.params.code as string;

onMounted(async () => {
  try {
    qualification.value = await getQualification(qualCode)
  } catch (err: any) {
    errorMessage.value = err.message || 'Failed to load qualification'
  }
})

function goBack() {
  router.push('/qualification/list')
}

function editQualification() {
  router.push(`/qualification/${qualCode}/edit`)
}
</script>

<template>
    <div class="dashboard-content">
      <div class="card">
        <div v-if="loading" class="loading">Loading qualification details...</div>

        <div v-else-if="errorMessage" class="error">
          {{ errorMessage }}
        </div>

        <div v-else-if="qualification" class="detail-content">
          <div class="detail-header">
            <h2>{{ qualification.description }}</h2>
            <div class="detail-actions">
              <button @click="editQualification" class="btn btn-primary">Edit</button>
              <button @click="goBack" class="btn btn-secondary">Back to List</button>
            </div>
          </div>

          <div class="detail-grid">
            <div class="detail-item">
              <div class="detail-label">Qualification Code</div>
              <div class="detail-value">
                <span class="badge" style="background: #e0e7ff; color: #3730a3; font-weight: 600; font-size: 1.25rem; padding: 0.5rem 1rem;">
                  {{ qualification.code }}
                </span>
              </div>
            </div>

            <div class="detail-item">
              <div class="detail-label">Descriptive Name</div>
              <div class="detail-value">{{ qualification.description }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
</template>

<style scoped>
@import '../../assets/dashboard.css';
</style>
