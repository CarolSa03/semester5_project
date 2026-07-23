// Vessel Type DTOs and Types

export interface VesselTypeDto {
  id?: string
  name: string
  description?: string | null
  capacityTEU?: number | null
  maxRows?: number | null
  maxBays?: number | null
  maxTiers?: number | null
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}
