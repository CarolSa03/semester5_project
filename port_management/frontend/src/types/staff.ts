// Staff Member DTOs and Types

export interface QualificationDto {
  code: number
  descriptiveName: string
}

export interface StaffMemberDto {
  id?: string
  staffMemberId: string
  shortName: string
  email: string
  phoneNumber: string
  contactDetails?: string
  qualifications: QualificationDto[]
  isAvailable: boolean
  createdAt?: string
  updatedAt?: string
}

export interface StaffFormData {
  id?: string
  staffMemberId: string
  shortName: string
  email: string
  phoneNumber: string
  qualifications: string[]
  isAvailable: boolean
}
