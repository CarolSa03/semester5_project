<template>
  <div class="page">
    <h1>Privacy Policy</h1>
    
    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      Loading...
    </div>
    
    <!-- Error State -->
    <div v-else-if="error" class="error">{{ error }}</div>
    
    <!-- Current Policy -->
    <div v-else-if="policy" class="policy-container">
      <div class="policy-header">
        <div class="version-badge">Current Version</div>
        <p class="version-info">Version {{ policy.version }} | Effective: {{ date }}</p>
      </div>
      
      <div class="policy-content" v-html="policy.content"></div>
      
      <!-- Version History Section -->
      <div class="version-history-section">
        <button @click="toggleVersionHistory" class="version-history-toggle">
          <span class="icon">📜</span>
          {{ showVersionHistory ? 'Hide' : 'View' }} Previous Versions
          <span class="chevron" :class="{ 'rotated': showVersionHistory }">▼</span>
        </button>

        <div v-if="showVersionHistory" class="version-list">
          <div v-if="loadingHistory" class="loading-small">Loading history...</div>
          
          <div v-else-if="versionHistory.length === 0" class="no-history">
            No previous versions available.
          </div>
          
          <div v-else>
            <p class="version-list-intro">Previous versions of our Privacy Policy:</p>
            <div v-for="version in versionHistory" :key="version.id" class="version-item">
              <div class="version-item-info">
                <strong>Version {{ version.version }}</strong>
                <span class="version-date">{{ formatDate(version.createdAt) }}</span>
              </div>
              <button @click="viewOldVersion(version)" class="btn-view-old">
                View
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- No Policy Available -->
    <div v-else class="no-policy">
      No privacy policy available yet.
    </div>

    <!-- Modal for Old Versions -->
    <div v-if="showModal" class="modal-overlay" @click="closeModal">
      <div class="modal-content" @click.stop>
        <button class="close-btn" @click="closeModal">✕</button>
        
        <div class="modal-header">
          <h2>Privacy Policy</h2>
          <p class="version-info">
            Version {{ selectedVersion?.version }} | 
            Published: {{ formatDate(selectedVersion?.createdAt) }}
          </p>
          <div class="archived-badge">Archived Version</div>
        </div>
        
        <div class="modal-body" v-html="selectedVersion?.content"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import privacyService, { type PrivacyPolicy } from '@/services/privacyService';

const policy = ref<PrivacyPolicy | null>(null);
const loading = ref(true);
const error = ref('');

const showVersionHistory = ref(false);
const versionHistory = ref<PrivacyPolicy[]>([]);
const loadingHistory = ref(false);

const showModal = ref(false);
const selectedVersion = ref<PrivacyPolicy | null>(null);

onMounted(async () => {
  try {
    const res = await privacyService.getCurrent();
    policy.value = res.data;
  } catch (err: any) {
    console.error('Error:', err);
    error.value = err.response?.data?.error || 'Failed to load privacy policy';
  } finally {
    loading.value = false;
  }
});

const date = computed(() => 
  policy.value ? new Date(policy.value.createdAt).toLocaleDateString() : ''
);

const toggleVersionHistory = async () => {
  showVersionHistory.value = !showVersionHistory.value;
  
  // Fetch history only when opening for the first time
  if (showVersionHistory.value && versionHistory.value.length === 0) {
    await fetchVersionHistory();
  }
};

const fetchVersionHistory = async () => {
  loadingHistory.value = true;
  try {
    const res = await privacyService.getHistory();
    versionHistory.value = res.data;
  } catch (err: any) {
    console.error('Error fetching history:', err);
  } finally {
    loadingHistory.value = false;
  }
};

const viewOldVersion = (version: PrivacyPolicy) => {
  selectedVersion.value = version;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedVersion.value = null;
};

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};
</script>

<style scoped>
.page {
  max-width: 900px;
  margin: 2rem auto;
  padding: 2rem;
}

h1 {
  color: #1e3a5f;
  margin-bottom: 1.5rem;
}

.loading {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 2rem;
  color: #64748b;
}

.spinner {
  width: 24px;
  height: 24px;
  border: 3px solid #e2e8f0;
  border-top-color: #2563eb;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.error {
  color: #dc2626;
  padding: 1rem;
  background: #fee2e2;
  border: 1px solid #fecaca;
  border-radius: 8px;
}

.no-policy {
  padding: 2rem;
  text-align: center;
  color: #64748b;
  background: #f8fafc;
  border-radius: 8px;
}

.policy-container {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.06);
}

.policy-header {
  position: relative;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e2e8f0;
}

.version-badge {
  display: inline-block;
  background: #10b981;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.version-info {
  color: #64748b;
  font-size: 0.95rem;
  margin: 0;
}

.policy-content {
  line-height: 1.8;
  color: #334155;
}

.policy-content :deep(h2) {
  color: #1e293b;
  margin-top: 2rem;
  margin-bottom: 1rem;
  font-size: 1.5rem;
}

.policy-content :deep(h3) {
  color: #334155;
  margin-top: 1.5rem;
  margin-bottom: 0.75rem;
  font-size: 1.25rem;
}

.policy-content :deep(ul) {
  margin-left: 1.5rem;
  margin-bottom: 1rem;
}

.policy-content :deep(p) {
  margin-bottom: 1rem;
}

/* Version History Section */
.version-history-section {
  margin-top: 3rem;
  padding-top: 2rem;
  border-top: 2px solid #e2e8f0;
}

.version-history-toggle {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 1rem 1.5rem;
  background: #f8fafc;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-weight: 500;
  color: #475569;
  cursor: pointer;
  transition: all 0.2s;
}

.version-history-toggle:hover {
  background: #f1f5f9;
  border-color: #94a3b8;
}

.version-history-toggle .icon {
  margin-right: 0.5rem;
}

.version-history-toggle .chevron {
  transition: transform 0.3s;
  margin-left: 0.5rem;
}

.version-history-toggle .chevron.rotated {
  transform: rotate(180deg);
}

.version-list {
  margin-top: 1rem;
  padding: 1.5rem;
  background: #f9fafb;
  border-radius: 8px;
}

.loading-small {
  color: #64748b;
  text-align: center;
  padding: 1rem;
}

.no-history {
  color: #94a3b8;
  text-align: center;
  padding: 1rem;
  font-style: italic;
}

.version-list-intro {
  color: #64748b;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.version-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  margin-bottom: 0.75rem;
  transition: all 0.2s;
}

.version-item:hover {
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.08);
  transform: translateX(2px);
}

.version-item-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.version-item-info strong {
  color: #1e293b;
  font-size: 1rem;
}

.version-date {
  color: #64748b;
  font-size: 0.85rem;
}

.btn-view-old {
  padding: 0.5rem 1.25rem;
  background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-view-old:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10000;
  padding: 1rem;
}

.modal-content {
  background: white;
  border-radius: 12px;
  max-width: 900px;
  max-height: 90vh;
  width: 100%;
  overflow-y: auto;
  position: relative;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.close-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 36px;
  height: 36px;
  border: none;
  background: #f1f5f9;
  color: #475569;
  border-radius: 50%;
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.2s;
  z-index: 10;
}

.close-btn:hover {
  background: #e2e8f0;
  color: #1e293b;
}

.modal-header {
  position: relative;
  padding: 2rem;
  border-bottom: 2px solid #e2e8f0;
}

.modal-header h2 {
  color: #1e3a5f;
  margin: 0 0 0.5rem 0;
}

.archived-badge {
  position: absolute;
  top: 2rem;
  right: 4rem;
  background: #f59e0b;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.modal-body {
  padding: 2rem;
  line-height: 1.8;
  color: #334155;
}

.modal-body :deep(h2) {
  color: #1e293b;
  margin-top: 2rem;
  margin-bottom: 1rem;
}

.modal-body :deep(h3) {
  color: #334155;
  margin-top: 1.5rem;
  margin-bottom: 0.75rem;
}

.modal-body :deep(ul) {
  margin-left: 1.5rem;
  margin-bottom: 1rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .page {
    padding: 1rem;
  }

  .policy-container {
    padding: 1.5rem;
  }

  .version-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }

  .btn-view-old {
    width: 100%;
  }

  .modal-content {
    max-height: 95vh;
  }

  .modal-header,
  .modal-body {
    padding: 1.5rem;
  }

  .archived-badge {
    position: static;
    display: inline-block;
    margin-top: 0.5rem;
  }
}
</style>
