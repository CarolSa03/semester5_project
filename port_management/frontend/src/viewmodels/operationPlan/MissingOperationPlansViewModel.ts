import { ref, computed } from 'vue';
import OperationPlanService from '@/service/OperationPlanService';

export interface IMissingPlan {
    vvnId: string;
    vesselId: string;
    vesselName?: string;
    arrivalDate: string;
    departureDate: string;
    dockId?: string;
    status: string;
    totalContainers: number;
}

export class MissingOperationPlansViewModel {
    // Filter state
    startDate = ref<string>('');
    endDate = ref<string>('');

    // Data
    missingPlans = ref<IMissingPlan[]>([]);

    // UI state
    isLoading = ref<boolean>(false);
    errorMessage = ref<string>('');

    // Selection for bulk generation
    selectedVvnIds = ref<Set<string>>(new Set());

    /**
     * Computed: Count of missing plans
     */
    missingCount = computed(() => this.missingPlans.value.length);

    /**
     * Computed: Are all plans selected?
     */
    allSelected = computed(() => {
        return this.missingPlans.value.length > 0 &&
            this.selectedVvnIds.value.size === this.missingPlans.value.length;
    });

    /**
     * US 4.1.5 - Load missing operation plans
     */
    async loadMissingPlans(): Promise<void> {
        this.isLoading.value = true;
        this.errorMessage.value = '';

        try {
            this.missingPlans.value = await OperationPlanService.getMissingOperationPlans(
                this.startDate.value || undefined,
                this.endDate.value || undefined
            );
        } catch (error: any) {
            this.errorMessage.value = error.message || 'Failed to load missing operation plans';
            console.error('Load error:', error);
        } finally {
            this.isLoading.value = false;
        }
    }

    /**
     * Apply date filters
     */
    async applyFilters(): Promise<void> {
        await this.loadMissingPlans();
    }

    /**
     * Reset filters
     */
    resetFilters(): void {
        this.startDate.value = '';
        this.endDate.value = '';
        this.selectedVvnIds.value.clear();
        this.loadMissingPlans();
    }

    /**
     * Toggle single VVN selection
     */
    toggleSelection(vvnId: string): void {
        if (this.selectedVvnIds.value.has(vvnId)) {
            this.selectedVvnIds.value.delete(vvnId);
        } else {
            this.selectedVvnIds.value.add(vvnId);
        }
    }

    /**
     * Toggle all selections
     */
    toggleSelectAll(): void {
        if (this.allSelected.value) {
            this.selectedVvnIds.value.clear();
        } else {
            this.missingPlans.value.forEach(plan => {
                this.selectedVvnIds.value.add(plan.vvnId);
            });
        }
    }

    /**
     * Check if a VVN is selected
     */
    isSelected(vvnId: string): boolean {
        return this.selectedVvnIds.value.has(vvnId);
    }

    /**
     * Get selected VVNs count
     */
    getSelectedCount(): number {
        return this.selectedVvnIds.value.size;
    }

    /**
     * Clear selections
     */
    clearSelections(): void {
        this.selectedVvnIds.value.clear();
    }

    /**
     * Format date for display
     */
    formatDate(dateString: string): string {
        return new Date(dateString).toLocaleDateString();
    }

    /**
     * Get urgency level based on arrival date
     */
    getUrgency(arrivalDate: string): 'high' | 'medium' | 'low' {
        const arrival = new Date(arrivalDate);
        const now = new Date();
        const daysUntil = Math.ceil((arrival.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));

        if (daysUntil <= 2) return 'high';
        if (daysUntil <= 7) return 'medium';
        return 'low';
    }

    /**
     * Get urgency label
     */
    getUrgencyLabel(arrivalDate: string): string {
        const urgency = this.getUrgency(arrivalDate);
        const arrival = new Date(arrivalDate);
        const now = new Date();
        const daysUntil = Math.ceil((arrival.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));

        if (daysUntil < 0) return 'Overdue';
        if (daysUntil === 0) return 'Today';
        if (daysUntil === 1) return 'Tomorrow';
        return `${daysUntil} days`;
    }

    /**
     * Export to CSV
     */
    exportToCSV(): void {
        if (this.missingPlans.value.length === 0) return;

        const headers = ['VVN ID', 'Vessel ID', 'Vessel Name', 'Arrival Date', 'Departure Date', 'Dock', 'Total Containers', 'Status'];
        const rows = this.missingPlans.value.map(plan => [
            plan.vvnId,
            plan.vesselId,
            plan.vesselName || 'N/A',
            plan.arrivalDate,
            plan.departureDate,
            plan.dockId || 'N/A',
            plan.totalContainers.toString(),
            plan.status
        ]);

        const csvContent = [
            headers.join(','),
            ...rows.map(row => row.map(cell => `"${cell}"`).join(','))
        ].join('\n');

        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);

        link.setAttribute('href', url);
        link.setAttribute('download', `missing-operation-plans-${new Date().toISOString().split('T')[0]}.csv`);
        link.style.visibility = 'hidden';

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}
