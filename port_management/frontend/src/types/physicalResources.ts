// Base DTO
export interface PhysicalResourceDto {
  id?: string | null;
  code: string;
  description: string;
  area: string;
  setupTime: number;
  operationalWindow: string;
  requiredQualificationIds: string[];
  status: string;
}

// Truck
export interface TruckDto extends PhysicalResourceDto {
  capacity: string;
  capacityUnit: string; // "Containers/Trip"
  speed: string;
  speedUnit: string; // "Km/h"
}

// Yard Crane
export interface YardCraneDto extends PhysicalResourceDto {
  capacity: string;
  capacityUnit: string; // "Containers/Hour"
}

// STS Crane
export interface STSCraneDto extends PhysicalResourceDto {
  capacity: string;
  capacityUnit: string; // "Containers/Hour"
}

