// Port Structure Types for 3D Visualization
// US 3.3.2, 3.3.3, 3.3.4

import type { DockDto } from './dock'
import type { StorageAreaDto } from './storageArea'
import type { VesselRecordDto, VesselVisitNotificationDto } from './vessel'
import type { YardCraneDto, STSCraneDto } from './physicalResources'

// ============================================================================
// 3D Positioning and Rendering
// ============================================================================

export interface Position3D {
  x: number
  y: number
  z: number
}

export interface Dimensions3D {
  width: number
  height: number
  depth: number
}

export interface Rotation3D {
  x: number
  y: number
  z: number
}

/**
 * US 3.3.4: Texture configuration with at least 2 maps
 * (color map + roughness/bump/normal map)
 */
export interface TextureConfig {
  colorMap?: string
  roughnessMap?: string
  bumpMap?: string
  normalMap?: string
  roughness?: number
  metalness?: number
}

// ============================================================================
// 3D Port Elements (combining backend DTOs with 3D data)
// ============================================================================

/**
 * US 3.3.2: Dock with 3D positioning
 */
export interface Dock3D {
  dto: DockDto // Real data from backend
  position: Position3D
  dimensions: Dimensions3D
  rotation?: Rotation3D
  texture?: TextureConfig
  model?: string // Optional GLB model path
}

/**
 * US 3.3.2: Warehouse (StorageArea type='Warehouse') with 3D positioning
 */
export interface Warehouse3D {
  dto: StorageAreaDto // Real data from backend
  position: Position3D
  dimensions: Dimensions3D
  rotation?: Rotation3D
  texture?: TextureConfig
  model?: string // Optional GLB model path
}

/**
 * US 3.3.2: Container Yard (StorageArea type='Yard') with 3D positioning
 */
export interface Yard3D {
  dto: StorageAreaDto // Real data from backend
  position: Position3D
  dimensions: Dimensions3D
  rotation?: Rotation3D
  texture?: TextureConfig
  model?: string // Optional GLB model path
}

/**
 * US 3.3.3: Vessel with 3D positioning (from approved VVN)
 */
export interface Vessel3D {
  vvn: VesselVisitNotificationDto // Vessel Visit Notification (approved)
  vessel: VesselRecordDto // Vessel record
  position: Position3D
  dimensions: Dimensions3D
  rotation?: Rotation3D
  texture?: TextureConfig
  model?: string // Optional GLB model path
}

/**
 * US 3.3.3: Crane with 3D positioning (STS or Yard Crane with assigned area)
 */
export interface Crane3D {
  dto: STSCraneDto | YardCraneDto // Real data from backend
  type: 'STS' | 'Yard'
  position: Position3D
  dimensions: Dimensions3D
  rotation?: Rotation3D
  texture?: TextureConfig
  model?: string // Optional GLB model path
}

/**
 * Complete port layout for 3D rendering
 * US 3.3.2 + 3.3.3
 */
export interface PortLayout3D {
  docks: Dock3D[]
  warehouses: Warehouse3D[]
  yards: Yard3D[]
  vessels: Vessel3D[]
  cranes: Crane3D[]
}

// ============================================================================
// Layout Configuration from Backend
// US 3.3.4: Texture and material properties retrieved as JSON
// ============================================================================

export interface PortLayoutConfig {
  positions?: {
    docks?: Record<string, Position3D> // key = dock ID
    storageAreas?: Record<string, Position3D> // key = storage area ID
    vessels?: Record<string, Position3D> // key = vessel ID
    cranes?: Record<string, Position3D> // key = crane ID
  }
  dimensions?: {
    docks?: Record<string, Dimensions3D>
    storageAreas?: Record<string, Dimensions3D>
    vessels?: Record<string, Dimensions3D>
    cranes?: Record<string, Dimensions3D>
  }
  textures?: {
    docks?: TextureConfig
    warehouses?: TextureConfig
    yards?: TextureConfig
    vessels?: TextureConfig
    stsCranes?: TextureConfig
    yardCranes?: TextureConfig
  }
  models?: {
    docks?: string
    warehouses?: string
    yards?: string
    vessels?: Record<string, string> // key = vessel type ID
    stsCranes?: string
    yardCranes?: string
  }
}

