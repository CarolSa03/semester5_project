<template>
  <div class="admin-privacy">
    <h1>Manage Privacy Policy</h1>

    <div class="card">
      <h2>Create New Version</h2>
      <form @submit.prevent="create">
        <div class="form-group">
          <label>Policy Content (HTML):</label>
          <textarea 
            v-model="content" 
            rows="15" 
            placeholder="<h2>Privacy Policy</h2>
<p>We respect your privacy...</p>"
            required
          ></textarea>
          <small>You can use HTML tags for formatting</small>
        </div>

        <button type="submit" :disabled="loading">
          {{ loading ? 'Publishing...' : 'Publish New Version' }}
        </button>
      </form>

      <div v-if="message" :class="['message', messageType]">
        {{ message }}
      </div>
    </div>

    <div class="card">
      <h2>Current Version</h2>
      <div v-if="loadingCurrent">Loading...</div>
      <div v-else-if="current">
        <p><strong>Version:</strong> {{ current.version }}</p>
        <p><strong>Created:</strong> {{ formatDate(current.createdAt) }}</p>
        <p><strong>By:</strong> {{ current.createdBy }}</p>
        <div class="preview" v-html="current.content"></div>
      </div>
      <div v-else>No active policy found</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';
import privacyService, { type PrivacyPolicy } from '@/services/privacyService';

const content = ref(`<h2>Privacy Policy</h2>

<p>This Privacy Policy explains how we collect, use, and protect your personal information.</p>

<h3>Information We Collect</h3>
<ul>
  <li>Name and contact information</li>
  <li>Account credentials</li>
  <li>Usage data and logs</li>
</ul>

<h3>How We Use Your Data</h3>
<p>We use your information to provide and improve our port management services.</p>

<h3>Your Rights</h3>
<p>You have the right to access, correct, or delete your personal data at any time.</p>

<h3>Contact Us</h3>
<p>For questions about this policy, please contact our Data Protection Officer.</p>`);

const current = ref<PrivacyPolicy | null>(null);
const loading = ref(false);
const loadingCurrent = ref(false);
const message = ref('');
const messageType = ref<'success' | 'error'>('success');

const create = async () => {
  if (!content.value.trim()) {
    message.value = 'Please enter policy content';
    messageType.value = 'error';
    return;
  }

  if (!confirm('Publish new privacy policy? All users will be notified on next login.')) {
    return;
  }

  loading.value = true;
  message.value = '';

  try {
    await axios.post('/api/privacy', JSON.stringify(content.value), {
      headers: { 'Content-Type': 'application/json' }
    });
    
    message.value = '✅ Privacy Policy published successfully!';
    messageType.value = 'success';
    
    // Reload current policy
    await loadCurrent();
    
    // Optional: clear textarea
    // content.value = '';
  } catch (err: any) {
    console.error('Error:', err);
    message.value = '❌ Failed: ' + (err.response?.data?.error || err.message);
    messageType.value = 'error';
  } finally {
    loading.value = false;
  }
};

const loadCurrent = async () => {
  loadingCurrent.value = true;
  try {
    const res = await privacyService.getCurrent();
    current.value = res.data;
  } catch (err) {
    console.error('Load error:', err);
    current.value = null;
  } finally {
    loadingCurrent.value = false;
  }
};

const formatDate = (date: string) => {
  return new Date(date).toLocaleString();
};

onMounted(() => {
  loadCurrent();
});
</script>

<style scoped>
.admin-privacy {
  max-width: 1000px;
  margin: 2rem auto;
  padding: 2rem;
}

h1 {
  margin-bottom: 2rem;
  color: #2c3e50;
}

.card {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

h2 {
  margin-top: 0;
  margin-bottom: 1.5rem;
  color: #34495e;
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #495057;
}

textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  font-size: 0.9rem;
  resize: vertical;
}

textarea:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
}

small {
  display: block;
  margin-top: 0.5rem;
  color: #6c757d;
}

button {
  background: #007bff;
  color: white;
  border: none;
  padding: 0.75rem 2rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
}

button:hover:not(:disabled) {
  background: #0056b3;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.message {
  margin-top: 1rem;
  padding: 1rem;
  border-radius: 4px;
  font-weight: 500;
}

.message.success {
  background: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.message.error {
  background: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.preview {
  margin-top: 1rem;
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 4px;
  border: 1px solid #dee2e6;
  max-height: 400px;
  overflow-y: auto;
}

.preview :deep(h2) {
  color: #2c3e50;
  margin-top: 1.5rem;
}

.preview :deep(h3) {
  color: #34495e;
  margin-top: 1rem;
}

.preview :deep(ul) {
  padding-left: 2rem;
}

.preview :deep(li) {
  margin-bottom: 0.5rem;
}
</style>
