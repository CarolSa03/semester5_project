import type { PortLayoutStructure } from '@/types/portLayout'
import type { PortLayoutConfig } from '@/types/port'

/**
 * Converte layout estruturado (docks, yards, warehouses, cranes, containers)
 * para o formato 3D compatível com o sistema existente
 */
export async function loadStructuredLayout(layoutName: string): Promise<{
  config: PortLayoutConfig
  structure: PortLayoutStructure
}> {
  try {
    // Tentar carregar layout estruturado
    const response = await fetch(`/layouts/${layoutName}-realista.json`)
    if (!response.ok) {
      throw new Error(`Layout ${layoutName} não encontrado`)
    }

    const structure: PortLayoutStructure = await response.json()
    console.log(`✅ Layout estruturado carregado: ${structure.name}`)
    console.log(`📊 Stats:`, {
      docks: structure.docks.length,
      yards: structure.yards.length,
      warehouses: structure.warehouses.length,
      cranes: structure.cranes.length,
      containers: structure.containers.length
    })

    // Converter para formato PortLayoutConfig
    const config: PortLayoutConfig = {
      positions: {
        docks: {},
        storageAreas: {},
        cranes: {}
      },
      dimensions: {
        docks: {},
        storageAreas: {}
      },
      textures: {
        docks: {
          roughness: 0.8,
          metalness: 0.2
        },
        warehouses: {
          roughness: 0.6,
          metalness: 0.3
        },
        yards: {
          roughness: 0.9,
          metalness: 0.1
        }
      },
      models: {
        warehouses: '/models/long_warehouse.glb',
        stsCranes: '/models/crane.glb',
        yardCranes: '/models/crane.glb',
        vessels: {
          'container-ship': '/models/cargo-ship.glb'
        }
      }
    }

    // Mapear docas
    structure.docks.forEach((dock, index) => {
      const dockId = `dock-${index + 1}`
      config.positions!.docks![dockId] = {
        x: dock.x,
        y: 0,
        z: dock.z
      }
      config.dimensions!.docks![dockId] = {
        width: dock.length,
        height: 5,
        depth: dock.width
      }
    })

    // Mapear warehouses
    structure.warehouses.forEach((wh, index) => {
      const whId = `warehouse-${index + 1}`
      config.positions!.storageAreas![whId] = {
        x: wh.x,
        y: 6,
        z: wh.z
      }
    })

    // Mapear yards
    structure.yards.forEach((yard, index) => {
      const yardId = `yard-${index + 1}`
      config.positions!.storageAreas![yardId] = {
        x: yard.x,
        y: 0.1,
        z: yard.z
      }
      config.dimensions!.storageAreas![yardId] = {
        width: yard.width,
        height: 0.2,
        depth: yard.height
      }
    })

    // Mapear cranes
    structure.cranes.forEach((crane, index) => {
      const craneId = `crane-${index + 1}`
      config.positions!.cranes![craneId] = {
        x: crane.x,
        y: 8,
        z: crane.z
      }
    })

    return { config, structure }
  } catch (error) {
    console.error('Erro ao carregar layout estruturado:', error)
    throw error
  }
}

/**
 * Carrega layout estruturado diretamente do JSON
 */
export async function fetchStructuredLayout(layoutName: string): Promise<PortLayoutStructure> {
  const response = await fetch(`/layouts/${layoutName}.json`)
  if (!response.ok) {
    throw new Error(`Layout ${layoutName} não encontrado`)
  }
  return await response.json()
}

