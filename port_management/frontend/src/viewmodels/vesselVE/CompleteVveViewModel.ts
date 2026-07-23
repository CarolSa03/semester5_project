import { ref } from 'vue';
import vesselVisitService from '../../service/VesselVisitService';

export class CompleteVveViewModel {
  vvnId = ref<string>('');
  departureTime = ref<string>('');

  isLoading = ref(false);
  successMessage = ref<string>('');
  errorMessage = ref<string>('');

  initialize(id: string) {
    this.vvnId.value = id;
    
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    this.departureTime.value = now.toISOString().slice(0, 16);
  }

  async completeVisit(): Promise<boolean> {
    if (!this.departureTime.value) {
      this.errorMessage.value = "Departure time is required.";
      return false;
    }

    this.isLoading.value = true;
    this.errorMessage.value = '';
    this.successMessage.value = '';

    try {
      console.log(`Requesting Completion for ${this.vvnId.value}...`);

      await vesselVisitService.completeVisit(
        this.vvnId.value, 
        new Date(this.departureTime.value).toISOString()
      );

      this.successMessage.value = "Vessel Visit completed successfully.";
      return true;

    } catch (error: any) {
      console.error("Completion Error:", error);

      if (error.response && error.response.status === 409) {
        this.errorMessage.value = "Cannot complete visit: There are still pending cargo operations.";
      } else {
        this.errorMessage.value = error.response?.data?.error || error.response?.data?.message || "Failed to complete visit.";
      }
      return false;
    } finally {
      this.isLoading.value = false;
    }
  }
}
