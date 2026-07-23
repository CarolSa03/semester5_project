<template>
  <div class="port-scene-wrapper">
    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
      <p>A carregar visualização 3D...</p>
    </div>

    <div v-if="error" class="error-overlay">
      <p>{{ error }}</p>
      <button @click="retry">Tentar novamente</button>
    </div>

    <aside class="sidebar" :class="{ collapsed: sidebarCollapsed }">
      <button class="collapse-btn" @click="sidebarCollapsed = !sidebarCollapsed">
        {{ sidebarCollapsed ? '→' : '←' }}
      </button>

      <div v-if="!sidebarCollapsed" class="sidebar-content">
        <h3>🎮 Controlos</h3>

        <div class="layout-selector">
          <h4>🗺️ Layout do Porto</h4>
          <select v-model="selectedLayout" @change="changeLayout(selectedLayout)" class="layout-dropdown">
            <option value="compacto">Porto Compacto (3 docks, 20 containers)</option>
            <option value="expandido">Porto Expandido (5 docks, 30 containers)</option>
            <option value="rio">Porto Rio (Bilateral)</option>
          </select>
          <p class="layout-info">
            {{ selectedLayout === 'rio' ? '2 margens (Oeste/Leste) separadas pelo rio' : 'Layout de cais único' }}
          </p>
        </div>

        <div class="control-group">
          <h4>🎬 Simulação</h4>
          <button @click="demoAnimation" class="control-btn animation-btn">
            🏗️ Simular Carregamento
          </button>
        </div>

        <div class="control-help">
          <p><strong>🖱️ Rato:</strong></p>
          <ul>
            <li>Botão esquerdo: Selecionar Objeto</li>
            <li>Botão direito: Rodar câmara</li>
            <li>Scroll: Zoom in/out</li>
          </ul>
        </div>

        <button @click="reloadScene" class="control-btn">
          🔄 Recarregar Cena
        </button>

        <button @click="resetCamera" class="control-btn">
          📷 Resetar Câmara
        </button>

        <div class="toggle-group">
          <h4>👁️ Visibilidade</h4>
          <label>
            <input type="checkbox" v-model="showDocks" @change="toggleDocks" />
            🚢 Mostrar Docks ({{ stats.docks }})
          </label>
          <label>
            <input type="checkbox" v-model="showWarehouses" @change="toggleWarehouses" />
            🏭 Mostrar Armazéns ({{ stats.warehouses }})
          </label>
          <label>
            <input type="checkbox" v-model="showYards" @change="toggleYards" />
            📦 Mostrar Pátios ({{ stats.yards }})
          </label>
          <label>
            <input type="checkbox" v-model="showVessels" @change="toggleVessels" />
            ⛴️ Mostrar Navios ({{ stats.vessels }})
          </label>
          <label>
            <input type="checkbox" v-model="showCranes" @change="toggleCranes" />
            🏗️ Mostrar Gruas ({{ stats.cranes }})
          </label>
          <label>
            <input type="checkbox" v-model="showContainers" @change="toggleContainers" />
            📦 Mostrar Contentores
          </label>
          <label>
            <input type="checkbox" v-model="showGrid" @change="toggleGrid" />
            📐 Mostrar Grelha
          </label>
        </div>

        <div class="camera-info">
          <h4>📊 Info</h4>
          <p>Posição: {{ cameraPosition }}</p>
          <p>Distância: {{ cameraDistance.toFixed(1) }}m</p>
        </div>
      </div>
    </aside>

    <canvas ref="canvas"></canvas>

    <div v-if="selectedObject && showInfoPanel" class="object-info-panel">
      <button class="close-btn" @click="closeInfoPanel">✕</button>
      <h3>{{ selectedObject.typeLabel || selectedObject.type?.toUpperCase() || 'OBJETO' }}</h3>
      <div class="info-content">
        <p><strong>ID:</strong> {{ selectedObject.id || 'N/A' }}</p>
        <p><strong>Nome:</strong> {{ selectedObject.name || 'N/A' }}</p>
        <p><strong>Tipo:</strong> {{ selectedObject.typeLabel || selectedObject.type || 'N/A' }}</p>
        <template v-if="selectedObject.dto">
          <p v-if="selectedObject.dto.capacity"><strong>Capacidade:</strong> {{ selectedObject.dto.capacity }}</p>
          <p v-if="selectedObject.dto.status"><strong>Estado:</strong> {{ selectedObject.dto.status }}</p>
          <p v-if="selectedObject.dto.location"><strong>Localização:</strong> {{ selectedObject.dto.location }}</p>
          <p v-if="selectedObject.dto.length"><strong>Comprimento:</strong> {{ selectedObject.dto.length }}m</p>
          <p v-if="selectedObject.dto.depth"><strong>Profundidade:</strong> {{ selectedObject.dto.depth }}m</p>
        </template>
        <template v-if="selectedObject.vvn">
          <p><strong>ETA:</strong> {{ new Date(selectedObject.vvn.eta).toLocaleString() }}</p>
          <p><strong>ETD:</strong> {{ new Date(selectedObject.vvn.etd).toLocaleString() }}</p>
        </template>
      </div>
    </div>

    <div class="floating-help" v-if="showHelp">
      <p><strong>Atalhos:</strong></p>
      <p>H - Alternar ajuda | G - Alternar grelha | R - Resetar câmara</p>
      <p v-if="selectedObject">I - Informações do objeto</p>
      <button @click="showHelp = false" class="close-help">✕</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import * as THREE from 'three'
import { PortScene } from '@/three/scene'
import { setupLights } from '@/three/lights'
import { fetchPortLayout3D } from '@/api/portApi'
import { fetchStructuredLayout } from '@/utils/structuredLayoutLoader'
import type { PortLayout3D } from '@/types/port'
import type { PortLayoutStructure } from '@/types/portLayout'

const canvas = ref<HTMLCanvasElement>()
const loading = ref(true)
const error = ref<string | null>(null)
const sidebarCollapsed = ref(false)

// Visibilidade
const showDocks = ref(true)
const showWarehouses = ref(true)
const showYards = ref(true)
const showVessels = ref(true)
const showCranes = ref(true)
const showContainers = ref(true)
const showGrid = ref(false)
const showHelp = ref(true)

// Seleção e UI
const selectedLayout = ref<string>('rio')
const selectedObject = ref<any>(null)
const showInfoPanel = ref(false)

// Instâncias
let portScene: PortScene | null = null
let animationId: number
let gridHelper: THREE.GridHelper | null = null
let portLayout: PortLayout3D | null = null
let structuredLayout: PortLayoutStructure | null = null

// Stats
const cameraPosition = ref('0, 50, 100')
const cameraDistance = ref(0)
const stats = ref({
  docks: 0,
  warehouses: 0,
  yards: 0,
  vessels: 0,
  cranes: 0
})

const initScene = async () => {
  try {
    if (!canvas.value) throw new Error('Canvas não encontrado')

    loading.value = true
    error.value = null

    // Inicializar Cena
    portScene = new PortScene(canvas.value)
    setupLights(portScene.scene)

    // Configurar callback de seleção
    portScene.onObjectSelected = (object: any) => {
      selectedObject.value = object

      if (!object) {
        showInfoPanel.value = false
      } else {
        // Se selecionar algo novo, pode abrir automaticamente ou esperar pelo 'I'
        // showInfoPanel.value = true 
      }
      console.log('Objeto selecionado:', object)
    }

    // Carregar dados iniciais
    await loadLayoutData()

    // Renderizar elementos
    renderPortLayout()

    // Helpers
    gridHelper = new THREE.GridHelper(200, 200, 0x888888, 0x444444)
    gridHelper.visible = showGrid.value
    portScene.scene.add(gridHelper)

    // --- CONSTRUÇÃO DO AMBIENTE (RIO + MARGENS) ---

    const textureLoader = new THREE.TextureLoader()

    // Textura da Relva (Pode usar a que já tem ou uma cor sólida se falhar)
    const groundTexture = textureLoader.load('/textures/ground/grass_color.jpg', (t) => {
      t.wrapS = t.wrapT = THREE.RepeatWrapping
      t.repeat.set(10, 20)
    })

    const grassMaterial = new THREE.MeshStandardMaterial({
      map: groundTexture,
      roughness: 1,
      metalness: 0
    })

    // 1. Paredes do Canal (O limite exato da água)
    // Se a água tem largura 80, as paredes estão em -40 e +40
    const wallGeo = new THREE.BoxGeometry(2, 400, 15);
    const wallMat = new THREE.MeshStandardMaterial({ color: 0x333333 });

    const leftWall = new THREE.Mesh(wallGeo, wallMat);
    leftWall.rotation.x = -Math.PI / 2;
    // Posição X = -40 (Limite da água)
    // Posição Y = -2 (Para a parede nascer do fundo)
    leftWall.position.set(-70, -5, 0);
    portScene.scene.add(leftWall);

    const rightWall = new THREE.Mesh(wallGeo, wallMat);
    rightWall.rotation.x = -Math.PI / 2;
    // Posição X = 40 (Limite da água)
    rightWall.position.set(70, -5, 0);
    portScene.scene.add(rightWall);

    // 2. Margens de Terra (Relva)
    // A Doca (Betão) ocupa de X=40 a X=70.
    // Portanto a relva começa em X=70.

    // Geometria da Relva (Larga para cobrir o horizonte)
    const bankWidth = 300;
    const bankGeometry = new THREE.PlaneGeometry(bankWidth, 400)

    // Centro da margem esquerda: Começa em -70 e vai para a esquerda (-370).
    // Centro = -70 - 150 = -220
    const leftBank = new THREE.Mesh(bankGeometry, grassMaterial)
    leftBank.rotation.x = -Math.PI / 2
    leftBank.position.set(-220, 2.0, 0) // Altura alinhada com o topo das docas
    portScene.scene.add(leftBank)

    // Centro da margem direita: Começa em 70 e vai para a direita (+370).
    // Centro = 70 + 150 = 220
    const rightBank = new THREE.Mesh(bankGeometry, grassMaterial)
    rightBank.rotation.x = -Math.PI / 2
    rightBank.position.set(220, 2.0, 0)
    portScene.scene.add(rightBank)


    // Loop de animação
    const animate = () => {
      animationId = requestAnimationFrame(animate)
      updateCameraInfo()
      portScene?.render()
    }
    animate()

    // Event Listeners
    window.addEventListener('resize', handleResize)
    window.addEventListener('keydown', handleKeyboard)

    loading.value = false
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erro desconhecido ao carregar porto'
    loading.value = false
    console.error('Error initializing scene:', err)
  }
}

const loadLayoutData = async () => {
  console.log(`Loading port layout '${selectedLayout.value}'...`)
  portLayout = await fetchPortLayout3D(selectedLayout.value)

  stats.value = {
    docks: portLayout.docks.length,
    warehouses: portLayout.warehouses.length,
    yards: portLayout.yards.length,
    vessels: portLayout.vessels.length,
    cranes: portLayout.cranes.length
  }
}

const renderPortLayout = async () => {
  if (!portScene || !portLayout) return

  // Limpar elementos dinâmicos (não limpa o chão/água fixos)
  portScene.clearScene()

  // Adicionar elementos
  portLayout.docks.forEach(dock => portScene!.addDock(dock))
  portLayout.warehouses.forEach(warehouse => portScene!.addWarehouse(warehouse))
  portLayout.yards.forEach(yard => portScene!.addYard(yard))
  portLayout.vessels.forEach(vessel => portScene!.addVessel(vessel))
  portLayout.cranes.forEach(crane => portScene!.addCrane(crane))

  // Carregar contentores otimizados (InstancedMesh)
  try {
    // Nota: fetchStructuredLayout deve ler do mesmo nome de ficheiro JSON
    structuredLayout = await fetchStructuredLayout(selectedLayout.value)

    console.log(`📦 Rendering ${structuredLayout.containers.length} containers...`)
    portScene!.updateContainers(structuredLayout.containers)

  } catch (error) {
    console.warn('No structured container layout found:', error)
  }

  updateVisibility()
}

const updateVisibility = () => {
  if (!portScene) return
  portScene.docksGroup.visible = showDocks.value
  portScene.warehousesGroup.visible = showWarehouses.value
  portScene.yardsGroup.visible = showYards.value
  portScene.vesselsGroup.visible = showVessels.value
  portScene.cranesGroup.visible = showCranes.value
  portScene.containersGroup.visible = showContainers.value
}

const updateCameraInfo = () => {
  if (!portScene) return
  const pos = portScene.camera.position
  cameraPosition.value = `${pos.x.toFixed(1)}, ${pos.y.toFixed(1)}, ${pos.z.toFixed(1)}`
  cameraDistance.value = pos.length()
}

const changeLayout = async (layoutName: string) => {
  if (!portScene) return

  loading.value = true
  selectedLayout.value = layoutName

  try {
    await loadLayoutData()
    await renderPortLayout()
    console.log(`Layout changed to '${layoutName}' successfully`)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erro ao mudar layout'
  } finally {
    loading.value = false
  }
}

// Interações UI
const toggleInfoPanel = () => {
  if (selectedObject.value) {
    showInfoPanel.value = !showInfoPanel.value
  }
}
const closeInfoPanel = () => showInfoPanel.value = false
const resetCamera = () => {
  if (!portScene) return
  // Posição padrão ajustada para ver o rio de cima
  portScene.camera.position.set(0, 150, 150)
  portScene.controls.target.set(0, 0, 0)
  portScene.controls.update()
}
const reloadScene = async () => {
  cleanup()
  await initScene()
}
const retry = () => reloadScene()
const handleResize = () => portScene?.onWindowResize()

const demoAnimation = () => {
  if (!portScene) return;
  // Exemplo de animação do cais esquerdo para um navio
  const startPoint = new THREE.Vector3(-80, 5, -40);
  const endPoint = new THREE.Vector3(-46, 15, -40);

  portScene.focusOnObject(null, new THREE.Vector3(-60, 10, -40));
  portScene.animateContainerTransfer(startPoint, endPoint, 4000);
}

// Atalhos de Teclado
const handleKeyboard = (e: KeyboardEvent) => {
  switch (e.key.toLowerCase()) {
    case 'h': showHelp.value = !showHelp.value; break
    case 'g': showGrid.value = !showGrid.value; toggleGrid(); break
    case 'r': resetCamera(); break
    case 'i': toggleInfoPanel(); break
  }
}

const toggleGrid = () => {
  if (gridHelper) gridHelper.visible = showGrid.value
}
const toggleDocks = () => updateVisibility()
const toggleWarehouses = () => updateVisibility()
const toggleYards = () => updateVisibility()
const toggleVessels = () => updateVisibility()
const toggleCranes = () => updateVisibility()
const toggleContainers = () => updateVisibility()

const cleanup = () => {
  window.removeEventListener('resize', handleResize)
  window.removeEventListener('keydown', handleKeyboard)
  if (animationId) cancelAnimationFrame(animationId)
  portScene?.dispose()
  portScene = null
  gridHelper = null
}

onMounted(initScene)
onUnmounted(cleanup)
</script>

<style scoped>
.port-scene-wrapper {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  overflow: hidden;
}

canvas {
  display: block;
  width: 100%;
  height: 100%;
}

/* Overlays */
.loading-overlay, .error-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.95);
  z-index: 100;
}
.spinner {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #42b983;
  border-radius: 50%;
  width: 50px; height: 50px;
  animation: spin 1s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* Sidebar */
.sidebar {
  position: absolute;
  top: 0; left: 0;
  width: 320px; height: 100%;
  background: rgba(255, 255, 255, 0.98);
  box-shadow: 2px 0 10px rgba(0,0,0,0.2);
  transition: transform 0.3s ease-in-out;
  z-index: 50;
  overflow-y: auto;
}
.sidebar.collapsed { transform: translateX(-280px); }

.collapse-btn {
  position: absolute;
  top: 10px; right: 10px;
  background: #42b983; color: white;
  border: none; padding: 0.5rem;
  cursor: pointer; border-radius: 4px;
}

.sidebar-content { padding: 3rem 1.5rem 1.5rem; }
.sidebar-content h3 { color: #2c3e50; border-bottom: 2px solid #42b983; padding-bottom: 0.5rem; }

/* Controlos */
.control-btn {
  width: 100%; padding: 0.8rem; margin-bottom: 0.8rem;
  background: #2c3e50; color: white;
  border: none; border-radius: 6px;
  cursor: pointer; transition: background 0.2s;
}
.control-btn:hover { background: #34495e; }
.animation-btn { background: #e67e22; }
.animation-btn:hover { background: #d35400; }

.layout-selector {
  background: #f1f8e9; padding: 1rem;
  border-radius: 6px; margin-bottom: 1rem;
  border-left: 4px solid #42b983;
}
.layout-dropdown {
  width: 100%; padding: 0.5rem; margin-top: 0.5rem;
  border: 1px solid #ccc; border-radius: 4px;
}

.toggle-group label {
  display: flex; align-items: center;
  margin-bottom: 0.5rem; cursor: pointer;
  font-size: 0.9rem;
}
.toggle-group input { margin-right: 0.5rem; }

/* Info Panel */
.object-info-panel {
  position: absolute; top: 20px; right: 20px;
  background: white; border-radius: 8px;
  padding: 1.5rem; width: 300px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.2);
  z-index: 50; border-top: 4px solid #3498db;
}
.object-info-panel h3 { margin-top: 0; color: #2c3e50; }
.close-btn {
  position: absolute; top: 10px; right: 10px;
  background: none; border: none; font-size: 1.2rem; cursor: pointer;
}

/* Floating Help */
.floating-help {
  position: absolute; bottom: 20px; right: 20px;
  background: rgba(0,0,0,0.8); color: white;
  padding: 1rem; border-radius: 8px;
  font-size: 0.85rem; z-index: 40;
}
.close-help {
  position: absolute; top: 5px; right: 5px;
  background: none; border: none; color: white; cursor: pointer;
}
</style>