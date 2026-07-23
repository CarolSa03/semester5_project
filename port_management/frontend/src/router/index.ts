import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import NotFoundView from '../views/NotFoundView.vue'
import PhysicalResourcesView from '../views/PhysicalResourcesView.vue'
import PortVisualization from '../views/PortVisualization.vue'
import Login from '../views/Login.vue'
import Dashboard from '../views/Dashboard.vue'
import Activate from '../views/Activate.vue'
import AdminUserManagement from '../views/AdminUserManagement.vue'
import DockCreate from '../views/docks/DockCreate.vue'
import DockEdit from '../views/docks/DockEdit.vue'
import DockList from '../views/docks/DockList.vue'
import OrganizationCreate from '@/views/organisation/OrganizationCreate.vue'
import OrganizationDetail from '@/views/organisation/OrganizationDetail.vue'
import OrganizationEdit from '@/views/organisation/OrganizationEdit.vue'
import OrganizationList from '@/views/organisation/OrganizationList.vue'
import RepresentativeCreate from '@/views/representative/RepresentativeCreate.vue'
import RepresentativeDetail from '@/views/representative/RepresentativeDetail.vue'
import RepresentativeEdit from '@/views/representative/RepresentativeEdit.vue'
import RepresentativeList from '@/views/representative/RepresentativeList.vue'
import StaffCreate from '@/views/staff/StaffCreate.vue'
import StaffDetail from '@/views/staff/StaffDetail.vue'
import StaffEdit from '@/views/staff/StaffEdit.vue'
import StaffList from '@/views/staff/StaffList.vue'
import VesselRecordCreate from '@/views/vessel/VesselRecordCreate.vue'
import VesselRecordDetail from '@/views/vessel/VesselRecordDetail.vue'
import VesselRecordEdit from '@/views/vessel/VesselRecordEdit.vue'
import VesselRecordList from '@/views/vessel/VesselRecordList.vue'
import VesselTypeCreate from '@/views/vessel/VesselTypeCreate.vue'
import VesselTypeDetail from '@/views/vessel/VesselTypeDetail.vue'
import VesselTypeEdit from '@/views/vessel/VesselTypeEdit.vue'
import VesselTypeList from '@/views/vessel/VesselTypeList.vue'
import StorageAreaCreate from '@/views/storage/StorageAreaCreate.vue'
import StorageAreaEdit from '@/views/storage/StorageAreaEdit.vue'
import StorageAreaDetail from '@/views/storage/StorageAreaDetail.vue'
import StorageAreaList from '@/views/storage/StorageAreaList.vue'
import PrologView from '@/views/PrologView.vue'
import PhysicalResourceCreate from '@/views/physicalresource/PhysicalResourceCreate.vue'
import PhysicalResourceDetail from '@/views/physicalresource/PhysicalResourceDetail.vue'
import PhysicalResourceEdit from '@/views/physicalresource/PhysicalResourceEdit.vue'
import PhysicalResourceList from '@/views/physicalresource/PhysicalResourceList.vue'
import QualificationCreate from '@/views/qualification/QualificationCreate.vue'
import QualificationDetail from '@/views/qualification/QualificationDetail.vue'
import QualificationEdit from '@/views/qualification/QualificationEdit.vue'
import QualificationList from '@/views/qualification/QualificationList.vue'
import VesselNotificationCreate from '@/views/vessel/VesselNotificationCreate.vue'
import { useUserStore } from '@/services/useUserStore'
import NotAuthorized from '@/views/vessel/NotAuthorized.vue'
import Logout from '@/views/Logout.vue'
import GenerateOperationPlanView from '@/view/operationPlan/GenerateOperationPlanView.vue'
import OperationPlanListView from '@/view/operationPlan/OperationPlanListView.vue'
import UpdateOperationPlanView from '@/view/operationPlan/UpdateOperationPlanView.vue'
import MissingOperationPlansView from '@/view/operationPlan/MissingOperationPlansView.vue'
import PrivacyPage from '@/views/Privacy/view/PrivacyPage.vue'
import AdminPrivacyPolicy from '@/views/admin/AdminPrivacyPolicy.vue'
import MonitorExecutionView from "@/view/vesselVE/MonitorExecutionView.vue";
import ReportIncidentView from "@/view/incident/ReportIncidentView.vue";
import IncidentTypeView from "@/view/incident/IncidentTypeView.vue";
import ResourceStatsView from "@/view/stats/ResourceStatsView.vue";
import VesselVisitListView from "@/view/vesselVE/VesselVisitListView.vue";
import VesselVisitManagerView from "@/view/vesselVE/VesselVisitManagerView.vue";
import ManageTaskCategoriesView from "@/view/complementaryTask/ManageTaskCategoriesView.vue";
import ResourceAllocationView from '@/view/operationPlan/ResourceAllocationView.vue';
import CompleteVveView from '@/view/vesselVE/CompleteVveView.vue';
import VveManagerView from '@/view/vesselVE/VveManagerView.vue';
import VesselNotificationList from '@/views/vessel/VesselNotificationList.vue'
import VesselNotificationDetail from '@/views/vessel/VesselNotificationDetail.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/login'
  },

  {
    path: '/privacy',
    name: 'privacy',
    component: PrivacyPage,
    meta: { requiresAuth: false }
  },

  {
    path: '/admin/privacy',
    name: 'admin-privacy',
    component: AdminPrivacyPolicy,
    meta: { requiresAuth: true, roles: ['Administrator'] }
  },

  {
    path: '/privacy',
    name: 'privacy',
    component: PrivacyPage,
    meta: { requiresAuth: false }
  },

  {
    path: '/admin/privacy',
    name: 'admin-privacy',
    component: AdminPrivacyPolicy,
    meta: { requiresAuth: true, roles: ['Administrator'] }
  },

  {
    path: '/oem/vve-manager',
    name: 'vve-manager',
    component: VveManagerView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/vve/:id/complete',
    name: 'complete-vve',
    component: CompleteVveView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/operation-plans/allocation',
    name: 'resource-allocation',
    component: ResourceAllocationView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/operation-plans/generate',
    name: 'operation-plan-generate',
    component: GenerateOperationPlanView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/operation-plans/list',
    name: 'operation-plan-list',
    component: OperationPlanListView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },

  {
    path: '/oem/operation-plans/:id',
    name: 'operation-plan-detail',
    // Usamos import dinâmico para não ter de adicionar imports no topo do ficheiro
    component: () => import('@/view/operationPlan/OperationPlanDetailView.vue'),
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },

  {
    path: '/oem/operation-plans/missing',
    name: 'operation-plans-missing',
    component: MissingOperationPlansView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/operation-plans/:id/edit',
    name: 'operation-plan-edit',
    component: UpdateOperationPlanView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/execution-monitor',
    name: 'execution-monitor',
    component: MonitorExecutionView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/incident-types',
    name: 'incident-types',
    component: IncidentTypeView,
    meta: {
      title: 'Incident Types Catalog', requiresAuth: true, roles: ['Administrator', 'LogisticsOperator']
    }
  },
  {
    path: '/oem/incident-reporting',
    name: 'incident-reporting',
    component: ReportIncidentView,
    meta: {
      title: 'Report Incidents', requiresAuth: true, roles: ['Administrator', 'LogisticsOperator']
    }
  },
  {
    path: '/oem/stats/resources',
    name: 'stats-resources',
    component: ResourceStatsView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/vessel-visits',
    name: 'vessel-visits',
    component: VesselVisitListView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/oem/vessel-visits/:id',
    name: 'vessel-visits-detail',
    component: VesselVisitManagerView,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/complementary-tasks/categories',
    name: 'task-categories',
    component: ManageTaskCategoriesView,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortOperationsSupervisor', 'LogisticsOperator'] }
  },

  // Public routes (no authentication required)
  {
    path: '/login',
    name: 'login',
    component: Login
  },
  {
    path: '/activate',
    name: 'activate',
    component: Activate
  },

  // Dashboard (all authenticated users)
  {
    path: '/dashboard',
    name: 'dashboard',
    component: Dashboard,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'ShippingAgentRepresentative', 'LogisticsOperator'] }
  },

  // Admin routes (Administrator only)
  {
    path: '/admin/users',
    name: 'admin-users',
    component: AdminUserManagement,
    //    meta: { requiresAuth: true, roles: ['Administrator'] }
  },

  // Port Authority Officer routes
  {
    path: '/dock/create',
    name: 'dock-create',
    component: DockCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/dock/list',
    name: 'dock-list',
    component: DockList,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'LogisticsOperator'] }
  },
  {
    path: '/dock/edit/:id',
    name: 'dock-edit',
    component: DockEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },

  {
    path: '/vessel-record/create',
    name: 'vessel-record-create',
    component: VesselRecordCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/vessel-record/list',
    name: 'vessel-record-list',
    component: VesselRecordList,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'ShippingAgentRepresentative'] }
  },
  {
    path: '/vessel-record/edit/:id',
    name: 'vessel-record-edit',
    component: VesselRecordEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/vessel-record/:id',
    name: 'vessel-record-detail',
    component: VesselRecordDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'ShippingAgentRepresentative'] }
  },

  {
    path: '/vessel-type/create',
    name: 'vessel-type-create',
    component: VesselTypeCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/vessel-type/list',
    name: 'vessel-type-list',
    component: VesselTypeList,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/vessel-type/:id/edit',
    name: 'vessel-type-edit',
    component: VesselTypeEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/vessel-type/:id',
    name: 'vessel-type-detail',
    component: VesselTypeDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },

  {
    path: '/storage-area/create',
    name: 'storage-area-create',
    component: StorageAreaCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/storage-area/list',
    name: 'storage-area-list',
    component: StorageAreaList,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'LogisticsOperator'] }
  },
  {
    path: '/storage-area/edit/:id',
    name: 'storage-area-edit',
    component: StorageAreaEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/storage-area/:id',
    name: 'storage-area-detail',
    component: StorageAreaDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'LogisticsOperator'] }
  },

  // Shipping Agent routes
  {
    path: '/organization/create',
    name: 'organization-create',
    component: OrganizationCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/organization/list',
    name: 'organization-list',
    component: OrganizationList,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/organization/:id/edit',
    name: 'organization-edit',
    component: OrganizationEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/organization/:id',
    name: 'organization-detail',
    component: OrganizationDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },

  {
    path: '/representative/create',
    name: 'representative-create',
    component: RepresentativeCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/representative/list',
    name: 'representative-list',
    component: RepresentativeList,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/representative/:id/edit',
    name: 'representative-edit',
    component: RepresentativeEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/representative/:id',
    name: 'representative-detail',
    component: RepresentativeDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer'] }
  },

  // Vessel Visit Notifications (Shipping Agent creates, Port Authority approves)
  {
    path: '/notification/create',
    name: 'notification-create',
    component: VesselNotificationCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'ShippingAgentRepresentative'] }
  },

  {
    path: '/notification/list',
    name: 'notification-list',
    component: VesselNotificationList,
    meta: { requiresAuth: true, roles: ['Administrator', 'ShippingAgentRepresentative', 'PortAuthorityOfficer'] }
  },

  {
    path: '/notification/:businessId',
    name: 'notification-detail',
    component: () => VesselNotificationDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'ShippingAgentRepresentative', 'PortAuthorityOfficer'] }
  },

  // Logistics Operator routes
  {
    path: '/staff/create',
    name: 'staff-create',
    component: StaffCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/staff/list',
    name: 'staff-list',
    component: StaffList,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/staff/:id/edit/',
    name: 'staff-edit',
    component: StaffEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/staff/:id',
    name: 'staff-detail',
    component: StaffDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },

  {
    path: '/resource/create',
    name: 'physical-resource-create',
    component: PhysicalResourceCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/resource/list',
    name: 'physical-resource-list',
    component: PhysicalResourceList,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator', 'PortAuthorityOfficer'] }
  },
  {
    path: '/resource/:id/edit',
    name: 'physical-resource-edit',
    component: PhysicalResourceEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/resource/:id',
    name: 'physical-resource-detail',
    component: PhysicalResourceDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator', 'PortAuthorityOfficer'] }
  },

  {
    path: '/qualification/create',
    name: 'qualification-create',
    component: QualificationCreate,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/qualification/list',
    name: 'qualification-list',
    component: QualificationList,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/qualification/:id/edit',
    name: 'qualification-edit',
    component: QualificationEdit,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },
  {
    path: '/qualification/:id',
    name: 'qualification-detail',
    component: QualificationDetail,
    meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },

  // Shared routes (multiple roles)
  {
    path: '/resources',
    name: 'resources',
    component: PhysicalResourcesView,
    meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'LogisticsOperator'] }
  },
  {
    path: '/3d',
    name: 'PortVisualization',
    component: PortVisualization,
    // meta: { requiresAuth: true, roles: ['Administrator', 'PortAuthorityOfficer', 'LogisticsOperator'] }
  },
  {
    path: '/prolog',
    name: 'prolog',
    component: PrologView,
    //meta: { requiresAuth: true, roles: ['Administrator', 'LogisticsOperator'] }
  },

  // 404
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: NotFoundView
  },
  {
    path: '/not-authorized',
    name: 'not-authorized',
    component: NotAuthorized
  },
  {
    path: '/logout',
    name: 'logout',
    component: Logout
  }

]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to, from, next) => {
  const userStore = useUserStore()
  let user = userStore.currentUser.value
  if (!user) {
    user = await userStore.fetchCurrentUser()
  }
  const requiresAuth = to.meta.requiresAuth
  const allowedRoles = to.meta.roles as string[] | undefined

  if (!requiresAuth) {
    return next()
  }

  const { currentUser, fetchCurrentUser, hasRole } = useUserStore()

  try {
    let user = currentUser.value
    if (!user) {
      user = await fetchCurrentUser()
    }

    if (!user) {
      return next({ name: 'login' })
    }

    if (!user.isActive) {
      alert('Your account is not active. Please contact an administrator.')
      return next({ name: 'dashboard' })
    }

    if (hasRole('Administrator')) {
      return next()
    }
    if (allowedRoles && allowedRoles.length > 0) {
      const hasAccess = allowedRoles.some(role => hasRole(role))

      if (hasAccess) {
        return next()
      } else {
        return next({ name: 'not-authorized' })
      }
    }
    return next()

  } catch (error) {
    console.error('Error in navigation guard:', error)
    return next({ name: 'login' })
  }
})

export default router
