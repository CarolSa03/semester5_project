// Storage Area DTOs and Types

export interface StorageAreaDto {
  id?: string
  businessId: string
  type: 'Yard' | 'Warehouse' // Yard = Container Yard, Warehouse = Covered storage
  location: string
  maxCapacityTEU: number
  currentCapacityTEU: number
  servedDocks?: string[]
  dockDistances?: Record<number, number>
  notes?: string
}

