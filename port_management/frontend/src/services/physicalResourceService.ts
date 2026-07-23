import apiClient from './apiClient'
import type {
  PhysicalResourceDto,
  TruckDto,
  YardCraneDto,
  STSCraneDto
} from '../types/physicalResources'
import { mockSTSCranes, mockYardCranes } from '@/mocks'

export async function getAllResources() {
  const res = await apiClient.get<PhysicalResourceDto[]>('/PhysicalResource')
  return res.data
}

export async function getResourceById(id: string) {
  const res = await apiClient.get<PhysicalResourceDto>(`/PhysicalResource/${id}`)
  return res.data
}

export async function getTrucks() {
  const res = await apiClient.get<TruckDto[]>('/PhysicalResource/trucks')
  return res.data
}

export async function getYardCranes() {
  try {
    const res = await apiClient.get<YardCraneDto[]>('/PhysicalResource/yardcranes')
    // If backend returns empty array, use mocks
    if (!res.data || res.data.length === 0) {
      console.warn('Backend returned empty data, using mock yard cranes data')
      return mockYardCranes
    }
    return res.data
  } catch (error) {
    console.warn('Backend not available, using mock yard cranes data')
    return mockYardCranes
  }
}

export async function getSTSCranes() {
  try {
    const res = await apiClient.get<STSCraneDto[]>('/PhysicalResource/stscranes')
    // If backend returns empty array, use mocks
    if (!res.data || res.data.length === 0) {
      console.warn('Backend returned empty data, using mock STS cranes data')
      return mockSTSCranes
    }
    return res.data
  } catch (error) {
    console.warn('Backend not available, using mock STS cranes data')
    return mockSTSCranes
  }
}

// ------- CREATE --------

export async function createResource(dto: PhysicalResourceDto) {
  const res = await apiClient.post('/PhysicalResource', dto)
  return res.data
}

export async function createTruck(dto: TruckDto) {
  const res = await apiClient.post('/PhysicalResource/truck', dto)
  return res.data
}

export async function createYardCrane(dto: YardCraneDto) {
  const res = await apiClient.post('/PhysicalResource/yardcrane', dto)
  return res.data
}

export async function createSTSCrane(dto: STSCraneDto) {
  const res = await apiClient.post('/PhysicalResource/stscrane', dto)
  return res.data
}

// ------- UPDATE --------

export async function updateResource(id: string, dto: PhysicalResourceDto) {
  const res = await apiClient.put(`/PhysicalResource/${id}`, dto)
  return res.data
}

export async function deactivateResource(id: string) {
  const res = await apiClient.patch(`/PhysicalResource/${id}/deactivate`)
  return res.data
}



