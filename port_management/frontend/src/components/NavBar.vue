<template>
  <header class="nav-header">
    <div class="nav-container">
      <router-link to="/dashboard" class="nav-logo">
        <div class="logo-icon">
          <Anchor :size="28" />
        </div>
        <span class="logo-text">Port Management</span>
      </router-link>

      <nav class="nav-menu" :class="{ 'mobile-open': mobileMenuOpen }">

        <div
            v-if="canViewManagement"
            class="nav-item"
            @mouseenter="handleMouseEnter('management')"
            @mouseleave="handleMouseLeave('management')"
            @click="toggleDropdown('management')"
        >
          <button class="nav-link">
            <Building2 :size="18" />
            <span>Management</span>
            <ChevronDown :size="16" class="chevron" />
          </button>
          <div class="dropdown" v-show="activeDropdown === 'management'">
            <router-link v-if="isAdmin || isPortOfficer || isLogistics" to="/dock/list" class="dropdown-item">
              <Ship :size="16" /> <span>Docks</span>
            </router-link>
            <router-link v-if="isAdmin || isPortOfficer" to="/organization/list" class="dropdown-item">
              <Building :size="16" /> <span>Organizations</span>
            </router-link>
            <router-link v-if="isAdmin || isPortOfficer" to="/representative/list" class="dropdown-item">
              <UserCheck :size="16" /> <span>Representatives</span>
            </router-link>
            <router-link v-if="isAdmin || isLogistics" to="/staff/list" class="dropdown-item">
              <Users :size="16" /> <span>Staff</span>
            </router-link>
            <router-link v-if="isAdmin || isLogistics" to="/qualification/list" class="dropdown-item">
              <Tag :size="18" /> <span>Qualification</span>
            </router-link>
          </div>
        </div>

        <div
            v-if="canViewVessels"
            class="nav-item"
            @mouseenter="handleMouseEnter('vessels')"
            @mouseleave="handleMouseLeave('vessels')"
            @click="toggleDropdown('vessels')"
        >
          <button class="nav-link">
            <Ship :size="18" />
            <span>Vessels</span>
            <ChevronDown :size="16" class="chevron" />
          </button>
          <div class="dropdown" v-show="activeDropdown === 'vessels'">
            <router-link to="/vessel-record/list" class="dropdown-item">
              <FileText :size="16" /> <span>Vessel Records</span>
            </router-link>
            <router-link v-if="isAdmin || isPortOfficer" to="/vessel-type/list" class="dropdown-item">
              <Tag :size="16" /> <span>Vessel Types</span>
            </router-link>
            <router-link to="/notification/list" class="dropdown-item">
              <List :size="16" /> <span>Visit Notifications</span>
            </router-link>
          </div>
        </div>

        <div
            v-if="canViewResources"
            class="nav-item"
            @mouseenter="handleMouseEnter('resources')"
            @mouseleave="handleMouseLeave('resources')"
            @click="toggleDropdown('resources')"
        >
          <button class="nav-link">
            <Package :size="18" />
            <span>Resources</span>
            <ChevronDown :size="16" class="chevron" />
          </button>
          <div class="dropdown" v-show="activeDropdown === 'resources'">
            <router-link to="/resource/list" class="dropdown-item">
              <Package :size="16" /> <span>Physical Resources</span>
            </router-link>
            <router-link to="/storage-area/list" class="dropdown-item">
              <Warehouse :size="16" /> <span>Storage Areas</span>
            </router-link>
            <router-link to="/3d" class="dropdown-item">
              <Box :size="16" /> <span>3D Visualization</span>
            </router-link>
          </div>
        </div>

        <div
            v-if="canViewOperations"
            class="nav-item"
            @mouseenter="handleMouseEnter('planning')"
            @mouseleave="handleMouseLeave('planning')"
            @click="toggleDropdown('planning')"
        >
          <button class="nav-link">
            <Calendar :size="18" />
            <span>Planning</span>
            <ChevronDown :size="16" class="chevron" />
          </button>
          <div class="dropdown" v-show="activeDropdown === 'planning'">
            <router-link to="/oem/operation-plans/list" class="dropdown-item">
              <ListChecks :size="16" /> <span>List Plans</span>
            </router-link>
            <router-link to="/oem/operation-plans/generate" class="dropdown-item">
              <FilePlus :size="16" /> <span>Generate Plan</span>
            </router-link>
            <router-link to="/oem/operation-plans/missing" class="dropdown-item">
              <Search :size="16" /> <span>Missing Plans</span>
            </router-link>
            <router-link to="/oem/operation-plans/allocation" class="dropdown-item">
              <BarChart3 :size="16" /> <span>Allocation Check</span>
            </router-link>

          </div>
        </div>

        <div
            v-if="canViewExecution"
            class="nav-item"
            @mouseenter="handleMouseEnter('execution')"
            @mouseleave="handleMouseLeave('execution')"
            @click="toggleDropdown('execution')"
        >
          <button class="nav-link">
            <Activity :size="18" />
            <span>Execution</span>
            <ChevronDown :size="16" class="chevron" />
          </button>
          <div class="dropdown" v-show="activeDropdown === 'execution'">
            <router-link to="/oem/vessel-visits" class="dropdown-item">
              <ClipboardList :size="16" /> <span>Visits Log (VVE)</span>
            </router-link>
            <router-link to="/oem/stats/resources" class="dropdown-item">
              <BarChart3 :size="16" /> <span>Resource Stats</span>
            </router-link>
            <router-link to="/oem/execution-monitor" class="dropdown-item">
              <MonitorPlay :size="16" /> <span>Monitor Ops</span>
            </router-link>
            <router-link to="/vve-manager" class="dropdown-item" style="color: #e67e22;">
                <Settings :size="16" /> <span>VVE Manager</span>
            </router-link>
          </div>
        </div>

        <div
            v-if="canViewIncidents"
            class="nav-item"
            @mouseenter="handleMouseEnter('incidents')"
            @mouseleave="handleMouseLeave('incidents')"
            @click="toggleDropdown('incidents')"
        >
          <button class="nav-link">
            <AlertTriangle :size="18" />
            <span>Incidents</span>
            <ChevronDown :size="16" class="chevron" />
          </button>
          <div class="dropdown" v-show="activeDropdown === 'incidents'">
            <router-link to="/oem/incident-reporting" class="dropdown-item">
              <AlertCircle :size="16" /> <span>Report Incident</span>
            </router-link>
            <router-link to="/oem/incident-types" class="dropdown-item">
              <Settings :size="16" /> <span>Incident Types</span>
            </router-link>
            <router-link to="/oem/complementary-tasks/categories" class="dropdown-item">
              <Layers :size="16" /> <span>Task Categories</span>
            </router-link>
          </div>
        </div>

        <router-link v-if="isAuthenticated" to="/dashboard" class="nav-link standalone">
          <LayoutDashboard :size="18" />
          <span>Dashboard</span>
        </router-link>
      </nav>

      <div class="nav-actions">
        <div
            v-if="isAuthenticated"
            class="nav-item user-menu"
            @mouseenter="handleMouseEnter('user')"
            @mouseleave="handleMouseLeave('user')"
            @click="toggleDropdown('user')"
        >
          <button class="user-button">
            <div class="user-avatar">
              <User :size="18" />
            </div>
            <ChevronDown :size="16" class="chevron" />
          </button>
          <div class="dropdown dropdown-right" v-show="activeDropdown === 'user'">
            <router-link v-if="isAdmin" to="/admin/users" class="dropdown-item">
              <Settings :size="16" /> <span>Admin Panel</span>
            </router-link>

            <!-- Privacy Policy Management - Admin only -->
            <router-link v-if="isAdmin" to="/admin/privacy" class="dropdown-item">
              <Shield :size="16" />
              <span>Privacy Policy</span>
            </router-link>
            
            <!-- Prolog Console: All users -->
            <router-link to="/prolog" class="dropdown-item">
              <Terminal :size="16" /> <span>Prolog Console</span>
            </router-link>
            <div class="dropdown-divider"></div>
            <a href="/logout" class="dropdown-item">
              <LogOut :size="16" /> <span>Sign Out</span>
            </a>
          </div>
        </div>

        <button class="mobile-toggle" @click="mobileMenuOpen = !mobileMenuOpen">
          <Menu v-if="!mobileMenuOpen" :size="24" />
          <X v-else :size="24" />
        </button>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed } from 'vue'
import {
  Anchor, Building2, Ship, ChevronDown, User, Menu, X,
  Building, UserCheck, Users, FileText, Tag, Package,
  Warehouse, Box, LayoutDashboard, Settings, Terminal, LogOut, Shield,
  // Novos ícones importados
  Calendar, ListChecks, FilePlus, Search, // Planning
  Activity, ClipboardList, BarChart3, MonitorPlay, // Execution
  AlertTriangle, AlertCircle, Layers // Incidents
  , List
} from 'lucide-vue-next'
import { useUserStore } from '@/services/useUserStore'

const activeDropdown = ref(null)
const mobileMenuOpen = ref(false)
const pinnedDropdown = ref(null)

const handleMouseEnter = (menu) => {
  if (pinnedDropdown.value !== menu) {
    activeDropdown.value = menu
  }
}

const handleMouseLeave = (menu) => {
  if (pinnedDropdown.value !== menu) {
    activeDropdown.value = null
  }
}

const toggleDropdown = (menu) => {
  if (pinnedDropdown.value === menu) {
    pinnedDropdown.value = null
    activeDropdown.value = null
  } else {
    pinnedDropdown.value = menu
    activeDropdown.value = menu
  }
}

// User store
const userStore = useUserStore()
const isAuthenticated = computed(() => userStore.isAuthenticated())

// Role checks
const isAdmin = computed(() => userStore.hasRole('Administrator'))
const isPortOfficer = computed(() => userStore.hasRole('PortAuthorityOfficer'))
const isLogistics = computed(() => userStore.hasRole('LogisticsOperator'))
const isShippingAgent = computed(() => userStore.hasRole('ShippingAgentRepresentative'))

// --- Permissões dos Menus Existentes ---
const canViewManagement = computed(() =>
    isAdmin.value || isPortOfficer.value || isLogistics.value
)
const canViewVessels = computed(() =>
    isAdmin.value || isPortOfficer.value || isShippingAgent.value
)
const canViewResources = computed(() =>
    isAdmin.value || isPortOfficer.value || isLogistics.value
)

// --- Novas Permissões (Baseadas no router index.ts) ---
// Estas rotas estão protegidas para 'Administrator' e 'LogisticsOperator' apenas.

const canViewOperations = computed(() =>
    isAdmin.value || isLogistics.value
)

const canViewExecution = computed(() =>
    isAdmin.value || isLogistics.value
)

const canViewIncidents = computed(() =>
    isAdmin.value || isLogistics.value
)
</script>

<style scoped>
.nav-header {
  background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
  border-bottom: 1px solid #e2e8f0;
  box-shadow: 0 2px 12px rgba(15, 23, 42, 0.06);
  position: sticky;
  top: 0;
  z-index: 1000;
  backdrop-filter: blur(8px);
}

.nav-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1.5rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 72px;
  gap: 2rem;
}

/* Logo Section */
.nav-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-weight: 700;
  font-size: 1.25rem;
  color: #1e3a5f;
  cursor: pointer;
  transition: transform 0.2s;
  text-decoration: none;
}

.nav-logo:hover {
  transform: translateY(-1px);
}

.logo-icon {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.25);
}

.logo-text {
  background: linear-gradient(135deg, #1e3a5f 0%, #2563eb 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Navigation Menu */
.nav-menu {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
}

.nav-item {
  position: relative;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  color: #475569;
  font-weight: 500;
  font-size: 0.95rem;
  text-decoration: none;
  border-radius: 8px;
  transition: all 0.2s;
  background: transparent;
  border: none;
  cursor: pointer;
  white-space: nowrap;
}

.nav-link:hover {
  background: #f1f5f9;
  color: #2563eb;
  transform: translateY(-1px);
}

.nav-link.standalone {
  background: linear-gradient(135deg, #dbeafe 0%, #eff6ff 100%);
  color: #1e40af;
  border: 1px solid #bfdbfe;
}

.nav-link.standalone:hover {
  background: linear-gradient(135deg, #bfdbfe 0%, #dbeafe 100%);
  box-shadow: 0 2px 8px rgba(37, 99, 235, 0.15);
}

.chevron {
  transition: transform 0.2s;
}

.nav-item:hover .chevron {
  transform: rotate(180deg);
}

/* Dropdown Menu */
.dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  background: white;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(15, 23, 42, 0.12);
  padding: 0.5rem;
  min-width: 220px;
  border: 1px solid #e2e8f0;
  animation: dropdownFade 0.2s ease-out;
}

.dropdown-right {
  left: auto;
  right: 0;
}

@keyframes dropdownFade {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  color: #475569;
  text-decoration: none;
  border-radius: 8px;
  transition: all 0.15s;
  font-size: 0.9rem;
}

.dropdown-item:hover {
  background: linear-gradient(135deg, #eff6ff 0%, #dbeafe 100%);
  color: #2563eb;
  transform: translateX(4px);
}

.dropdown-divider {
  height: 1px;
  background: #e2e8f0;
  margin: 0.5rem 0;
}

/* User Section */
.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  background: transparent;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.user-button:hover {
  background: #f1f5f9;
}

.user-avatar {
  width: 38px;
  height: 38px;
  background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 2px 8px rgba(37, 99, 235, 0.25);
}

/* Mobile Toggle */
.mobile-toggle {
  display: none;
  background: transparent;
  border: none;
  color: #475569;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 8px;
  transition: all 0.2s;
}

.mobile-toggle:hover {
  background: #f1f5f9;
  color: #2563eb;
}

/* Responsive Design */
@media (max-width: 968px) {
  .nav-container {
    height: 64px;
  }

  .mobile-toggle {
    display: block;
  }

  .nav-menu {
    position: fixed;
    top: 64px;
    left: 0;
    right: 0;
    background: white;
    flex-direction: column;
    align-items: stretch;
    padding: 1.5rem;
    gap: 0.75rem;
    border-bottom: 1px solid #e2e8f0;
    box-shadow: 0 8px 24px rgba(15, 23, 42, 0.12);
    transform: translateY(-100%);
    opacity: 0;
    pointer-events: none;
    transition: all 0.3s ease-out;
  }

  .nav-menu.mobile-open {
    transform: translateY(0);
    opacity: 1;
    pointer-events: all;
  }

  .nav-item {
    width: 100%;
  }

  .nav-link {
    width: 100%;
    justify-content: space-between;
  }

  .dropdown {
    position: static;
    box-shadow: none;
    border: 1px solid #f1f5f9;
    margin-top: 0.5rem;
    background: #f9fafb;
  }

  .user-menu {
    display: none;
  }
}

@media (max-width: 480px) {
  .logo-text {
    display: none;
  }

  .nav-container {
    padding: 0 1rem;
  }
}
</style>
