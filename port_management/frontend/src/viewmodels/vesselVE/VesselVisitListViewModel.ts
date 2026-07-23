import { ref } from 'vue';
import VesselVisitService, { IVesselVisit } from '@/service/VesselVisitService';
import router from '@/router';

export class VesselVisitListViewModel {
    // CORREÇÃO: Renomeado de 'visits' para 'vesselVisits' para bater certo com a View
    vesselVisits = ref<IVesselVisit[]>([]);

    isLoading = ref(false);

    // Filtros
    startDate = ref('');
    endDate = ref('');
    vesselId = ref('');
    status = ref(''); // PENDING, IN_PROGRESS, COMPLETED

    /**
     * Carrega as visitas aplicando os filtros definidos
     */
    async loadVisits() {
        this.isLoading.value = true;
        try {
            // Constroi objeto de filtros para enviar ao Backend
            const filters: any = {};
            if (this.startDate.value) filters.startDate = this.startDate.value;
            if (this.endDate.value) filters.endDate = this.endDate.value;
            if (this.vesselId.value) filters.vesselId = this.vesselId.value;
            if (this.status.value) filters.status = this.status.value;

            // Chama o serviço que agora suporta a busca combinada (Porto + Execuções)
            this.vesselVisits.value = await VesselVisitService.getAll(filters);

        } catch (e) {
            console.error(e);
            this.vesselVisits.value = [];
        } finally {
            this.isLoading.value = false;
        }
    }

    /**
     * Formata data para string legível
     */
    formatDate(date?: string) {
        if (!date) return '-';
        return new Date(date).toLocaleString();
    }

    /**
     * Navega para os detalhes (Adicionado para corrigir erro TS)
     */
    viewDetails(id: string) {
        if (id) {
            router.push(`/oem/vessel-visits/${id}`);
        }
    }

    /**
     * Limpa os filtros e recarrega
     */
    resetFilters() {
        this.startDate.value = '';
        this.endDate.value = '';
        this.vesselId.value = '';
        this.status.value = '';
        this.loadVisits();
    }
}