// src/services/mockSchedulingData.ts

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

export function getMockVessels(): MockVessel[] {
    return [
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
}

export function getMockVesselsSmall(): MockVessel[] {
    return [
        { imo: 'IMO9000001', eta: 6, etd: 63, cargo: 260 },
        { imo: 'IMO9000002', eta: 23, etd: 50, cargo: 160 },
        { imo: 'IMO9000003', eta: 8, etd: 40, cargo: 170 },
        { imo: 'IMO9000004', eta: 27, etd: 40, cargo: 80 },
        { imo: 'IMO9000005', eta: 36, etd: 70, cargo: 120 }
    ]
}

export function getMockVesselsLarge(): MockVessel[] {
    return [
        ...getMockVessels(),
        { imo: 'IMO9000011', eta: 90, etd: 140, cargo: 400 },
        { imo: 'IMO9000012', eta: 112, etd: 140, cargo: 150 },
        { imo: 'IMO9000013', eta: 82, etd: 135, cargo: 250 },
        { imo: 'IMO9000014', eta: 95, etd: 145, cargo: 190 },
        { imo: 'IMO9000015', eta: 105, etd: 155, cargo: 210 }
    ]
}

export function getMockSTSCranes(): MockResource[] {
    return [
        {
            code: 'STS-001',
            capacity: 40,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['STS_CRANE_OPERATOR']
        },
        {
            code: 'STS-002',
            capacity: 40,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['STS_CRANE_OPERATOR']
        },
        {
            code: 'STS-003',
            capacity: 35,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['STS_CRANE_OPERATOR']
        },
        {
            code: 'STS-004',
            capacity: 45,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['STS_CRANE_OPERATOR']
        }
    ]
}

export function getMockYardCranes(): MockResource[] {
    return [
        {
            code: 'YC-001',
            capacity: 25,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['YARD_CRANE_OPERATOR']
        },
        {
            code: 'YC-002',
            capacity: 25,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['YARD_CRANE_OPERATOR']
        },
        {
            code: 'YC-003',
            capacity: 20,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['YARD_CRANE_OPERATOR']
        },
        {
            code: 'YC-004',
            capacity: 20,
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['YARD_CRANE_OPERATOR']
        }
    ]
}

export function getMockStaff(): MockStaffMember[] {
    return [
        {
            id: 'STAFF-001',
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['STS_CRANE_OPERATOR', 'SAFETY_CERTIFIED']
        },
        {
            id: 'STAFF-002',
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['STS_CRANE_OPERATOR', 'YARD_CRANE_OPERATOR']
        },
        {
            id: 'STAFF-003',
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['YARD_CRANE_OPERATOR']
        },
        {
            id: 'STAFF-004',
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['STS_CRANE_OPERATOR']
        },
        {
            id: 'STAFF-005',
            status: 'available',
            operationalWindowStart: 0,
            operationalWindowEnd: 168,
            qualifications: ['YARD_CRANE_OPERATOR', 'SAFETY_CERTIFIED']
        }
    ]
}

export function getMockStorageAreas(): MockStorageArea[] {
    return [
        { code: 'YARD-A', type: 'yard', capacity: 5000 },
        { code: 'YARD-B', type: 'yard', capacity: 4500 },
        { code: 'YARD-C', type: 'yard', capacity: 6000 },
        { code: 'WH-001', type: 'warehouse', capacity: 3000 },
        { code: 'WH-002', type: 'warehouse', capacity: 2000 }
    ]
}