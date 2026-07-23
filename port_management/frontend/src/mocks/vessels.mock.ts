import type { VesselRecordDto, VesselVisitNotificationDto } from '@/types/vessel'

/**
 * Mock data for Vessel Records
 * Used when backend is unavailable or returns empty data
 */
export const mockVessels: VesselRecordDto[] = [
  {
    id: 'vessel-1',
    imoValue: 'IMO9876543',
    name: 'MSC Aurora',
    vesselTypeId: 'container-ship',
    owner: 'Mediterranean Shipping Company',
    isActive: true,
    createdAt: new Date().toISOString()
  },
  {
    id: 'vessel-2',
    imoValue: 'IMO9876544',
    name: 'Maersk Explorer',
    vesselTypeId: 'container-ship',
    owner: 'Maersk Line',
    isActive: true,
    createdAt: new Date().toISOString()
  },
  {
    id: 'vessel-3',
    imoValue: 'IMO9876545',
    name: 'CMA CGM Marco Polo',
    vesselTypeId: 'container-ship',
    owner: 'CMA CGM',
    isActive: true,
    createdAt: new Date().toISOString()
  }
]

/**
 * Mock data for Vessel Visit Notifications (VVN)
 * Used when backend is unavailable or returns empty data
 */
export const mockVesselVisitNotifications: VesselVisitNotificationDto[] = [
  {
    id: 'vvn-1',
    businessId: 'VVN-2024-001',
    vesselId: 'vessel-1',
    shippingAgentRepresentativeId: 1,
    eta: new Date(Date.now() + 86400000).toISOString(),
    etd: new Date(Date.now() + 172800000).toISOString(),
    cargoManifestsId: ['CM-001'],
    crewId: 'CREW-001',
    status: 'Approved',
    assignedDockId: '1',
    approvedByOfficerId: 'OFFICER-01',
    approvedAt: new Date().toISOString(),
    createdAt: new Date().toISOString()
  },
  {
    id: 'vvn-2',
    businessId: 'VVN-2024-002',
    vesselId: 'vessel-2',
    shippingAgentRepresentativeId: 2,
    eta: new Date(Date.now() + 172800000).toISOString(),
    etd: new Date(Date.now() + 259200000).toISOString(),
    cargoManifestsId: ['CM-002'],
    crewId: 'CREW-002',
    status: 'Approved',
    assignedDockId: '2',
    approvedByOfficerId: 'OFFICER-01',
    approvedAt: new Date().toISOString(),
    createdAt: new Date().toISOString()
  },
  {
    id: 'vvn-3',
    businessId: 'VVN-2024-003',
    vesselId: 'vessel-3',
    shippingAgentRepresentativeId: 1,
    eta: new Date(Date.now() + 259200000).toISOString(),
    etd: new Date(Date.now() + 345600000).toISOString(),
    cargoManifestsId: ['CM-003'],
    crewId: 'CREW-003',
    status: 'Approved',
    assignedDockId: '3',
    approvedByOfficerId: 'OFFICER-01',
    approvedAt: new Date().toISOString(),
    createdAt: new Date().toISOString()
  }
]
