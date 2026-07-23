// src1/services/vesselVE/VVEMetricsService.ts
import {Service} from 'typedi';
import {VesselVisitExecution} from '../../domain/vesselVE/VesselVisitExecution';
import {IVVEMetricsDTO} from '../../dto/vesselVE/IVVEMetricsDTO';

@Service()
export default class VVEMetricsService {

    public calculateMetrics(
        vve: VesselVisitExecution,
        vvnData?: { eta?: Date; etd?: Date; vesselName?: string },
        operationPlan?: any
    ): IVVEMetricsDTO {

        const metrics: IVVEMetricsDTO = {
            turnaroundTime: null,
            berthOccupancyTime: null,
            waitingTimeForBerthing: null,
            arrivalDelay: 0,
            departureDelay: 0,
            operationDelays: 0,
            totalOperations: 0,
            completedOperations: 0,
            delayedOperations: 0,
            progressPercentage: 0
        };

        const arrivalDate = vve.arrivalDate;
        const departureDate = vve.departureDate;
        const executedOperations = vve.executedOperations;

        // Tempos
        if (arrivalDate && departureDate) {
            metrics.turnaroundTime = this.calculateMinutesDifference(arrivalDate, departureDate);
            metrics.berthOccupancyTime = metrics.turnaroundTime;
        }

        // Atrasos
        if (arrivalDate && vvnData?.eta) {
            const delay = this.calculateMinutesDifference(vvnData.eta, arrivalDate);
            metrics.arrivalDelay = delay > 0 ? delay : 0;
        }
        if (departureDate && vvnData?.etd) {
            const delay = this.calculateMinutesDifference(vvnData.etd, departureDate);
            metrics.departureDelay = delay > 0 ? delay : 0;
        }

        // Operações e Progresso
        if (operationPlan && Array.isArray(operationPlan.operations)) {
            metrics.totalOperations = operationPlan.operations.length;
        }
        if (metrics.totalOperations === 0 && executedOperations.length > 0) {
            metrics.totalOperations = executedOperations.length;
        }

        metrics.completedOperations = executedOperations.filter(op => op.status.toString() === 'COMPLETED').length;
        metrics.delayedOperations = executedOperations.filter(op => op.status.toString() === 'DELAYED').length;

        if (metrics.totalOperations > 0) {
            metrics.progressPercentage = Math.round((metrics.completedOperations / metrics.totalOperations) * 100);
        }

        return metrics;
    }

    private calculateMinutesDifference(start: Date, end: Date): number {
        const diffMs = end.getTime() - start.getTime();
        return Math.round(diffMs / 60000);
    }
}