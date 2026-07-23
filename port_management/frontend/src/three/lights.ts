import * as THREE from 'three'

/**
 * US 3.3.5 - Lighting System
 * Setup ambient + directional lights for visibility and realism
 */
export function setupLights(scene: THREE.Scene) {
    // 1. Ambient Light - Uniform base illumination
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.7)
    scene.add(ambientLight)

    // 2. Directional Light - Simulates sunlight
    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.5)
    directionalLight.position.set(50, 100, 50) // Elevated position at 45° angle
    directionalLight.castShadow = false // Disabled for better performance

    scene.add(directionalLight)

    return { ambientLight, directionalLight }
}
