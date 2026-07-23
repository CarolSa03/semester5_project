import apiClient from './apiClient'
import type { VesselRecordDto, VesselVisitNotificationDto } from '@/types/vessel'
import { mockVessels, mockVesselVisitNotifications } from '@/mocks'

/**
 * Get all vessel records
 * GET /api/VesselRecord
 */
export async function getAllVessels(): Promise<VesselRecordDto[]> {
  try {
    const response = await apiClient.get<VesselRecordDto[]>('/VesselRecord')
    // If backend returns empty array, use mocks
    if (!response.data || response.data.length === 0) {
      console.warn('Backend returned empty data, using mock vessels data')
      return mockVessels
    }
    return response.data
  } catch (error) {
    console.warn('Backend not available, using mock vessels data')
    return mockVessels
  }
}

/**
 * Get vessel by ID
 * GET /api/VesselRecord/{id}
 */
export async function getVesselById(id: string): Promise<VesselRecordDto> {
  try {
    const response = await apiClient.get<VesselRecordDto>(`/VesselRecord/${id}`)
    return response.data
  } catch (error) {
    console.warn(`Backend not available, using mock vessel data for ${id}`)
    const vessel = mockVessels.find(v => v.id === id)
    if (vessel) return vessel
    throw new Error(`Vessel ${id} not found`)
  }
}

/**
 * Get all vessel visit notifications
 * GET /api/vessel-visit-notifications
 */
export async function getAllVesselVisitNotifications(): Promise<VesselVisitNotificationDto[]> {
  try {
    const response = await apiClient.get<VesselVisitNotificationDto[]>('/vessel-visit-notifications')
    // If backend returns empty array, use mocks
    if (!response.data || response.data.length === 0) {
      console.warn('Backend returned empty data, using mock VVNs data')
      return mockVesselVisitNotifications
    }
    return response.data
  } catch (error) {
    console.warn('Backend not available, using mock VVNs data')
    return mockVesselVisitNotifications
  }
}

/**
 * Get approved vessel visit notifications only
 * US 3.3.3: Consider only approved VVNs
 */
export async function getApprovedVesselVisits(): Promise<VesselVisitNotificationDto[]> {
  const all = await getAllVesselVisitNotifications()
  return all.filter(vvn => vvn.status === 'Approved')
}

/**
 * Get vessel visit notifications by status
 */
export async function getVesselVisitsByStatus(status: string): Promise<VesselVisitNotificationDto[]> {
  const all = await getAllVesselVisitNotifications()
  return all.filter(vvn => vvn.status === status)
}

/**
 * Create vessel visit notification
 * POST /api/vessel-visit-notifications
 */
export async function createVesselVisitNotification(dto: VesselVisitNotificationDto): Promise<VesselVisitNotificationDto> {
  const response = await apiClient.post<VesselVisitNotificationDto>('/vessel-visit-notifications', dto)
  return response.data
}

