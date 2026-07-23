// Vessel DTOs and Types

export interface VesselRecordDto {
  id?: string
  imoValue: string
  name: string
  vesselTypeId: string
  owner: string
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}

export interface VesselVisitNotificationDto {
  id?: string
  businessId: string
  vesselId?: string
  shippingAgentRepresentativeId?: number
  eta?: string
  etd?: string
  cargoManifestsId?: string[]
  crewId?: string
  status?: string // "Pending", "Approved", "Rejected"
  approvedByOfficerId?: string
  rejectedByOfficerId?: string
  approvedAt?: string
  rejectedAt?: string
  approvalNotes?: string
  assignedDockId?: string
  rejectionReason?: string
  createdAt?: string
  updatedAt?: string
}

