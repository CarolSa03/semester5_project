// Mock data for scheduling algorithms (Prolog integration)

export interface MockVessel {
    imo: string
    eta: number
    etd: number
    cargo: number
}

export interface MockResource {
    code: string
    capacity: number
    status: string
    operationalWindowStart?: number
    operationalWindowEnd?: number
    qualifications?: string[]
}

export interface MockStaffMember {
    id: string
    status: string
    operationalWindowStart?: number
    operationalWindowEnd?: number
    qualifications?: string[]
}

export interface MockStorageArea {
    code: string
    type: string
    capacity: number
}

export const mockVesselsScheduling: MockVessel[] = [
    { imo: 'IMO9000001', eta: 6, etd: 63, cargo: 260 },
    { imo: 'IMO9000002', eta: 23, etd: 50, cargo: 160 },
    { imo: 'IMO9000003', eta: 8, etd: 40, cargo: 170 },
    { imo: 'IMO9000004', eta: 27, etd: 40, cargo: 80 },
    { imo: 'IMO9000005', eta: 36, etd: 70, cargo: 120 },
    { imo: 'IMO9000006', eta: 40, etd: 60, cargo: 140 },
    { imo: 'IMO9000007', eta: 52, etd: 80, cargo: 190 },
    { imo: 'IMO9000008', eta: 61, etd: 90, cargo: 210 },
    { imo: 'IMO9000009', eta: 74, etd: 100, cargo: 140 },
    { imo: 'IMO9000010', eta: 81, etd: 110, cargo: 140 }
]

export const mockVesselsSchedulingSmall: MockVessel[] = [
    { imo: 'IMO9000001', eta: 6, etd: 63, cargo: 260 },
    { imo: 'IMO9000002', eta: 23, etd: 50, cargo: 160 },
    { imo: 'IMO9000003', eta: 8, etd: 40, cargo: 170 }
]

export const mockVesselsSchedulingLarge: MockVessel[] = [
    { imo: 'IMO9000001', eta: 6, etd: 63, cargo: 260 },
    { imo: 'IMO9000002', eta: 23, etd: 50, cargo: 160 },
    { imo: 'IMO9000003', eta: 8, etd: 40, cargo: 170 },
    { imo: 'IMO9000004', eta: 27, etd: 40, cargo: 80 },
    { imo: 'IMO9000005', eta: 36, etd: 70, cargo: 120 },
    { imo: 'IMO9000006', eta: 40, etd: 60, cargo: 140 },
    { imo: 'IMO9000007', eta: 52, etd: 80, cargo: 190 },
    { imo: 'IMO9000008', eta: 61, etd: 90, cargo: 210 },
    { imo: 'IMO9000009', eta: 74, etd: 100, cargo: 140 },
    { imo: 'IMO9000010', eta: 81, etd: 110, cargo: 140 },
    { imo: 'IMO9000011', eta: 90, etd: 120, cargo: 180 },
    { imo: 'IMO9000012', eta: 95, etd: 130, cargo: 150 },
    { imo: 'IMO9000013', eta: 100, etd: 140, cargo: 200 },
    { imo: 'IMO9000014', eta: 110, etd: 150, cargo: 170 },
    { imo: 'IMO9000015', eta: 120, etd: 160, cargo: 220 }
]

export const mockResources: MockResource[] = [
    { code: 'STS-001', capacity: 65, status: 'Available' },
    { code: 'STS-002', capacity: 65, status: 'Available' },
    { code: 'YC-001', capacity: 40, status: 'Available' },
    { code: 'YC-002', capacity: 40, status: 'Available' }
]

export const mockStaffMembers: MockStaffMember[] = [
    { id: 'STAFF-001', status: 'Available', qualifications: ['QUAL-STS-01'] },
    { id: 'STAFF-002', status: 'Available', qualifications: ['QUAL-STS-01'] },
    { id: 'STAFF-003', status: 'Available', qualifications: ['QUAL-YC-01'] },
    { id: 'STAFF-004', status: 'Available', qualifications: ['QUAL-YC-01'] }
]

export const mockStorageAreasScheduling: MockStorageArea[] = [
    { code: 'WH-001', type: 'Warehouse', capacity: 500 },
    { code: 'WH-002', type: 'Warehouse', capacity: 400 },
    { code: 'YD-001', type: 'Yard', capacity: 1000 },
    { code: 'YD-002', type: 'Yard', capacity: 800 }
]
