// Port Layout Generator Types
// Sistema de geração automática de layouts portuários realistas

export interface DockLayout {
  x: number
  z: number
  length: number
  width: number
  name?: string
}

export interface YardLayout {
  x: number
  z: number
  width: number
  height: number
  name?: string
}

export interface WarehouseLayout {
  x: number
  z: number
  name?: string
}

export interface CraneLayout {
  x: number
  z: number
  dockId?: number
}

export interface ContainerLayout {
  x: number
  z: number
  y?: number // Altura (stacking)
  rotation?: number
}

/**
 * Estrutura completa de um layout portuário
 * Compatível com geração automática e backend
 */
export interface PortLayoutStructure {
  name: string
  description: string
  docks: DockLayout[]
  yards: YardLayout[]
  warehouses: WarehouseLayout[]
  cranes: CraneLayout[]
  containers: ContainerLayout[]
}

/**
 * Configuração para gerador de layouts
 */
export interface PortGeneratorConfig {
  numDocks: number
  bilateral?: boolean // Docas dos dois lados
  numWarehouses: number
  numYards: number
  cranesPerDock?: number
  containersInYard?: number
  spacing?: {
    dockToDock?: number
    dockToWarehouse?: number
    containerSpacing?: number
  }
}

