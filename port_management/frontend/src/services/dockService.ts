import apiClient from './apiClient'
import type { DockDto } from '@/types/dock'
import { mockDocks } from '@/mocks'

const baseURL = import.meta.env.VITE_API_BASE_URL || '/api'

/**
 * Get all docks
 * GET /api/Dock
 */
export async function getAllDocks(): Promise<DockDto[]> {
  try {
    const response = await apiClient.get<DockDto[]>('/Dock')
    // If backend returns empty array, use mocks
    if (!response.data || response.data.length === 0) {
      console.warn('Backend returned empty data, using mock docks data')
      return mockDocks
    }
    return response.data
  } catch (error) {
    console.warn('Backend not available, using mock docks data')
    return mockDocks
  }
}

/**
 * Get dock by ID
 * GET /api/Dock/{id}
 */
export async function getDockById(id: string): Promise<DockDto> {
  const response = await apiClient.get<DockDto>(`/Dock/${id}`)
  return response.data
}

/**
 * Create a new dock
 * POST /api/Dock
 */
export async function createDock(dto: DockDto): Promise<DockDto> {
  const response = await apiClient.post<DockDto>('/Dock', dto)
  return response.data
}

/**
 * Update a dock
 * PUT /api/Dock/{id}
 */
export async function updateDock(id: string, dto: DockDto): Promise<DockDto> {
  const response = await apiClient.put<DockDto>(`/Dock/${id}`, dto)
  return response.data
}

/**
 * Delete a dock
 * DELETE /api/Dock/{id}
 */
export async function deleteDock(id: string): Promise<void> {
  await apiClient.delete(`/Dock/${id}`)
}

