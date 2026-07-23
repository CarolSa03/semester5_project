import { ref, computed } from 'vue';
import OperationPlanService from '@/service/OperationPlanService';
import { IOperationPlan } from '@/entities/OperationPlan';

export class OperationPlanListViewModel {
    // Search filters
    startDate = ref<string>('');
    endDate = ref<string>('');
    vesselId = ref<string>('');

    // Data
    operationPlans = ref<IOperationPlan[]>([]);

    // UI state
    isLoading = ref<boolean>(false);
    errorMessage = ref<string>('');

    // Sorting
    sortBy = ref<'startTime' | 'vesselName' | 'delay'>('startTime');
    sortDirection = ref<'asc' | 'desc'>('asc');

    /**
     * Computed: Sorted and filtered operation plans
     */
    sortedPlans = computed(() => {
        const plans = [...this.operationPlans.value];

        // Sort plans
        plans.sort((a, b) => {
            let comparison = 0;

            // Proteção contra schedule undefined/null
            const getStartTime = (plan: IOperationPlan) =>
                (plan.schedule && plan.schedule.length > 0) ? plan.schedule[0].startTime : 0;

            const getVesselId = (plan: IOperationPlan) =>
                plan.vvnId || (plan.schedule && plan.schedule.length > 0 ? plan.schedule[0].vesselId : '');

            switch (this.sortBy.value) {
                case 'startTime':
                    comparison = getStartTime(a) - getStartTime(b);
                    break;
                case 'vesselName':
                    comparison = getVesselId(a).localeCompare(getVesselId(b));
                    break;
                case 'delay':
                    const delayA = a.metrics ? a.metrics.totalDelay : 0;
                    const delayB = b.metrics ? b.metrics.totalDelay : 0;
                    comparison = delayA - delayB;
                    break;
            }

            return this.sortDirection.value === 'asc' ? comparison : -comparison;
        });

        return plans;
    });

    /**
     * US 4.1.3 - Search operation plans
     */
    async searchPlans(): Promise<void> {
        this.isLoading.value = true;
        this.errorMessage.value = '';

        try {
            if (this.startDate.value && this.endDate.value) {
                // Search by date range
                // NOTA: O Serviço tem de ter este metodo implementado
                this.operationPlans.value = await OperationPlanService.getOperationPlansByDateRange(
                    this.startDate.value,
                    this.endDate.value
                );
            } else {
                // Get all plans
                this.operationPlans.value = await OperationPlanService.getAllOperationPlans();
            }

            // Filter by vessel if specified
            if (this.vesselId.value.trim()) {
                const searchTerm = this.vesselId.value.toLowerCase();
                this.operationPlans.value = this.operationPlans.value.filter(plan => {
                    // 1. Procura no ID do Plano (VVN-XXXX) - MAIS IMPORTANTE
                    const matchesPlanId = plan.vvnId && plan.vvnId.toLowerCase().includes(searchTerm);

                    // 2. Procura nas operações (caso existam)
                    const matchesOperations = plan.schedule && plan.schedule.some(op =>
                        op.vesselId && op.vesselId.toLowerCase().includes(searchTerm)
                    );

                    return matchesPlanId || matchesOperations;
                });
            }
        } catch (error: any) {
            this.errorMessage.value = error.message || 'Failed to search operation plans';
            console.error('Search error:', error);
        } finally {
            this.isLoading.value = false;
        }
    }

    /**
     * Load all plans on init
     */
    async loadAllPlans(): Promise<void> {
        this.isLoading.value = true;
        this.errorMessage.value = '';

        try {
            this.operationPlans.value = await OperationPlanService.getAllOperationPlans();
        } catch (error: any) {
            this.errorMessage.value = error.message || 'Failed to load operation plans';
            console.error('Load error:', error);
        } finally {
            this.isLoading.value = false;
        }
    }

    changeSortBy(column: 'startTime' | 'vesselName' | 'delay'): void {
        if (this.sortBy.value === column) {
            this.sortDirection.value = this.sortDirection.value === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortBy.value = column;
            this.sortDirection.value = 'asc';
        }
    }

    resetFilters(): void {
        this.startDate.value = '';
        this.endDate.value = '';
        this.vesselId.value = '';
        this.errorMessage.value = '';
        this.loadAllPlans();
    }

    formatDate(dateString: string): string {
        if (!dateString) return 'N/A';
        return new Date(dateString).toLocaleDateString();
    }

    getPlanSummary(plan: IOperationPlan): string {
        if (!plan.schedule) return 'No operations';
        const numOperations = plan.schedule.length;
        // Safe access to vesselId
        const uniqueVessels = new Set(plan.schedule.map(op => op.vesselId).filter(v => v));
        return `${uniqueVessels.size} vessel(s), ${numOperations} operation(s)`;
    }
}