import { ref } from 'vue';
import axios from 'axios';

export class ResourceAllocationViewModel {
  // Inputs
  resourceId = ref<string>('');
  startDate = ref<string>('');
  endDate = ref<string>('');

  // Outputs
  result = ref<{
    totalAllocatedTime: number; // in minutes (based on our backend logic)
    totalOperations: number;
    resourceId: string;
  } | null>(null);

  // States
  isLoading = ref(false);
  errorMessage = ref<string>('');

  async fetchAllocation(): Promise<void> {
    if (!this.resourceId.value || !this.startDate.value || !this.endDate.value) {
      this.errorMessage.value = "Please fill in all fields.";
      return;
    }

    this.isLoading.value = true;
    this.errorMessage.value = '';
    this.result.value = null;

    try {
      // Calls the Controller we created earlier: GET /api/operation-plans/allocation
      const response = await axios.get(`${import.meta.env.VITE_OEM_BASE_URL}/operation-plans/allocation`, {
        params: {
          resourceId: this.resourceId.value,
          start: this.startDate.value,
          end: this.endDate.value
        }
      });

      this.result.value = response.data;
    } catch (error: any) {
      console.error(error);
      this.errorMessage.value = error.response?.data?.message || "Failed to fetch allocation data.";
    } finally {
      this.isLoading.value = false;
    }
  }

  // Helper to format minutes into "Xh Ym"
  formatDuration(minutes: number): string {
    if (!minutes) return '0m';
    const h = Math.floor(minutes / 60);
    const m = Math.round(minutes % 60);
    return h > 0 ? `${h}h ${m}m` : `${m}m`;
  }
}
