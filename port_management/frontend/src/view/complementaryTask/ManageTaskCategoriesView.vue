<template>
  <div class="page-container">
    <h1>Task Categories</h1>
    <p class="subtitle">US 4.1.14 - Manage complementary task catalog</p>

    <section class="card">
      <div class="card-header">
        <h2>Categories List</h2>
        <button class="btn-primary" @click="viewModel.openCreateModal()">+ Create Category</button>
      </div>
      <div class="table-container">
        <table class="modern-table">
          <thead><tr><th>Code</th><th>Name</th><th>Description</th></tr></thead>
          <tbody>
          <tr v-for="cat in viewModel.categories.value" :key="cat.code">
            <td class="font-mono">{{ cat.code }}</td>
            <td>{{ cat.name }}</td>
            <td>{{ cat.description || '-' }}</td>
          </tr>
          </tbody>
        </table>
      </div>
    </section>

    <div v-if="viewModel.showModal.value" class="modal-overlay">
      <div class="modal-card">
        <div class="modal-header"><h3>New Task Category</h3><button class="close-btn" @click="viewModel.showModal.value=false">✕</button></div>
        <div class="modal-body">
          <form @submit.prevent="viewModel.createCategory()">
            <div class="input-group">
              <label>Code *</label>
              <input v-model="viewModel.newCategory.value.code" class="form-input" required />
            </div>
            <div class="input-group">
              <label>Name *</label>
              <input v-model="viewModel.newCategory.value.name" class="form-input" required />
            </div>
            <div class="input-group">
              <label>Description</label>
              <textarea v-model="viewModel.newCategory.value.description" class="form-input"></textarea>
            </div>
            <div class="actions-row">
              <button class="btn-primary" type="submit">Create</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ManageTaskCategoriesViewModel } from '@/viewmodels/complementaryTask/ManageTaskCategoriesViewModel';
const viewModel = new ManageTaskCategoriesViewModel();
</script>

<style scoped>
/* Reutilização dos mesmos estilos globais das views anteriores */
.page-container { max-width: 1200px; margin: 0 auto; padding: 2rem; font-family: 'Inter', sans-serif; color: #374151; }
.card { background: white; border-radius: 12px; padding: 1.5rem; margin-bottom: 2rem; border: 1px solid #e5e7eb; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
.card-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #f3f4f6; padding-bottom: 1rem; margin-bottom: 1rem; }
.modern-table { width: 100%; border-collapse: collapse; }
.modern-table th { text-align: left; padding: 0.75rem; border-bottom: 1px solid #eee; }
.modern-table td { padding: 0.75rem; border-bottom: 1px solid #f9fafb; }
.btn-primary { background: #2563eb; color: white; padding: 0.75rem 1.5rem; border-radius: 0.5rem; border: none; cursor: pointer; }
.form-input { width: 100%; padding: 0.75rem; border: 1px solid #d1d5db; border-radius: 0.5rem; margin-bottom: 1rem; }
.modal-overlay { position: fixed; top:0; left:0; right:0; bottom:0; background:rgba(0,0,0,0.5); display:flex; justify-content:center; align-items:center; z-index:50; }
.modal-card { background:white; width:500px; padding:1.5rem; border-radius:12px; }
.font-mono { font-family: monospace; color: #4b5563; }
</style>