export interface AppUser {
    id: string
    iamUserId: string
    email: string
    name: string
    isActive: boolean
    activatedOn: string | null
    createdAt: string
    updatedAt: string
    roles: string[]
}

export interface GoogleLoginResponse {
    credential: string
    select_by?: string
}
