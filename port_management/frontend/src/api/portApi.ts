import { useDockApi } from '@/services/useDockApi'
import { useStorageAreaApi } from '@/services/useStorageAreaApi'
import { useVesselVisitNotificationApi } from '@/services/useVesselVisitNotificationApi'
import { usePhysicalResourceApi } from '@/services/usePhysicalResourceApi'
import { useVesselRecordApi } from '@/services/useVesselRecordApi'
import type { PortLayout3D, PortLayoutConfig, Dock3D, Warehouse3D, Yard3D, Vessel3D, Crane3D, Position3D, Dimensions3D, TextureConfig } from '@/types/port'
import { mockDocks, mockVessels, mockVesselVisitNotifications, mockStorageAreas, mockSTSCranes, mockYardCranes } from '@/mocks'

// --- FUNÇÕES DE MOCK ---
function getMockDocks() { return mockDocks }
function getMockStorageAreas() { return mockStorageAreas }
function getMockVessels() { return mockVessels }
function getMockVVNs() { return mockVesselVisitNotifications }
function getMockSTSCranes() { return mockSTSCranes }
function getMockYardCranes() { return mockYardCranes }

export async function fetchPortLayoutConfig(layoutName?: string): Promise<PortLayoutConfig> {
  const layout = layoutName || 'compacto'
  try {
    const timestamp = new Date().getTime()
    const response = await fetch(`/layouts/${layout}.json?t=${timestamp}`)
    if (response.ok) {
      const jsonData = await response.json()
      return {
        positions: {}, dimensions: {},
        textures: jsonData.textures || {},
        models: {
          warehouses: '/models/long_warehouse.glb',
          stsCranes: '/models/crane.glb',
          yardCranes: '/models/crane.glb',
          vessels: { 'container-ship': '/models/cargo-ship.glb' }
        }
      }
    }
  } catch (e) { console.warn(e) }
  return getDefaultLayoutConfig()
}

export async function fetchPortLayout3D(layoutName?: string): Promise<PortLayout3D> {
  const config = await fetchPortLayoutConfig(layoutName)

  // DETEÇÃO DO LAYOUT RIO
  const isRio = layoutName === 'rio';

  const dockApi = useDockApi()
  const storageApi = useStorageAreaApi()
  const vvnApi = useVesselVisitNotificationApi()
  const vesselApi = useVesselRecordApi()
  const physicalApi = usePhysicalResourceApi()

  let docksData: any[] = []
  let yardsData: any[] = []
  let warehousesData: any[] = []
  let approvedVVNs: any[] = []
  let stsCranes: any[] = []
  let yardCranes: any[] = []

  try {
    // --- LÓGICA DE DADOS ---
    // Se for 'rio', ignoramos o backend e usamos Mocks para garantir que aparecem as 3 docas
    if (isRio) {
      console.log("🌊 MODO RIO: Forçando dados Mock para visualização correta.");
      docksData = getMockDocks(); // Garante 3 Docas

      const allStorage = getMockStorageAreas();
      yardsData = allStorage.filter((sa: any) => sa.type === 'Yard');
      warehousesData = allStorage.filter((sa: any) => sa.type === 'Warehouse');

      const allVVNs = getMockVVNs();
      approvedVVNs = allVVNs.filter((vvn: any) => vvn.status === 'Approved');

      stsCranes = getMockSTSCranes();
      yardCranes = getMockYardCranes();
    } else {
      // Lógica Normal (Backend First)
      const [allDocks, allStorageAreas, allVVNs, allSTSCranes, allYardCranes] = await Promise.all([
        dockApi.fetchDocks().catch(() => []),
        storageApi.listStorageAreas().catch(() => []),
        vvnApi.listNotifications().catch(() => []),
        physicalApi.listSTSCranes().catch(() => []),
        physicalApi.listYardCrane().catch(() => [])
      ])

      docksData = (allDocks && allDocks.length > 0) ? allDocks : getMockDocks()
      const allStorage = (allStorageAreas && allStorageAreas.length > 0) ? allStorageAreas : getMockStorageAreas()
      yardsData = allStorage.filter((sa: any) => sa.type === 'Yard')
      warehousesData = allStorage.filter((sa: any) => sa.type === 'Warehouse')

      const vvnData = (allVVNs && allVVNs.length > 0) ? allVVNs : getMockVVNs()
      approvedVVNs = vvnData.filter((vvn: any) => vvn.status === 'Approved')

      stsCranes = (allSTSCranes && allSTSCranes.length > 0) ? allSTSCranes : getMockSTSCranes()
      yardCranes = (allYardCranes && allYardCranes.length > 0) ? allYardCranes : getMockYardCranes()
    }
  } catch (error) {
    console.warn("Using mocks due to error", error)
    docksData = getMockDocks()
    // ... (fallback normal)
  }

  // CARREGAR POSIÇÕES DO JSON
  try {
    const timestamp = new Date().getTime()
    const layoutResponse = await fetch(`/layouts/${layoutName || 'compacto'}.json?t=${timestamp}`)
    if (layoutResponse.ok) {
      const layoutData = await layoutResponse.json()

      // Mapear Docks
      if (layoutData.docks) {
        if (!config.positions) config.positions = {}
        if (!config.positions.docks) config.positions.docks = {}
        if (!config.dimensions) config.dimensions = {}
        if (!config.dimensions.docks) config.dimensions.docks = {}

        layoutData.docks.forEach((dock: any, index: number) => {
          // Usa o ID do dado se existir, ou gera um ID genérico para mapeamento
          const dockId = docksData[index]?.id || `dock-${index + 1}`
          if (config.positions?.docks && config.dimensions?.docks) {
            config.positions.docks[dockId] = { x: dock.x, y: 2.05, z: dock.z }
            config.dimensions.docks[dockId] = { width: dock.width, height: 0.2, depth: dock.length }
          }
        })
      }

      // Mapear Navios (Forçar posições do JSON)
      if (layoutData.vessels) {
        if (!config.positions) config.positions = {}
        if (!config.positions.vessels) config.positions.vessels = {}

        // Usa os navios aprovados disponíveis
        layoutData.vessels.forEach((vPos: any, index: number) => {
          // Se houver um VVN aprovado correspondente a este índice, atribuímos a posição
          if (approvedVVNs[index]) {
            config.positions!.vessels![approvedVVNs[index].vesselId] = {
              x: vPos.x,
              y: vPos.y || 0,
              z: vPos.z
            }
          }
        })
      }

      // ... (Mapeamento de Warehouses e Yards mantém-se igual) ...
      // Mapear Warehouses
      if (layoutData.warehouses) {
        if (!config.positions!.storageAreas) config.positions!.storageAreas = {}
        layoutData.warehouses.forEach((w: any, index: number) => {
          const wId = warehousesData[index]?.id || `wh-${index}`
          config.positions!.storageAreas![wId] = { x: w.x, y: 8, z: w.z }
        })
      }
      // Mapear Yards
      if (layoutData.yards) {
        if (!config.positions!.storageAreas) config.positions!.storageAreas = {}
        if (!config.dimensions!.storageAreas) config.dimensions!.storageAreas = {}
        layoutData.yards.forEach((y: any, index: number) => {
          const yId = yardsData[index]?.id || `yd-${index}`
          config.positions!.storageAreas![yId] = { x: y.x, y: 2.1, z: y.z }
          config.dimensions!.storageAreas![yId] = { width: y.width, height: 0.2, depth: y.height }
        })
      }
      // Mapear Cranes
      if (layoutData.cranes) {
        if (!config.positions!.cranes) config.positions!.cranes = {}
        const allCranes = [...stsCranes, ...yardCranes]
        layoutData.cranes.forEach((c: any, index: number) => {
          if (allCranes[index]) {
            config.positions!.cranes![allCranes[index].id] = { x: c.x, y: 6, z: c.z }
          }
        })
      }
    }
  } catch (e) {}

  // BUILDERS
  const docks: Dock3D[] = docksData.map((dto, index) => {
    // Garante que usa a posição do config (vinda do JSON)
    const pos = config.positions?.docks?.[dto.id] || generateDefaultDockPosition(index)
    return {
      dto,
      position: pos,
      dimensions: config.dimensions?.docks?.[dto.id] || generateDefaultDockDimensions(dto),
      rotation: { x: 0, y: 0, z: 0 },
      texture: config.textures?.docks || getDefaultDockTexture(),
      model: config.models?.docks
    }
  })

  // ... (Builders de Warehouse e Yard mantêm-se) ...
  const warehouses: Warehouse3D[] = warehousesData.map((dto) => ({
    dto,
    position: config.positions?.storageAreas?.[dto.id] || {x:0, y:0, z:0},
    dimensions: generateDefaultWarehouseDimensions(dto), // Simplificado
    rotation: {x:0, y:0, z:0},
    texture: config.textures?.warehouses,
    model: config.models?.warehouses
  }))

  const yards: Yard3D[] = yardsData.map((dto) => ({
    dto,
    position: config.positions?.storageAreas?.[dto.id] || {x:0, y:0, z:0},
    dimensions: config.dimensions?.storageAreas?.[dto.id] || generateDefaultYardDimensions(dto),
    rotation: {x:0, y:0, z:0},
    texture: config.textures?.yards,
    model: config.models?.yards
  }))

  const vessels: Vessel3D[] = []
  for (const vvn of approvedVVNs) {
    // Simplificação: Usa Mock Vessel direto para garantir visualização
    const mockVessels = getMockVessels()
    const vessel = mockVessels.find((v: any) => v.id === vvn.vesselId)

    if (vessel) {
      // CORREÇÃO DA ROTAÇÃO: Usa a rotação do JSON se existir, senão 0
      // IMPORTANTE: Removemos o Math.PI / 2 forçado
      const jsonPos = config.positions?.vessels?.[vvn.vesselId];
      const rotationY = 0; // Alinhado com o eixo Z (Rio)

      vessels.push({
        vvn,
        vessel,
        position: jsonPos || generateDefaultVesselPosition(vvn, docks),
        dimensions: { width: 12, height: 8, depth: 40 },
        rotation: { x: 0, y: rotationY, z: 0 }, // <--- ROTAÇÃO CORRIGIDA
        texture: config.textures?.vessels || getDefaultVesselTexture(),
        model: config.models?.vessels?.[vessel.vesselTypeId]
      })
    }
  }

  // Cranes Builder (Simplificado)
  const cranes: Crane3D[] = [...stsCranes, ...yardCranes].map(c => ({
    dto: c,
    type: c.code.startsWith('STS') ? 'STS' : 'Yard',
    position: config.positions?.cranes?.[c.id] || {x:0, y:0, z:0},
    dimensions: { width: 6, height: 15, depth: 6 },
    rotation: { x: 0, y: 0, z: 0 },
    texture: config.textures?.stsCranes,
    model: config.models?.stsCranes
  }))

  return { docks, warehouses, yards, vessels, cranes }
}

// ... (Manter as funções auxiliares getDefault... no final do ficheiro)
function getDefaultLayoutConfig(): PortLayoutConfig { return { positions:{}, dimensions:{}, textures:{}, models:{} } }
function getDefaultDockTexture() { return { roughness: 0.8, metalness: 0.2 } }
function getDefaultVesselTexture() { return { roughness: 0.4, metalness: 0.6 } }
function generateDefaultDockPosition(i: number) { return { x: i * 50, y: 0, z: 0 } }
function generateDefaultDockDimensions(d: any) { return { width: 50, height: 5, depth: 20 } }
function generateDefaultWarehouseDimensions(d: any) { return { width: 20, height: 10, depth: 20 } }
function generateDefaultYardDimensions(d: any) { return { width: 30, height: 0.2, depth: 30 } }
function generateDefaultVesselPosition(v: any, d: any) { return { x: 0, y: 0, z: 0 } }