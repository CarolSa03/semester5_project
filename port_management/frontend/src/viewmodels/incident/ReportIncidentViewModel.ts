import { ref, computed } from 'vue';
import IncidentService, { IIncident, IIncidentType } from '@/service/IncidentService';
import VesselVisitService from '@/service/VesselVisitService'; // <--- Importar o Serviço de Visitas

export class ReportIncidentViewModel {
    searchInput = ref('');
    vvnId = ref('');       // Só preenchemos se a visita for válida

    activeIncidents = ref<IIncident[]>([]);
    availableTypes = ref<IIncidentType[]>([]);

    // Modal State
    showCreateModal = ref(false);
    incidentForm = ref({ typeCode: '', description: '', severity: '' });

    isLoading = ref(false);
    errorMessage = ref('');
    successMessage = ref('');

    isFormValid = computed(() => this.incidentForm.value.typeCode !== '' && this.incidentForm.value.description !== '');

    async searchByVessel() {
        // Limpar estados anteriores
        this.errorMessage.value = '';
        this.successMessage.value = '';
        this.activeIncidents.value = [];
        this.vvnId.value = ''; // Reset ao ID confirmado

        const input = this.searchInput.value.trim();
        if(!input) {
            this.errorMessage.value = "Please enter a Vessel Visit ID.";
            return;
        }

        this.isLoading.value = true;

        try {
            // PASSO 1: Validar Existência da Visita
            // Chamamos o serviço de visitas para ver se este VVN existe no sistema (Porto ou Local)
            const visits = await VesselVisitService.getAll({ vvnId: input });

            // Verificamos se encontrámos alguma visita com esse ID exato
            const validVisit = visits.find(v => v.vvnId === input);

            if (!validVisit) {
                // Se não existe, lançamos erro e paramos aqui.
                throw new Error(`Vessel Visit '${input}' not found. Please check the ID.`);
            }

            // PASSO 2: Se existe, confirmamos o ID e buscamos os incidentes
            this.vvnId.value = validVisit.vvnId;

            this.activeIncidents.value = await IncidentService.getIncidentsByVVN(this.vvnId.value);

            // Pré-carregar tipos para o modal
            if (this.availableTypes.value.length === 0) {
                this.availableTypes.value = await IncidentService.getIncidentTypes();
            }

        } catch (e: any) {
            this.errorMessage.value = e.message;
            // Garante que a secção de "Active Incidents" não aparece em caso de erro
            this.vvnId.value = '';
        } finally {
            this.isLoading.value = false;
        }
    }

    openCreateModal() {
        this.incidentForm.value = { typeCode: '', description: '', severity: '' };
        this.showCreateModal.value = true;
    }

    async submitIncident() {
        this.isLoading.value = true;
        try {
            await IncidentService.createIncident({
                incidentTypeCode: this.incidentForm.value.typeCode,
                description: this.incidentForm.value.description,
                severity: this.incidentForm.value.severity || undefined,
                affectedVvnIds: [this.vvnId.value], // Usa o ID já validado
                createdBy: 'CurrentUser'
            });
            this.successMessage.value = 'Incident Reported Successfully';
            this.showCreateModal.value = false;

            // Recarrega a lista para mostrar o novo incidente
            this.activeIncidents.value = await IncidentService.getIncidentsByVVN(this.vvnId.value);

        } catch (e: any) { this.errorMessage.value = e.message; }
        finally { this.isLoading.value = false; }
    }

    async resolveIncident(id: string) {
        if(!confirm('Mark as resolved?')) return;
        try {
            await IncidentService.resolveIncident(id, new Date().toISOString());
            // Recarrega a lista
            this.activeIncidents.value = await IncidentService.getIncidentsByVVN(this.vvnId.value);
        } catch(e:any) { this.errorMessage.value = e.message; }
    }
}