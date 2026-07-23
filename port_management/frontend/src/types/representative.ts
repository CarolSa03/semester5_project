// Shipping Agent Representative DTOs and Types

export interface RepresentativeDto {
  id?: string
  name: string
  email: string
  phone: string
  shippingAgentOrganizationId: string
  createdAt?: string
  updatedAt?: string
}

export interface RepresentativeFormData {
  name: string
  email: string
  phone: string
  shippingAgentOrganizationId: string
}
