<template>
  <div class="incident-type-container">
    <h1>Incident Types</h1>
    <p class="subtitle">US 4.1.12 - Manage standardized incident classifications</p>

    <div class="filter-card" style="display: flex; justify-content: space-between; align-items: center;">
      <div>
        <button class="btn-secondary" @click="viewModel.loadTypes()" :disabled="viewModel.isLoading.value">
          Refresh
        </button>
      </div>
      <div>
        <button class="btn-primary" @click="viewModel.openCreateModal()">
          + Create New Type
        </button>
      </div>
    </div>

    <div class="results-card">
      <div v-if="viewModel.isLoading.value" class="loading-state">
        <div class="spinner-large"></div>
        <p>Loading types...</p>
      </div>

      <table v-else class="plans-table">
        <thead>
        <tr>
          <th>Code</th>
          <th>Name</th>
          <th>Severity</th>
          <th>Parent Category</th>
          <th>Description</th>
          <th>Actions</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="type in viewModel.incidentTypes.value" :key="type.id">
          <td><strong>{{ type.code }}</strong></td>
          <td>{{ type.name }}</td>
          <td>
              <span :class="'urgency-badge urgency-' + (type.severity ? type.severity.toLowerCase() : 'unknown')">
                {{ type.severity|| 'N/A' }}
              </span>
          </td>
          <td>{{ viewModel.getParentName(type.parentId) }}</td>
          <td>{{ type.description }}</td>
          <td>
            <button class="btn-secondary" @click="viewModel.removeType(type.id)">Delete</button>
          </td>
        </tr>
        <tr v-if="viewModel.incidentTypes.value.length === 0">
          <td colspan="6" class="empty-state">No incident types defined.</td>
        </tr>
        </tbody>
      </table>
    </div>

    <div v-if="viewModel.showCreateModal.value" class="modal-overlay">
      <div class="modal-content">
        <div class="modal-header">
          <h2>Create Incident Type</h2>
          <button class="close-btn" @click="viewModel.showCreateModal.value = false">✕</button>
        </div>

        <div class="modal-body">
          <form @submit.prevent="viewModel.createType()">
            <div class="form-group">
              <label>Code *</label>
              <input v-model="viewModel.newType.value.code" placeholder="e.g. ENV-FOG" required />
            </div>

            <div class="form-group">
              <label>Name *</label>
              <input v-model="viewModel.newType.value.name" placeholder="e.g. Fog" required />
            </div>

            <div class="form-group">
              <label>Severity *</label>
              <select v-model="viewModel.newType.value.severity">
                <option value="Minor">Minor</option>
                <option value="Major">Major</option>
                <option value="Critical">Critical</option>
              </select>
            </div>

            <div class="form-group">
              <label>Parent Category</label>
              <select v-model="viewModel.newType.value.parentId">
                <option value="">(None - Root Category)</option>
                <option v-for="p in viewModel.parentOptions.value" :key="p.id" :value="p.id">
                  {{ p.name }} ({{ p.code }})
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>Description *</label>
              <textarea v-model="viewModel.newType.value.description" rows="3" required></textarea>
            </div>

            <div v-if="viewModel.errorMessage.value" class="error-message">
              {{ viewModel.errorMessage.value }}
            </div>

            <div class="button-group" style="justify-content: flex-end; margin-top: 2rem;">
              <button type="button" class="btn-secondary" @click="viewModel.showCreateModal.value = false">Cancel</button>
              <button type="submit" class="btn-primary" :disabled="viewModel.isLoading.value || !viewModel.isFormValid.value">
                <span v-if="viewModel.isLoading.value" class="spinner"></span>
                {{ viewModel.isLoading.value ? 'Creating...' : 'Create' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { IncidentTypeViewModel } from '@/viewmodels/incident/IncidentTypeViewModel';

const viewModel = new IncidentTypeViewModel();
onMounted(() => viewModel.loadTypes());
</script>

<style scoped>
/* Reutilizando classes comuns */
.incident-type-container { max-width: 1400px; margin: 0 auto; padding: 2rem; }
h1 { color: #2c3e50; margin-bottom: 0.5rem; }
.subtitle { color: #7f8c8d; margin-bottom: 2rem; }

.filter-card, .results-card {
  background: white; border-radius: 8px; padding: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1); margin-bottom: 2rem;
}

.plans-table { width: 100%; border-collapse: collapse; font-size: 0.95rem; }
.plans-table thead { background-color: #34495e; color: white; }
.plans-table th, .plans-table td { padding: 1rem; text-align: left; border-bottom: 1px solid #ddd; }
.plans-table tbody tr:hover { background-color: #f8f9fa; }

/* Badges adaptados para Severidade usando cores do MissingOperationPlansView */
.urgency-badge { display: inline-block; padding: 0.25rem 0.75rem; border-radius: 12px; font-size: 0.85rem; font-weight: bold; }
.urgency-minor { background-color: #3498db; color: white; }
.urgency-major { background-color: #f39c12; color: white; }
.urgency-critical { background-color: #e74c3c; color: white; }

/* Forms e Botões */
.form-group { margin-bottom: 1.5rem; }
.form-group label { display: block; margin-bottom: 0.5rem; font-weight: 600; color: #2c3e50; }
.form-group input, .form-group select, .form-group textarea {
  width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 4px; font-size: 1rem;
}
.form-group input:focus { outline: none; border-color: #3498db; }

.btn-primary {
  background-color: #3498db; color: white; padding: 0.75rem 1.5rem; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer; display: flex; align-items: center; gap: 0.5rem;
}
.btn-secondary {
  background-color: #95a5a6; color: white; padding: 0.75rem 1.5rem; border: none; border-radius: 4px; font-size: 1rem; cursor: pointer;
}

/* Modal */
.modal-overlay {
  position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0, 0, 0, 0.5); display: flex; align-items: center; justify-content: center; z-index: 1000;
}
.modal-content {
  background: white; border-radius: 8px; width: 600px; max-height: 90vh; overflow: auto; box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
}
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 1.5rem; border-bottom: 1px solid #eee; }
.modal-body { padding: 1.5rem; }
.close-btn { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #95a5a6; }

.error-message { padding: 1rem; border-radius: 4px; margin-top: 1rem; background-color: #fee; color: #c33; border: 1px solid #fcc; }
.loading-state { text-align: center; padding: 3rem; color: #7f8c8d; }
.spinner-large { width: 40px; height: 40px; border: 4px solid rgba(52, 152, 219, 0.3); border-top-color: #3498db; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 1rem; }
.spinner { width: 16px; height: 16px; border: 2px solid rgba(255, 255, 255, 0.3); border-top-color: white; border-radius: 50%; animation: spin 0.6s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
</style>