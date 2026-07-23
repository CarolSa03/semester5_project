import type { DockDto } from '@/types/dock'

/**
 * Mock data for Docks
 * Used when backend is unavailable or returns empty data
 */
export const mockDocks: DockDto[] = [
  {
    id: '1',
    name: 'Dock Norte',
    location: 'North Terminal',
    length: 200,
    depth: 15,
    maxDraft: 12,
    maxSTS: 3,
    allowedVesselTypes: ['container-ship', 'bulk-carrier'],
    stsCranes: ['STS-001', 'STS-002'],
    isActive: true,
    createdAt: new Date().toISOString()
  },
  {
    id: '2',
    name: 'Dock Sul',
    location: 'South Terminal',
    length: 180,
    depth: 14,
    maxDraft: 11,
    maxSTS: 2,
    allowedVesselTypes: ['container-ship'],
    stsCranes: ['STS-003'],
    isActive: true,
    createdAt: new Date().toISOString()
  },
  {
    id: '3',
    name: 'Dock Central',
    location: 'Central Terminal',
    length: 220,
    depth: 16,
    maxDraft: 13,
    maxSTS: 4,
    allowedVesselTypes: ['container-ship', 'tanker'],
    stsCranes: ['STS-004', 'STS-005'],
    isActive: true,
    createdAt: new Date().toISOString()
  }
]
