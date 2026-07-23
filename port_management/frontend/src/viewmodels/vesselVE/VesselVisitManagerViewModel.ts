import { ref } from 'vue';
import VesselVisitService, { IVesselVisit, IComplementaryTask } from '@/service/VesselVisitService';

export class VesselVisitManagerViewModel {
    visit = ref<IVesselVisit | null>(null);
    tasks = ref<IComplementaryTask[]>([]);

    // Forms
    arrivalForm = ref({ date: '' });
    berthForm = ref({ date: '', dockId: '' });
    departForm = ref({ unberthDate: '', departDate: '' });
    taskForm = ref({ category: '', description: '', start: '', end: '' });

    isLoading = ref(false);
    message = ref('');
    errorMessage = ref(''); // <--- NOVO: Variável para guardar erros

    async loadData(id: string) {
        this.isLoading.value = true;
        this.errorMessage.value = ''; // Limpar erros anteriores

        try {
            console.log("Loading visit data for ID:", id);
            const data = await VesselVisitService.getById(id);

            if (!data) {
                this.errorMessage.value = `Visit not found for ID: ${id}`;
                return;
            }

            this.visit.value = data;

            // Tentar carregar tarefas (sem falhar a página toda se der erro)
            try {
                this.tasks.value = await VesselVisitService.getTasks(id);
            } catch (taskError) {
                console.warn("Could not load tasks:", taskError);
            }

        } catch(e: any) {
            console.error(e);
            this.errorMessage.value = e.message || "Failed to load visit data. Please check connection.";
        }
        finally {
            this.isLoading.value = false;
        }
    }

    // US 4.1.7
    async registerArrival() {
        if (!this.visit.value) return;
        try {
            await VesselVisitService.registerArrival(this.visit.value.vvnId, this.arrivalForm.value.date);
            this.message.value = "Arrival Registered";
            await this.loadData(this.visit.value.id);
        } catch (e: any) { this.errorMessage.value = e.message; }
    }

    // US 4.1.8
    async registerBerth() {
        if (!this.visit.value) return;
        try {
            await VesselVisitService.registerBerthing(this.visit.value.id, this.berthForm.value.date, this.berthForm.value.dockId);
            this.message.value = "Berthing Registered";
            await this.loadData(this.visit.value.id);
        } catch (e: any) { this.errorMessage.value = e.message; }
    }

    // US 4.1.11
    async registerDeparture() {
        if (!this.visit.value) return;
        try {
            await VesselVisitService.registerDeparture(this.visit.value.id, this.departForm.value.unberthDate, this.departForm.value.departDate);
            this.message.value = "Visit Closed";
            await this.loadData(this.visit.value.id);
        } catch (e: any) { this.errorMessage.value = e.message; }
    }

    // US 4.1.15
    async logTask() {
        if (!this.visit.value) return;
        try {
            await VesselVisitService.addTask({
                vveId: this.visit.value.id,
                categoryCode: this.taskForm.value.category,
                description: this.taskForm.value.description,
                startTime: this.taskForm.value.start,
                endTime: this.taskForm.value.end
            });
            this.message.value = "Task Logged";
            this.tasks.value = await VesselVisitService.getTasks(this.visit.value.id);
            this.taskForm.value = { category: '', description: '', start: '', end: '' }; // Limpar form
        } catch (e: any) { this.errorMessage.value = e.message; }
    }
}