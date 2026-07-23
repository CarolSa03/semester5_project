import { ref, computed } from 'vue';
import OperationPlanService from '@/service/OperationPlanService';
import { IOperationPlan, IGenerateOperationPlanRequest } from '@/entities/OperationPlan';

export class GenerateOperationPlanViewModel {
    vvnId = ref<string>('');
    date = ref<string>('');
    algorithm = ref<'genetic' | 'astar' | 'Genetic' | 'AStar'>('genetic');
    isLoading = ref<boolean>(false);
    errorMessage = ref<string>('');
    successMessage = ref<string>('');
    generatedPlan = ref<IOperationPlan | null>(null);
    showPreview = ref<boolean>(false);
    isFormValid = computed(() => {
        return this.vvnId.value.trim() !== '' && this.date.value !== '';
    });
    availableAlgorithms = [
        { value: 'genetic', label: 'Genetic Algorithm' },
        { value: 'astar', label: 'A* Algorithm' },
        { value: 'Genetic', label: 'Genetic (Capitalized)' },
        { value: 'AStar', label: 'A* (Capitalized)' }
    ];

    async generatePlan(): Promise<void> {
        if (!this.isFormValid.value) {
            this.errorMessage.value = 'Please fill in all required fields';
            return;
        }

        this.isLoading.value = true;
        this.errorMessage.value = '';
        this.successMessage.value = '';
        this.generatedPlan.value = null;

        try {
            const request: IGenerateOperationPlanRequest = {
                vvnId: this.vvnId.value.trim(),
                date: this.date.value,
                algorithm: this.algorithm.value
            };

            const plan = await OperationPlanService.generateOperationPlan(request);

            this.generatedPlan.value = plan;
            this.showPreview.value = true;
            this.successMessage.value = `Operation Plan generated successfully! Total Delay: ${plan.metrics.totalDelay} minutes`;
        } catch (error: any) {
            this.errorMessage.value = error.message || 'Failed to generate operation plan';
            console.error('Generate plan error:', error);
        } finally {
            this.isLoading.value = false;
        }
    }

    resetForm(): void {
        this.vvnId.value = '';
        this.date.value = '';
        this.algorithm.value = 'genetic';
        this.errorMessage.value = '';
        this.successMessage.value = '';
        this.generatedPlan.value = null;
        this.showPreview.value = false;
    }

    closePreview(): void {
        this.showPreview.value = false;
    }

    formatTimestamp(timestamp: number): string {
        return new Date(timestamp).toLocaleString();
    }

    calculateDuration(startTime: number, endTime: number): number {
        return Math.round((endTime - startTime) / 60000);
    }
}
