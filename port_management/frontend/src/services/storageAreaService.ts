import apiClient from './apiClient'
import type { StorageAreaDto } from '@/types/storageArea'
import { mockStorageAreas } from '@/mocks'

/**
 * Get all storage areas (yards and warehouses)
 * GET /api/StorageArea
 */
export async function getAllStorageAreas(): Promise<StorageAreaDto[]> {
  try {
    const response = await apiClient.get<StorageAreaDto[]>('/StorageArea')
    // If backend returns empty array, use mocks
    if (!response.data || response.data.length === 0) {
      console.warn('Backend returned empty data, using mock storage areas data')
      return mockStorageAreas
    }
    return response.data
  } catch (error) {
    console.warn('Backend not available, using mock storage areas data')
    return mockStorageAreas
  }
}

/**
 * Get storage area by database ID
 * GET /api/StorageArea/{id}
 */
export async function getStorageAreaById(id: string): Promise<StorageAreaDto> {
  try {
    const response = await apiClient.get<StorageAreaDto>(`/StorageArea/${id}`)
    return response.data
  } catch (error) {
    console.warn(`Backend not available, using mock storage area for ${id}`)
    const area = mockStorageAreas.find(a => a.id === id)
    if (area) return area
    throw new Error(`Storage area ${id} not found`)
  }
}

/**
 * Get storage area by business ID
 * GET /api/StorageArea/business/{businessId}
 */
export async function getStorageAreaByBusinessId(businessId: string): Promise<StorageAreaDto> {
  try {
    const response = await apiClient.get<StorageAreaDto>(`/StorageArea/business/${businessId}`)
    return response.data
  } catch (error) {
    console.warn(`Backend not available, using mock storage area for ${businessId}`)
    const area = mockStorageAreas.find(a => a.businessId === businessId)
    if (area) return area
    throw new Error(`Storage area ${businessId} not found`)
  }
}

/**
 * Get all yards (container storage areas)
 */
export async function getYards(): Promise<StorageAreaDto[]> {
  const all = await getAllStorageAreas()
  return all.filter(sa => sa.type === 'Yard')
}

/**
 * Get all warehouses (covered storage)
 */
export async function getWarehouses(): Promise<StorageAreaDto[]> {
  const all = await getAllStorageAreas()
  return all.filter(sa => sa.type === 'Warehouse')
}

/**
 * Create a new storage area
 * POST /api/StorageArea
 */
export async function createStorageArea(dto: StorageAreaDto): Promise<StorageAreaDto> {
  const response = await apiClient.post<StorageAreaDto>('/StorageArea', dto)
  return response.data
}

/**
 * Update a storage area
 * PUT /api/StorageArea/{id}
 */
export async function updateStorageArea(id: string, dto: StorageAreaDto): Promise<StorageAreaDto> {
  const response = await apiClient.put<StorageAreaDto>(`/StorageArea/${id}`, dto)
  return response.data
}

/**
 * Delete a storage area
 * DELETE /api/StorageArea/{id}
 */
export async function deleteStorageArea(id: string): Promise<void> {
  await apiClient.delete(`/StorageArea/${id}`)
}


