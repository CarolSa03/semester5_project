<template>
  <div v-if="show" class="modal">
    <div class="box">
      <h2>Privacy Policy Updated</h2>
      <div v-html="content"></div>
      <button @click="ok">I Understand</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import privacyService from '@/services/privacyService';

defineProps<{
  show: boolean;
  content: string;
}>();

const emit = defineEmits<{ close: [] }>();

const ok = async () => {
  await privacyService.markViewed();
  emit('close');
};
</script>

<style scoped>
.modal {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}
.box {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  max-width: 600px;
  max-height: 80vh;
  overflow-y: auto;
}
button {
  margin-top: 1rem;
  padding: 0.5rem 1.5rem;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
button:hover {
  background: #0056b3;
}
</style>
