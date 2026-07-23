import type { StorageAreaDto } from '@/types/storageArea'

/**
 * Mock data for Storage Areas (Warehouses and Yards)
 * Used when backend is unavailable or returns empty data
 */
export const mockStorageAreas: StorageAreaDto[] = [
  {
    id: 'warehouse-1',
    businessId: 'WH-001',
    type: 'Warehouse',
    location: 'Zone A',
    maxCapacityTEU: 500,
    currentCapacityTEU: 200,
    servedDocks: ['1']
  },
  {
    id: 'warehouse-2',
    businessId: 'WH-002',
    type: 'Warehouse',
    location: 'Zone B',
    maxCapacityTEU: 400,
    currentCapacityTEU: 150,
    servedDocks: ['2']
  },
  {
    id: 'yard-1',
    businessId: 'YD-001',
    type: 'Yard',
    location: 'North Yard',
    maxCapacityTEU: 1000,
    currentCapacityTEU: 600,
    servedDocks: ['1', '3']
  },
  {
    id: 'yard-2',
    businessId: 'YD-002',
    type: 'Yard',
    location: 'South Yard',
    maxCapacityTEU: 800,
    currentCapacityTEU: 400,
    servedDocks: ['2']
  }
]

/**
 * Get mock warehouses only
 */
export const mockWarehouses: StorageAreaDto[] = mockStorageAreas.filter(
  (sa) => sa.type === 'Warehouse'
)

/**
 * Get mock yards only
 */
export const mockYards: StorageAreaDto[] = mockStorageAreas.filter(
  (sa) => sa.type === 'Yard'
)
