// Dock DTOs and Types

export interface DockDto {
  id?: string
  name: string
  location: string
  length: number
  depth: number
  maxDraft: number
  maxSTS: number
  allowedVesselTypes: string[]
  stsCranes?: string[]
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}

