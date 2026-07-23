import { ref, computed } from 'vue';
import IncidentService, { IIncidentType } from '@/service/IncidentService';

export class IncidentTypeViewModel {
    incidentTypes = ref<IIncidentType[]>([]);
    isLoading = ref(false);
    errorMessage = ref('');
    successMessage = ref('');

    // Modal State
    showCreateModal = ref(false);
    newType = ref({
        code: '',
        name: '',
        description: '',
        severity: 'Minor',
        parentId: ''
    });

    // Dropdown options
    parentOptions = computed(() => this.incidentTypes.value);

    // Validation
    isFormValid = computed(() => {
        return this.newType.value.code !== '' &&
            this.newType.value.name !== '' &&
            this.newType.value.description !== '';
    });

    async loadTypes() {
        this.isLoading.value = true;
        this.errorMessage.value = '';
        try {
            this.incidentTypes.value = await IncidentService.getIncidentTypes();
        } catch (e: any) {
            this.errorMessage.value = e.message;
        } finally {
            this.isLoading.value = false;
        }
    }

    openCreateModal() {
        this.newType.value = { code: '', name: '', description: '', severity: 'Minor', parentId: '' };
        this.showCreateModal.value = true;
        this.errorMessage.value = '';
        this.successMessage.value = '';
    }

    async createType() {
        this.isLoading.value = true;
        try {
            const payload: any = { ...this.newType.value };
            if (!payload.parentId) delete payload.parentId;

            await IncidentService.createIncidentType(payload);

            this.successMessage.value = 'Incident Type created successfully!';
            this.showCreateModal.value = false;
            await this.loadTypes();
        } catch (e: any) {
            this.errorMessage.value = e.message;
        } finally {
            this.isLoading.value = false;
        }
    }

    async removeType(id: string) {
        const confirmed = window.confirm('Are you sure you want to delete this Incident Type?');
        if (!confirmed) return;

        this.isLoading.value = true;
        this.errorMessage.value = '';
        try {
            await IncidentService.deleteIncidentType(id);
            this.successMessage.value = 'Incident Type deleted successfully!';
            await this.loadTypes();
        } catch (e: any) {
            this.errorMessage.value = e.message || 'Failed to delete Incident Type';
        } finally {
            this.isLoading.value = false;
        }
    }

    getParentName(parentId?: string): string {
        if (!parentId) return '-';
        const parent = this.incidentTypes.value.find(t => t.id === parentId);
        return parent ? parent.name : 'Unknown';
    }
}