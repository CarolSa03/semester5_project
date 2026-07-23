import type { STSCraneDto, YardCraneDto } from '@/types/physicalResources'

/**
 * Mock data for STS (Ship-to-Shore) Cranes
 * Used when backend is unavailable or returns empty data
 */
export const mockSTSCranes: STSCraneDto[] = [
  {
    id: 'sts-1',
    code: 'STS-001',
    description: 'Ship-to-Shore Crane 1',
    area: 'Dock Norte',
    setupTime: 30,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-STS-01'],
    status: 'Available',
    capacity: '65',
    capacityUnit: 'tons'
  },
  {
    id: 'sts-2',
    code: 'STS-002',
    description: 'Ship-to-Shore Crane 2',
    area: 'Dock Norte',
    setupTime: 30,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-STS-01'],
    status: 'Available',
    capacity: '65',
    capacityUnit: 'tons'
  },
  {
    id: 'sts-3',
    code: 'STS-003',
    description: 'Ship-to-Shore Crane 3',
    area: 'Dock Sul',
    setupTime: 30,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-STS-01'],
    status: 'Available',
    capacity: '65',
    capacityUnit: 'tons'
  },
  {
    id: 'sts-4',
    code: 'STS-004',
    description: 'Ship-to-Shore Crane 4',
    area: 'Dock Central',
    setupTime: 30,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-STS-01'],
    status: 'Available',
    capacity: '65',
    capacityUnit: 'tons'
  },
  {
    id: 'sts-5',
    code: 'STS-005',
    description: 'Ship-to-Shore Crane 5',
    area: 'Dock Central',
    setupTime: 30,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-STS-01'],
    status: 'Available',
    capacity: '65',
    capacityUnit: 'tons'
  }
]

/**
 * Mock data for Yard Cranes (RTG/RMG)
 * Used when backend is unavailable or returns empty data
 */
export const mockYardCranes: YardCraneDto[] = [
  {
    id: 'yard-1',
    code: 'YC-001',
    description: 'Yard Crane 1',
    area: 'YD-001',
    setupTime: 15,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-YC-01'],
    status: 'Available',
    capacity: '40',
    capacityUnit: 'tons'
  },
  {
    id: 'yard-2',
    code: 'YC-002',
    description: 'Yard Crane 2',
    area: 'YD-001',
    setupTime: 15,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-YC-01'],
    status: 'Available',
    capacity: '40',
    capacityUnit: 'tons'
  },
  {
    id: 'yard-3',
    code: 'YC-003',
    description: 'Yard Crane 3',
    area: 'YD-002',
    setupTime: 15,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-YC-01'],
    status: 'Available',
    capacity: '40',
    capacityUnit: 'tons'
  },
  {
    id: 'yard-4',
    code: 'YC-004',
    description: 'Yard Crane 4',
    area: 'YD-002',
    setupTime: 15,
    operationalWindow: '00:00-23:59',
    requiredQualificationIds: ['QUAL-YC-01'],
    status: 'Available',
    capacity: '40',
    capacityUnit: 'tons'
  }
]
