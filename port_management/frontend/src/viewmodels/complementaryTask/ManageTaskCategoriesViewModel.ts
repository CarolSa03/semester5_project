import { ref } from 'vue';
import VesselVisitService, { ITaskCategory } from '@/service/VesselVisitService';

export class ManageTaskCategoriesViewModel {
    categories = ref<ITaskCategory[]>([]);
    isLoading = ref(false);
    errorMessage = ref('');

    // Create Form
    showModal = ref(false);
    newCategory = ref({ code: '', name: '', description: '' });
    private successMessage = ref('');

    async loadCategories() {
        this.isLoading.value = true;
        try {
            this.categories.value = await VesselVisitService.getTaskCategories();
        } catch(e:any) { this.errorMessage.value = e.message; }
        finally { this.isLoading.value = false; }
    }

    openCreateModal() {
        this.newCategory.value = { code: '', name: '', description: '' };
        this.showModal.value = true;
    }

    async createCategory() {
        this.isLoading.value = true;
        try {
            // Chamada real ao serviço
            await VesselVisitService.createTaskCategory(this.newCategory.value);

            this.successMessage.value = "Category created successfully!";
            this.showModal.value = false;
            await this.loadCategories();
        } catch (e: any) {
            this.errorMessage.value = e.message;
        } finally {
            this.isLoading.value = false;
        }
    }
}