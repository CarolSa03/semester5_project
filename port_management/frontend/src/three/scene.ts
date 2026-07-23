import * as THREE from 'three'
import { OrbitControls } from 'three-stdlib'
import { GLTFLoader } from 'three-stdlib'
import { RoundedBoxGeometry } from 'three-stdlib';
import * as TWEEN from '@tweenjs/tween.js'
import { Water } from 'three/examples/jsm/objects/Water.js';

import type {
    Dock3D,
    Warehouse3D,
    Yard3D,
    Vessel3D,
    Crane3D,
    TextureConfig
} from '@/types/port'

export class PortScene {
    scene: THREE.Scene
    camera: THREE.PerspectiveCamera
    renderer: THREE.WebGLRenderer
    controls: OrbitControls
    textureLoader: THREE.TextureLoader
    gltfLoader: GLTFLoader

    // Object groups for easy management
    docksGroup: THREE.Group
    warehousesGroup: THREE.Group
    yardsGroup: THREE.Group
    vesselsGroup: THREE.Group
    cranesGroup: THREE.Group
    containersGroup: THREE.Group

    // Object picking
    raycaster: THREE.Raycaster
    mouse: THREE.Vector2
    selectedObject: THREE.Object3D | null = null
    onObjectSelected?: (object: any) => void

    private currentAnimationId: number | null = null;
    private waterMesh: THREE.Mesh | null = null;

    // --- NOVAS PROPRIEDADES (SPRINT C & OTIMIZAÇÕES) ---

    // US 4.2.5: Spotlight para destaque
    private selectionSpotlight: THREE.SpotLight | null = null;

    // Helper visual para seleção de contentores (caixa amarela)
    private selectionHelper: THREE.Mesh | null = null;

    // Mapa de dados para contentores instanciados
    private containerDataMap: Map<number, any> = new Map();

    // Cache para reutilização de modelos e materiais
    private modelCache: Map<string, THREE.Group> = new Map()
    private materialCache = new Map<string, THREE.MeshStandardMaterial>()
    private textureCache: Map<string, THREE.Texture> = new Map()

    // Track resources for disposal
    private disposables: Array<THREE.BufferGeometry | THREE.Material> = []

    // Instanced rendering for containers (Performance Boost)
    private containerInstancedMesh: THREE.InstancedMesh | null = null
    private containerDummy = new THREE.Object3D()
    private containerCount = 0

    // Pickable objects optimization
    private pickableObjects: THREE.Mesh[] = []

    // Shared geometries
    private sharedGeometries = {
        //unitBox: new THREE.BoxGeometry(1, 1, 1),
        unitBox: new RoundedBoxGeometry(1, 1, 1, 4, 0.05),

        unitPlane: new THREE.PlaneGeometry(1, 1),
        //containerBox: new THREE.BoxGeometry(2.4, 2.6, 6)
        containerBox: new RoundedBoxGeometry(2.4, 2.6, 6, 2, 0.1)
    }

    constructor(canvas: HTMLCanvasElement) {
        // Scene
        this.scene = new THREE.Scene()
        this.scene.background = new THREE.Color(0x87ceeb)
        this.scene.fog = new THREE.Fog(0x87ceeb, 200, 500);

        // Camera
        this.camera = new THREE.PerspectiveCamera(
            75,
            window.innerWidth / window.innerHeight,
            0.1,
            1000
        )
        this.camera.position.set(0, 50, 100)
        this.camera.lookAt(0, 0, 0)

        // Light Setup
        const ambientLight = new THREE.AmbientLight(0x404040, 2.0);
        this.scene.add(ambientLight);

        const sunLight = new THREE.DirectionalLight(0xffffff, 1.5);
        sunLight.position.set(100, 150, 50);
        sunLight.castShadow = true;

        const d = 150;
        sunLight.shadow.camera.left = -d;
        sunLight.shadow.camera.right = d;
        sunLight.shadow.camera.top = d;
        sunLight.shadow.camera.bottom = -d;

        sunLight.shadow.mapSize.width = 2048;
        sunLight.shadow.mapSize.height = 2048;
        sunLight.shadow.bias = -0.0005;

        this.scene.add(sunLight);

        // Water
        this.createWater();

        // Renderer (optimized)
        this.renderer = new THREE.WebGLRenderer({
            canvas,
            antialias: false,
            powerPreference: 'high-performance'
        })
        this.renderer.setSize(window.innerWidth, window.innerHeight)
        this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 1))
        this.renderer.shadowMap.enabled = true;
        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
        

        // OrbitControls 
        this.controls = new OrbitControls(this.camera, canvas)
        this.controls.enableDamping = true
        this.controls.dampingFactor = 0.05
        this.controls.minDistance = 10
        this.controls.maxDistance = 1000
        this.controls.maxPolarAngle = Math.PI / 2.2
        this.controls.mouseButtons = {
            LEFT: THREE.MOUSE.ROTATE,
            MIDDLE: THREE.MOUSE.DOLLY,
            RIGHT: THREE.MOUSE.ROTATE
        }

        this.controls.addEventListener('start', () => {
            if (this.currentAnimationId !== null) {
                cancelAnimationFrame(this.currentAnimationId);
                this.currentAnimationId = null;
            }
        });

        // Loaders
        this.textureLoader = new THREE.TextureLoader()
        this.gltfLoader = new GLTFLoader()

        // Groups
        this.docksGroup = new THREE.Group()
        this.docksGroup.name = 'Docks'
        this.scene.add(this.docksGroup)

        this.warehousesGroup = new THREE.Group()
        this.warehousesGroup.name = 'Warehouses'
        this.scene.add(this.warehousesGroup)

        this.yardsGroup = new THREE.Group()
        this.yardsGroup.name = 'Yards'
        this.scene.add(this.yardsGroup)

        this.vesselsGroup = new THREE.Group()
        this.vesselsGroup.name = 'Vessels'
        this.scene.add(this.vesselsGroup)

        this.cranesGroup = new THREE.Group()
        this.cranesGroup.name = 'Cranes'
        this.scene.add(this.cranesGroup)

        this.containersGroup = new THREE.Group()
        this.containersGroup.name = 'Containers'
        this.scene.add(this.containersGroup)

        // Raycaster
        this.raycaster = new THREE.Raycaster()
        this.mouse = new THREE.Vector2()

        // Event listeners
        canvas.addEventListener('click', this.onCanvasClick.bind(this))
        canvas.addEventListener('mousemove', this.onCanvasMouseMove.bind(this))

        // --- INICIALIZAÇÃO HELPERS SPRINT C ---

        // 1. Configurar Spotlight (US 4.2.5)
        this.selectionSpotlight = new THREE.SpotLight(0xffffff, 500); // Intensidade alta
        this.selectionSpotlight.angle = Math.PI / 8;
        this.selectionSpotlight.penumbra = 0.5;      // US 4.2.5: Penumbra
        this.selectionSpotlight.decay = 1;
        this.selectionSpotlight.distance = 200;
        this.selectionSpotlight.castShadow = false;
        this.selectionSpotlight.visible = false;
        this.scene.add(this.selectionSpotlight);
        this.scene.add(this.selectionSpotlight.target);

        // 2. Criar Helper de Seleção (Caixa Amarela)
        const helperGeometry = new THREE.BoxGeometry(2.5, 2.7, 6.1);
        const helperMaterial = new THREE.MeshBasicMaterial({
            color: 0xffff00,
            wireframe: true,
            transparent: true,
            opacity: 0.5
        });
        this.selectionHelper = new THREE.Mesh(helperGeometry, helperMaterial);
        this.selectionHelper.visible = false;
        this.scene.add(this.selectionHelper);
    }

    /**
     * Clear all port elements from scene
     */
    clearScene() {
        const disposeGroup = (group: THREE.Group) => {
            group.traverse((child) => {
                if (child instanceof THREE.Mesh) {
                    if (child.geometry) child.geometry.dispose()
                    if (child.material) {
                        if (Array.isArray(child.material)) {
                            child.material.forEach(mat => mat.dispose())
                        } else {
                            child.material.dispose()
                        }
                    }
                }
            })
            group.clear()
        }

        disposeGroup(this.docksGroup)
        disposeGroup(this.warehousesGroup)
        disposeGroup(this.yardsGroup)
        disposeGroup(this.vesselsGroup)
        disposeGroup(this.cranesGroup)
        disposeGroup(this.containersGroup)

        if (this.containerInstancedMesh) {
            this.containerInstancedMesh.dispose()
            this.containerInstancedMesh = null
        }
        this.containerDataMap.clear()
        this.containerCount = 0

        this.pickableObjects = []
        this.materialCache.clear()
        this.modelCache.clear()

        this.textureCache.forEach(texture => texture.dispose())
        this.textureCache.clear()

        this.disposables.forEach(item => item.dispose())
        this.disposables = []

        // Resetar helpers
        this.deselectObject()
    }

    createWater() {
        const waterGeometry = new THREE.PlaneGeometry(200, 400);

        this.waterMesh = new Water(
            waterGeometry,
            {
                textureWidth: 512,
                textureHeight: 512,
                waterNormals: new THREE.TextureLoader().load('/textures/waternormals.jpg', function (texture) {
                    texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
                }),
                sunDirection: new THREE.Vector3(),
                sunColor: 0xffffff,
                waterColor: 0x001e0f,
                distortionScale: 3.7,
                fog: this.scene.fog !== undefined
            }
        );

        this.waterMesh.rotation.x = -Math.PI / 2;
        this.waterMesh.position.y = -0.5;
        this.scene.add(this.waterMesh);
    }

    /**
     * Handle canvas click for object selection
     */
    private onCanvasClick(event: MouseEvent) {
        const canvas = this.renderer.domElement
        const rect = canvas.getBoundingClientRect()

        this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
        this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1

        this.raycaster.setFromCamera(this.mouse, this.camera)

        const objectsToCheck: THREE.Object3D[] = [...this.pickableObjects]
        if (this.containerInstancedMesh) {
            objectsToCheck.push(this.containerInstancedMesh)
        }

        const intersects = this.raycaster.intersectObjects(objectsToCheck, false)
        if (intersects.length > 0) {
            const intersection = intersects[0]
            const clickedObject = intersection.object
            const instanceId = intersection.instanceId !== undefined ? intersection.instanceId : null

            this.selectObject(clickedObject, instanceId)
        } else {
            this.deselectObject()
        }
    }

    /**
     * Handle canvas mouse move
     */
    private onCanvasMouseMove(event: MouseEvent) {
        const canvas = this.renderer.domElement
        const rect = canvas.getBoundingClientRect()
        this.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
        this.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1

        this.raycaster.setFromCamera(this.mouse, this.camera)

        const objectsToCheck: THREE.Object3D[] = [...this.pickableObjects]
        if (this.containerInstancedMesh) {
            objectsToCheck.push(this.containerInstancedMesh)
        }

        const intersects = this.raycaster.intersectObjects(objectsToCheck, false)
        canvas.style.cursor = intersects.length > 0 ? 'pointer' : 'default'
    }

    /**
     * US 4.2.5: Atualiza o Spotlight para o objeto selecionado
     */
    private updateSpotlight(targetPosition: THREE.Vector3) {
        if (!this.selectionSpotlight) return;

        this.selectionSpotlight.visible = true;
        this.selectionSpotlight.target.position.copy(targetPosition);

        // Luz acima do objeto
        this.selectionSpotlight.position.set(
            targetPosition.x,
            targetPosition.y + 50,
            targetPosition.z + 10
        );

        this.selectionSpotlight.target.updateMatrixWorld();
    }

    /**
     * US 4.2.6: Centra a câmara (preparado para animação)
     */
/**    public focusOnObject(object: THREE.Object3D | null, targetPosition?: THREE.Vector3) {
        const center = new THREE.Vector3();

        if (targetPosition) {
            center.copy(targetPosition);
        } else if (object) {
            const box = new THREE.Box3().setFromObject(object);
            if (box.isEmpty()) {
                center.copy(object.position);
            } else {
                box.getCenter(center);
            }
        } else {
            return;
        }

        // Calcular nova posição
        const offset = new THREE.Vector3().subVectors(this.camera.position, this.controls.target);
        const newCamPos = new THREE.Vector3().addVectors(center, offset);

        // Aplicar (aqui poderias usar TWEEN se instalado)
        this.controls.target.copy(center);
        this.camera.position.copy(newCamPos);
        this.controls.update();

        // Atualizar spotlight
        if (this.selectionSpotlight) {
            this.updateSpotlight(center);
        }
    }
*/

  // This new version, uses a smoother transition
  // Also uses currentAnimationId to stop any panning when input is detected 
  public focusOnObject(object: THREE.Object3D | null, targetPosition?: THREE.Vector3) {
        if (this.currentAnimationId !== null) {
            cancelAnimationFrame(this.currentAnimationId);
            this.currentAnimationId = null;
        }

        const center = new THREE.Vector3();

        if (targetPosition) {
            center.copy(targetPosition);
        } else if (object) {
            const box = new THREE.Box3().setFromObject(object);
            if (box.isEmpty()) {
                center.copy(object.position);
            } else {
                box.getCenter(center);
            }
        } else {
            return;
        }

        if (this.selectionSpotlight) {
            this.updateSpotlight(center);
        }

        const startPos = this.camera.position.clone();
        const startTarget = this.controls.target.clone();
        
        const offset = new THREE.Vector3().subVectors(startPos, startTarget);
        const endCamPos = new THREE.Vector3().addVectors(center, offset);

        const duration = 1000;
        const startTime = performance.now();

        const animateMovement = () => {
            const now = performance.now();
            const progress = Math.min((now - startTime) / duration, 1);
            
            const ease = 1 - Math.pow(1 - progress, 3);

            this.camera.position.lerpVectors(startPos, endCamPos, ease);
            this.controls.target.lerpVectors(startTarget, center, ease);
            this.controls.update();

            if (progress < 1) {
                this.currentAnimationId = requestAnimationFrame(animateMovement);
            } else {
                this.currentAnimationId = null;
            }
        };

        this.currentAnimationId = requestAnimationFrame(animateMovement);
    }

    /**
     * Lógica principal de seleção (US 4.2.2 + Otimização)
     */
    private selectObject(object: THREE.Object3D, instanceId: number | null = null) {
        this.deselectObject();
        this.selectedObject = object;

        const targetCenter = new THREE.Vector3();
        let targetFound = false;

        // --- CASO 1: InstancedMesh (Contentores) ---
        if (object instanceof THREE.InstancedMesh && instanceId !== null) {
            const matrix = new THREE.Matrix4();
            object.getMatrixAt(instanceId, matrix);

            targetCenter.setFromMatrixPosition(matrix);
            targetFound = true;

            // Mover Helper Visual
            if (this.selectionHelper) {
                this.selectionHelper.position.copy(targetCenter);
                this.selectionHelper.rotation.setFromRotationMatrix(matrix);
                this.selectionHelper.visible = true;
            }

            // Notificar UI
            if (this.containerDataMap.has(instanceId)) {
                if (this.onObjectSelected) {
                    this.onObjectSelected(this.containerDataMap.get(instanceId));
                }
            }
        }

        // --- CASO 2: Objeto Normal (Navio, Grua, etc.) ---
        else if (object instanceof THREE.Mesh) {
            // Highlight Material
            const material = object.material as THREE.MeshStandardMaterial;
            if (material && 'emissive' in material) {
                material.emissive.setHex(0xffaa00);
            }

            // Calcular centro
            const box = new THREE.Box3().setFromObject(object);
            if (box.isEmpty()) {
                targetCenter.copy(object.position);
            } else {
                box.getCenter(targetCenter);
            }
            targetFound = true;

            // Encontrar UserData
            let current: THREE.Object3D | null = object;
            while (current && !current.userData.type) current = current.parent;

            if (current && current.userData && this.onObjectSelected) {
                this.onObjectSelected(current.userData);
            }
        }

        // Focar e Iluminar
        if (targetFound) {
            this.updateSpotlight(targetCenter);
            this.focusOnObject(null, targetCenter);
        }
    }

    /**
     * Limpar seleção e efeitos
     */
    private deselectObject() {
        // Limpar highlight material
        if (this.selectedObject && this.selectedObject instanceof THREE.Mesh && !(this.selectedObject instanceof THREE.InstancedMesh)) {
            const material = this.selectedObject.material as THREE.MeshStandardMaterial;
            if (material && 'emissive' in material) {
                material.emissive.setHex(0x000000);
            }
        }

        // Esconder Helper
        if (this.selectionHelper) {
            this.selectionHelper.visible = false;
        }

        this.selectedObject = null;

        // Desligar Spotlight
        if (this.selectionSpotlight) {
            this.selectionSpotlight.visible = false;
        }

        if (this.onObjectSelected) {
            this.onObjectSelected(null);
        }
    }

    private generateMaterialCacheKey(config: TextureConfig): string {
        const parts: string[] = [
            config.colorMap || 'no-color',
            config.roughnessMap || 'no-roughness',
            config.bumpMap || 'no-bump',
            config.normalMap || 'no-normal',
            `r${config.roughness || 0.7}`,
            `m${config.metalness || 0.3}`
        ]
        return parts.join('|')
    }

    createMaterial(textureConfig: TextureConfig): THREE.MeshStandardMaterial {
        const cacheKey = this.generateMaterialCacheKey(textureConfig)
        if (this.materialCache.has(cacheKey)) {
            return this.materialCache.get(cacheKey)!
        }

        const material = new THREE.MeshStandardMaterial({
            roughness: textureConfig.roughness || 0.7,
            metalness: textureConfig.metalness || 0.3
        })

        this.materialCache.set(cacheKey, material)
        this.disposables.push(material)

        if (textureConfig.colorMap) {
            this.textureLoader.load(textureConfig.colorMap, (t) => { material.map = t; material.needsUpdate = true })
        }
        if (textureConfig.roughnessMap) {
            this.textureLoader.load(textureConfig.roughnessMap, (t) => { material.roughnessMap = t; material.needsUpdate = true })
        }
        if (textureConfig.bumpMap) {
            this.textureLoader.load(textureConfig.bumpMap, (t) => { material.bumpMap = t; material.bumpScale = 0.5; material.needsUpdate = true })
        }
        if (textureConfig.normalMap) {
            this.textureLoader.load(textureConfig.normalMap, (t) => { material.normalMap = t; material.needsUpdate = true })
        }

        return material
    }

    addDock(dock: Dock3D) {
        if (dock.model) {
            this.loadModel(dock.model, dock.position, dock.rotation, this.docksGroup, dock.dto.name, dock.dimensions)
        } else {
            let material;
            if (dock.texture) {
                material = this.createMaterial(dock.texture);
            } else {
                material = new THREE.MeshStandardMaterial({
                    color: 0x555555,
                    roughness: 0.9
                });
            }

            const mesh = new THREE.Mesh(this.sharedGeometries.unitBox, material)

            // LÓGICA DE ALTURA:
            // Queremos o topo em Y=+2.
            // Se a altura do bloco for 6, metade é 3.
            // Posição Y = -1. (Topo = -1 + 3 = +2). (Fundo = -1 - 3 = -4).
            const height = 6;
            const surfaceLevel = 2.2;
            const centerY = surfaceLevel - (height / 2);

            mesh.position.set(dock.position.x, centerY, dock.position.z)
            mesh.scale.set(dock.dimensions.width, height, dock.dimensions.depth)
            if (dock.rotation) mesh.rotation.set(dock.rotation.x, dock.rotation.y, dock.rotation.z)
            mesh.userData = { type: 'dock', id: dock.dto.id, name: dock.dto.name, typeLabel: 'Dock', dto: dock.dto }
            this.docksGroup.add(mesh)
            this.pickableObjects.push(mesh)
        }
    }

    addWarehouse(warehouse: Warehouse3D) {
        if (warehouse.model) {
            this.loadModel(warehouse.model, warehouse.position, warehouse.rotation, this.warehousesGroup, warehouse.dto.businessId, warehouse.dimensions)
        } else {
            const baseMaterial = warehouse.texture ? this.createMaterial(warehouse.texture) : new THREE.MeshStandardMaterial({ color: 0xA0A0A0 })
            const material = baseMaterial.clone()
            const mesh = new THREE.Mesh(this.sharedGeometries.unitBox, material)

            const height = 6;
            const surfaceLevel = 2.2;
            const centerY = surfaceLevel - (height / 2);

            mesh.position.set(warehouse.position.x, centerY, warehouse.position.z)
            mesh.scale.set(warehouse.dimensions.width, height, warehouse.dimensions.depth)
            if (warehouse.rotation) mesh.rotation.set(warehouse.rotation.x, warehouse.rotation.y, warehouse.rotation.z)
            mesh.userData = { type: 'warehouse', id: warehouse.dto.businessId, name: `Armazém ${warehouse.dto.businessId}`, typeLabel: 'Warehouse', dto: warehouse.dto }
            this.warehousesGroup.add(mesh)
            this.pickableObjects.push(mesh)
        }
    }

    addYard(yard: Yard3D) {
        if (yard.model) {
            this.loadModel(yard.model, yard.position, yard.rotation, this.yardsGroup, yard.dto.businessId, yard.dimensions)
        } else {
            const baseMaterial = yard.texture ? this.createMaterial(yard.texture) : new THREE.MeshStandardMaterial({ color: 0x606060 })
            const material = baseMaterial.clone()
            const mesh = new THREE.Mesh(this.sharedGeometries.unitPlane, material)
            mesh.position.set(yard.position.x, yard.position.y, yard.position.z)
            mesh.scale.set(yard.dimensions.width, yard.dimensions.depth, 1)
            mesh.rotation.x = -Math.PI / 2
            if (yard.rotation) mesh.rotation.y = yard.rotation.y
            mesh.userData = { type: 'yard', id: yard.dto.businessId, name: `Pátio ${yard.dto.businessId}`, typeLabel: 'Container Yard', dto: yard.dto }
            this.yardsGroup.add(mesh)
            this.pickableObjects.push(mesh)
        }
    }

    addVessel(vessel: Vessel3D) {
        // Definir altura de calado (parte submersa)
        // Se a água está em -0.5, o navio deve estar em -1 ou -2 para parecer dentro de água
        const draftY = 7.0;

        // CRIAR UM NOVO OBJETO DE POSIÇÃO COM O Y FORÇADO
        const visualPosition = {
            x: vessel.position.x,
            y: draftY,
            z: vessel.position.z
        };

        if (vessel.model) {
            // USAR visualPosition AQUI EM VEZ DE vessel.position
            this.loadModel(
                vessel.model,
                visualPosition,
                vessel.rotation,
                this.vesselsGroup,
                vessel.vessel.name,
                vessel.dimensions
            )
        } else {
            // Fallback para caixas
            const geometry = new THREE.BoxGeometry(vessel.dimensions.width, vessel.dimensions.height, vessel.dimensions.depth)
            const material = vessel.texture ? this.createMaterial(vessel.texture) : new THREE.MeshStandardMaterial({ color: 0x0088FF })
            const mesh = new THREE.Mesh(geometry, material)

            // USAR visualPosition AQUI TAMBÉM
            mesh.position.set(visualPosition.x, visualPosition.y, visualPosition.z)

            if (vessel.rotation) mesh.rotation.set(vessel.rotation.x, vessel.rotation.y, vessel.rotation.z)
            mesh.userData = { type: 'vessel', id: vessel.vvn?.id || vessel.vessel.id, name: vessel.vessel.name, typeLabel: 'Vessel', dto: vessel.vessel, vvn: vessel.vvn }
            this.vesselsGroup.add(mesh)
            this.pickableObjects.push(mesh)
        }
    }

    addCrane(crane: Crane3D) {
        if (crane.model) {
            this.loadModel(crane.model, crane.position, crane.rotation, this.cranesGroup, crane.dto.code, crane.dimensions)
        } else {
            const baseMaterial = crane.texture ? this.createMaterial(crane.texture) : new THREE.MeshStandardMaterial({ color: crane.type === 'STS' ? 0xFF8800 : 0xFFCC00 })
            const material = baseMaterial.clone()
            const mesh = new THREE.Mesh(this.sharedGeometries.unitBox, material)
            mesh.position.set(crane.position.x, crane.position.y, crane.position.z)
            mesh.scale.set(crane.dimensions.width, crane.dimensions.height, crane.dimensions.depth)
            if (crane.rotation) mesh.rotation.set(crane.rotation.x, crane.rotation.y, crane.rotation.z)
            mesh.userData = { type: 'crane', id: crane.dto.code, name: `Grua ${crane.dto.code}`, typeLabel: `Crane ${crane.type}`, dto: crane.dto }
            this.cranesGroup.add(mesh)
            this.pickableObjects.push(mesh)
        }
    }

    loadModel(modelPath: string, position: { x: number; y: number; z: number }, rotation: { x: number; y: number; z: number } | undefined, group: THREE.Group, name: string, dimentions: { width: number; height: number; depth: number }) {
        if (this.modelCache.has(modelPath)) {
            const cachedModel = this.modelCache.get(modelPath)!.clone()
            this.applyModelTransform(cachedModel, position, rotation, group, name, dimentions)
            return
        }

        this.gltfLoader.load(modelPath, (gltf) => {
            const model = gltf.scene
            this.modelCache.set(modelPath, model.clone())
            this.applyModelTransform(model, position, rotation, group, name)
        }, undefined, (err) => console.error(`Failed to load model: ${modelPath}`, err))
    }

    private applyModelTransform(model: THREE.Group, position: { x: number; y: number; z: number }, rotation: { x: number; y: number; z: number } | undefined, group: THREE.Group, name: string, dimensions?: { width: number; height: number; depth: number}) {
        // 1. Resetar escala original do modelo
        model.scale.set(1, 1, 1)

        // 2. Calcular a bounding box atual do modelo 3D
        const box = new THREE.Box3().setFromObject(model)
        const size = box.getSize(new THREE.Vector3())

        // 3. Lógica de Escala Inteligente
        if (dimensions && size.y > 0) {
            // Se temos dimensões definidas (ex: altura 15m para STS), usamos isso.
            // Usamos a ALTURA (height) como referência principal para manter a proporção.
            const scale = dimensions.height / size.y
            model.scale.set(scale, scale, scale)

            console.log(`📏 Escala ajustada para ${name}: ${scale.toFixed(3)} (Altura alvo: ${dimensions.height}m)`)
        }
        else {
            // Fallback antigo APENAS se não houver dimensões (para compatibilidade)
            const maxDim = Math.max(size.x, size.y, size.z)
            let targetSize = 10

            if (group.name === 'Vessels') targetSize = 40
            else if (group.name === 'Warehouses') targetSize = 25
            // else if (group.name === 'Cranes') targetSize = 15 // REMOVIDO para evitar conflitos

            if (maxDim > 0) {
                const scale = targetSize / maxDim
                model.scale.set(scale, scale, scale)
            }
        }


        model.position.set(position.x, position.y, position.z)
        if (rotation) model.rotation.set(rotation.x, rotation.y, rotation.z)
        model.name = name

        let userData: any = { type: group.name.toLowerCase().slice(0, -1), id: name, name: name }
        // Ajustar labels
        if (group.name === 'Vessels') userData.typeLabel = 'Vessel';
        if (group.name === 'Cranes') userData.typeLabel = 'Crane';

        model.userData = userData
        model.traverse((child) => {
            if (child instanceof THREE.Mesh) {
                child.castShadow = true;
                child.receiveShadow = true;

                child.userData = { ...userData }
                this.pickableObjects.push(child)
            }
        })
        group.add(model)
    }

    /**
     * Inicializa sistema de contentores instanciados
     */
    initContainerSystem(maxCount: number = 5000) {
        const texturePath = '/textures/containers/container.jpg'
        let texture = this.textureCache.get(texturePath)
        if (!texture) {
            texture = this.textureLoader.load(texturePath)
            this.textureCache.set(texturePath, texture)
        }

        const material = new THREE.MeshStandardMaterial({ map: texture, roughness: 0.7, metalness: 0.3 })
        this.containerInstancedMesh = new THREE.InstancedMesh(this.sharedGeometries.containerBox, material, maxCount)
        this.containerInstancedMesh.name = 'instanced_containers'
        this.containerInstancedMesh.userData = { type: 'container', typeLabel: 'Container' }
        this.containersGroup.add(this.containerInstancedMesh)
    }

    updateContainers(containersData: Array<{ x: number; z: number; y?: number; rotation?: number }>) {
        if (!this.containerInstancedMesh) {
            this.initContainerSystem(Math.max(5000, containersData.length))
        }

        let index = 0
        for (const container of containersData) {
            this.containerDummy.position.set(container.x, (container.y || 0) + 3.4, container.z)
            this.containerDummy.rotation.y = container.rotation || 0
            this.containerDummy.updateMatrix()
            this.containerInstancedMesh!.setMatrixAt(index, this.containerDummy.matrix)

            const containerId = `CNT-${Math.floor(container.x)}-${Math.floor(container.z)}`
            this.containerDataMap.set(index, {
                type: 'container',
                id: containerId,
                name: `Container ${containerId}`,
                typeLabel: 'Container'
            })
            index++
        }
        this.containerInstancedMesh!.count = index
        this.containerInstancedMesh!.instanceMatrix.needsUpdate = true
        this.containerCount = index
    }

    /**
     * Anima a transferência de um contentor de um ponto A para B
     * Simula o movimento de uma grua STS (Up -> Move -> Down)
     */
    public animateContainerTransfer(startPos: THREE.Vector3, endPos: THREE.Vector3, duration: number = 3000) {
        // 1. Criar um contentor temporário para a animação
        // Usamos a geometria partilhada já existente no seu código
        const geometry = this.sharedGeometries.containerBox;
        const material = new THREE.MeshStandardMaterial({
            color: 0x3366ff, // Azul destaque
            roughness: 0.7
        });

        const movingContainer = new THREE.Mesh(geometry, material);
        movingContainer.castShadow = true;

        // Posição inicial
        movingContainer.position.copy(startPos);
        this.scene.add(movingContainer);

        // 2. Definir alturas de segurança (altura da grua)
        const liftHeight = 25; // Altura para passar por cima de pilhas

        // --- Sequência de Animação (Tweening) ---

        // Fase 3: Descer até o destino
        const tweenDown = new TWEEN.Tween(movingContainer.position)
            .to({ y: endPos.y }, duration * 0.3)
            .easing(TWEEN.Easing.Quadratic.Out)
            .onComplete(() => {
                // Ao terminar, removemos o contentor temporário
                // (Na vida real, aqui você atualizaria os dados lógicos do porto)
                this.scene.remove(movingContainer);
                material.dispose();
                console.log("Transferência concluída");
            });

        // Fase 2: Mover horizontalmente
        const tweenMove = new TWEEN.Tween(movingContainer.position)
            .to({ x: endPos.x, z: endPos.z }, duration * 0.4)
            .easing(TWEEN.Easing.Linear.None) // Movimento constante da grua
            .chain(tweenDown); // Inicia a descida após chegar

        // Fase 1: Subir (Lift)
        const tweenUp = new TWEEN.Tween(movingContainer.position)
            .to({ y: liftHeight }, duration * 0.3)
            .easing(TWEEN.Easing.Quadratic.Out)
            .chain(tweenMove) // Inicia o movimento lateral após subir
            .start(); // DISPARAR ANIMAÇÃO
    }

    render() {
        this.controls.update()
        TWEEN.update()
        this.renderer.render(this.scene, this.camera)
        if (this.waterMesh) {
            // Aceder ao material.uniforms['time']
            (this.waterMesh.material as THREE.ShaderMaterial).uniforms['time'].value += 1.0 / 60.0;
        }
    }

    onWindowResize() {
        this.camera.aspect = window.innerWidth / window.innerHeight
        this.camera.updateProjectionMatrix()
        this.renderer.setSize(window.innerWidth, window.innerHeight)
    }

    dispose() {
        this.clearScene()
        this.controls.dispose()
        this.renderer.dispose()
        this.scene.clear()
    }
}
