import { ref } from 'vue';
import OperationPlanService from '@/service/OperationPlanService';
import { IOperationPlan, IPlannedOperation, IUpdateOperationPlanRequest } from '@/entities/OperationPlan';

export class UpdateOperationPlanViewModel {
    // Original plan data
    originalPlan = ref<IOperationPlan | null>(null);

    // Editable plan data
    editedOperations = ref<IPlannedOperation[]>([]);
    status = ref<'ACTIVE' | 'ARCHIVED' | 'SUPERSEDED'>('ACTIVE');

    // Update metadata
    author = ref<string>('');
    reason = ref<string>('');

    // UI state
    isLoading = ref<boolean>(false);
    isSaving = ref<boolean>(false);
    errorMessage = ref<string>('');
    successMessage = ref<string>('');
    showComparison = ref<boolean>(false);

    /**
     * Load operation plan by ID
     */
    async loadPlan(planId: string): Promise<void> {
        this.isLoading.value = true;
        this.errorMessage.value = '';

        try {
            const plan = await OperationPlanService.getOperationPlanById(planId);
            this.originalPlan.value = plan;

            // Create editable copies
            this.editedOperations.value = JSON.parse(JSON.stringify(plan.schedule));
            this.status.value = 'ACTIVE'; // Default status
        } catch (error: any) {
            this.errorMessage.value = error.message || 'Failed to load operation plan';
            console.error('Load error:', error);
        } finally {
            this.isLoading.value = false;
        }
    }

    /**
     * Update resource for an operation
     */
    updateOperationResource(operationIndex: number, newResource: string): void {
        if (this.editedOperations.value[operationIndex]) {
            // CORREÇÃO: Atualizar AMBOS os campos
            // 1. O resourceId é o que o Backend lê para gravar na BD
            (this.editedOperations.value[operationIndex] as any).resourceId = newResource;

            // 2. O assignedCranes é usado para visualização no frontend
            this.editedOperations.value[operationIndex].assignedCranes = [newResource];

            // Debug para confirmar
            console.log(`Resource updated for op ${operationIndex}:`, newResource);
        }
    }

    /**
     * Update time window for an operation
     */
    updateOperationTime(operationIndex: number, startTime: number, endTime: number): void {
        if (this.editedOperations.value[operationIndex]) {
            this.editedOperations.value[operationIndex].startTime = startTime;
            this.editedOperations.value[operationIndex].endTime = endTime;
            this.editedOperations.value[operationIndex].durationMinutes =
                Math.round((endTime - startTime) / 60000);
        }
    }

    /**
     * Check if any changes were made
     */
    hasChanges(): boolean {
        if (!this.originalPlan.value) return false;

        return JSON.stringify(this.originalPlan.value.schedule) !==
            JSON.stringify(this.editedOperations.value);
    }

    /**
     * Validate update data
     */
    validateUpdate(): string | null {
        if (!this.author.value.trim()) {
            return 'Author name is required';
        }

        if (!this.reason.value.trim()) {
            return 'Modification reason is required';
        }

        if (!this.hasChanges() && this.status.value === 'ACTIVE') {
            return 'No changes detected. Please modify operations or change status.';
        }

        return null;
    }

    /**
     * US 4.1.4 - Update operation plan
     */
    async updatePlan(): Promise<boolean> {
        const validationError = this.validateUpdate();
        if (validationError) {
            this.errorMessage.value = validationError;
            return false;
        }

        if (!this.originalPlan.value) {
            this.errorMessage.value = 'No plan loaded';
            return false;
        }

        this.isSaving.value = true;
        this.errorMessage.value = '';
        this.successMessage.value = '';

        try {
            const updateData: IUpdateOperationPlanRequest = {
                status: this.status.value,
                operations: this.editedOperations.value,
                author: this.author.value.trim(),
                reason: this.reason.value.trim()
            };

            const updatedPlan = await OperationPlanService.updateOperationPlan(
                this.originalPlan.value.id,
                updateData
            );

            this.successMessage.value = 'Operation plan updated successfully!';
            this.originalPlan.value = updatedPlan;
            return true;
        } catch (error: any) {
            this.errorMessage.value = error.message || 'Failed to update operation plan';
            console.error('Update error:', error);
            return false;
        } finally {
            this.isSaving.value = false;
        }
    }

    /**
     * Toggle comparison view
     */
    toggleComparison(): void {
        this.showComparison.value = !this.showComparison.value;
    }

    /**
     * Reset changes
     */
    resetChanges(): void {
        if (this.originalPlan.value) {
            this.editedOperations.value = JSON.parse(JSON.stringify(this.originalPlan.value.schedule));
            this.status.value = 'ACTIVE';
            this.errorMessage.value = '';
            this.successMessage.value = '';
        }
    }

    /**
     * Format timestamp
     */
    formatTimestamp(timestamp: number): string {
        return new Date(timestamp).toLocaleString();
    }

    /**
     * Format duration
     */
    formatDuration(minutes: number): string {
        const hours = Math.floor(minutes / 60);
        const mins = minutes % 60;
        return hours > 0 ? `${hours}h ${mins}m` : `${mins}m`;
    }
}
