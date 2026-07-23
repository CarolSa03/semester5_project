<script setup lang="ts">
import { onMounted, ref } from 'vue'
import Navbar from './components/NavBar.vue'
import { useUserStore } from '@/services/useUserStore'
import privacyService from '@/services/privacyService'
import PrivacyModal from '@/components/PrivacyModal.vue'
import AppFooter from '@/components/AppFooter.vue'

const userStore = useUserStore()
const show = ref(false)
const content = ref('')

onMounted(async () => {
  await userStore.fetchCurrentUser()
  
  if (userStore.currentUser.value) {
    try {
      const check = await privacyService.checkNotification()
      if (check.data.needsNotification) {
        const policy = await privacyService.getCurrent()
        content.value = policy.data.content
        show.value = true
      }
    } catch (err) {
      console.error('Privacy check failed:', err)
    }
  }
})
</script>

<template>
  <div class="app">
    <header class="app-header">
      <Navbar />
    </header>

    <main class="app-main">
      <router-view />
    </main>

    <AppFooter />
    <PrivacyModal :show="show" :content="content" @close="show = false" />
  </div>
</template>
<style>
.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  padding-bottom: 0px; 
}

.app-main {
  flex: 1;
}

.app-footer {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  width: 100%;
  background: #2c3e50;
  color: white;
  padding: 1rem;
  z-index: 9998;
  box-shadow: 0 -2px 5px rgba(0,0,0,0.1);
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1rem;
}

.footer-content p {
  margin: 0;
  font-size: 0.9rem;
}

.footer-content a {
  color: white;
  text-decoration: none;
  font-weight: 500;
}

.footer-content a:hover {
  text-decoration: underline;
}

@media (max-width: 768px) {
  .footer-content {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>