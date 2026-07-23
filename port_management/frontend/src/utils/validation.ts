export interface DockFormData {
    name: string
    location: string
    length?: number
    depth?: number
    maxDraft?: number
    maxSTS?: number
    allowedVesselTypes: number[]
}

export interface ValidationErrors {
    [key: string]: string
}

export function validateDockForm(form: DockFormData): ValidationErrors {
    const errors: ValidationErrors = {}

    if (!form.name?.trim()) {
        errors.name = "Name is required"
    }

    if (!form.location?.trim()) {
        errors.location = "Location is required"
    }

    if (!form.length || form.length < 1) {
        errors.length = "Length must be at least 1 meter"
    }

    if (!form.depth || form.depth < 1) {
        errors.depth = "Depth must be at least 1 meter"
    }

    if (!form.maxDraft || form.maxDraft < 1) {
        errors.maxDraft = "Max Draft must be at least 1 meter"
    }

    if (form.maxSTS === undefined || form.maxSTS < 0) {
        errors.maxSTS = "Max STS operations is required and cannot be negative"
    }

    if (!form.allowedVesselTypes || form.allowedVesselTypes.length === 0) {
        errors.allowedVesselTypes = "Select at least one vessel type"
    }

    return errors
}

export function validateRequired(value: string, fieldName: string): string {
    if (!value?.trim()) {
        return `${fieldName} is required`
    }
    return ""
}
