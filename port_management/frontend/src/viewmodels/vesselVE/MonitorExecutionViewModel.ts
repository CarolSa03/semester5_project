import { ref, computed } from 'vue';
import VesselVisitService, { IRegisterExecutionDTO } from '@/service/VesselVisitService';
import OperationPlanService from '@/service/OperationPlanService';

export class MonitorExecutionViewModel {
  // Search
  vvnId = ref<string>('');

  // Data
  operations = ref<any[]>([]);

  // Modal State
  showRegisterModal = ref(false);
  selectedOp = ref<any>(null);
  executionForm = ref({
    realResource: '',
    realStartTime: '',
    realEndTime: '',
    status: 'COMPLETED'
  });

  // UI State
  isLoading = ref(false);
  isSubmitting = ref(false);
  errorMessage = ref('');
  successMessage = ref('');
  hasSearched = ref(false);

  // Validation
  isFormValid = computed(() => {
    return this.executionForm.value.realResource !== '' &&
        this.executionForm.value.realStartTime !== '' &&
        this.executionForm.value.realEndTime !== '';
  });

  async loadExecutionData() {
    if (!this.vvnId.value) {
      this.errorMessage.value = 'Please enter a VVN ID';
      return;
    }

    this.isLoading.value = true;
    this.errorMessage.value = '';
    this.hasSearched.value = true;
    this.operations.value = [];

    try {
      // 1. Obter o plano original
      const plan = await OperationPlanService.getPlanByVVN(this.vvnId.value);
      if (!plan) throw new Error(`No plan found for VVN: ${this.vvnId.value}`);

      // 2. CORREÇÃO: Usar getAll filtrado em vez de getVesselVisitExecution
      // Buscamos as visitas que correspondem a este VVN ID
      const visits = await VesselVisitService.getAll({ vvnId: this.vvnId.value });

      // Assumimos que a primeira encontrada é a correta (ou null se não houver execução iniciada)
      const vve = visits.length > 0 ? visits[0] : null;

      const executedMap = new Set(vve?.executedOperations?.map((op: any) => op.operationId) || []);

      // 3. Merge dos dados
      this.operations.value = plan.schedule.map((op: any) => ({
        ...op,
        isExecuted: executedMap.has(op.operationId) || op.status === 'COMPLETED'
      }));

    } catch (error: any) {
      this.errorMessage.value = error.message || 'Failed to load execution data';
    } finally {
      this.isLoading.value = false;
    }
  }

  openRegisterModal(op: any) {
    this.selectedOp.value = op;
    this.errorMessage.value = '';
    this.successMessage.value = '';

    const now = new Date();
    const oneHourLater = new Date(now.getTime() + 3600000);
    const toISO = (d: Date) => {
      const offset = d.getTimezoneOffset() * 60000;
      return new Date(d.getTime() - offset).toISOString().slice(0, 16);
    };

    this.executionForm.value = {
      realResource: op.resourceId || '',
      realStartTime: toISO(now),
      realEndTime: toISO(oneHourLater),
      status: 'COMPLETED'
    };
    this.showRegisterModal.value = true;
  }

  async confirmExecution() {
    if (!this.selectedOp.value) return;
    this.isSubmitting.value = true;

    try {
      const dto: IRegisterExecutionDTO = {
        vvnId: this.vvnId.value,
        operationId: this.selectedOp.value.operationId,
        realStartTime: new Date(this.executionForm.value.realStartTime).toISOString(),
        realEndTime: new Date(this.executionForm.value.realEndTime).toISOString(),
        realResource: this.executionForm.value.realResource,
        status: this.executionForm.value.status as any,
        completedBy: 'Operator'
      };

      // CORREÇÃO: Usar o novo nome do metodo
      await VesselVisitService.registerOperationExecution(dto);

      this.successMessage.value = 'Operation registered successfully!';
      this.showRegisterModal.value = false;
      await this.loadExecutionData();

    } catch (error: any) {
      this.errorMessage.value = error.message;
    } finally {
      this.isSubmitting.value = false;
    }
  }

  formatDate(iso: string) { return iso ? new Date(iso).toLocaleString() : '-'; }
}