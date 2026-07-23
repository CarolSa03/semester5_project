import {Service} from "typedi";
import {IPlanningExternalService} from "../services/IServices/IPlanningExternalService";
import {IPlanningModuleResponseDTO} from "../dto/IPlanningModuleResponseDTO";
import {Result} from "../core/logic/Result";
import {IOperationPlanDTO} from "../dto/IOperationPlanDTO";

@Service()
export default class PlanningModuleAdapter implements IPlanningExternalService {

    public async fetchSchedule(date: Date, algorithm?: string, vvnId?: string): Promise<Result<IOperationPlanDTO>> {
        try {
            const effectiveVvnId = vvnId || "VVN-MOCK-GENERIC";
            const data = this.generateMockSchedule(date, algorithm || "genetic", effectiveVvnId);

            if (!data.success || !data.schedule || data.schedule.length === 0) {
                return Result.fail<IOperationPlanDTO>("Planning module returned empty schedule");
            }

            const planDTO: IOperationPlanDTO = {
                id: "",
                vvnId: effectiveVvnId,
                date: date.toISOString(),
                metrics: {
                    algorithm: data.algorithm,
                    totalDelay: data.totalDelay,
                    computationTime: 1.5
                },
                schedule: data.schedule.map(s => {
                    // CORREÇÃO: Gerar ID válido (OPR-Timestamp-Random) manualmente ou via helper
                    const timestamp = Date.now();
                    const random = Math.random().toString(36).substring(2, 6).toUpperCase();
                    const validOpId = `OPR-${timestamp}-${random}`;

                    return {
                        operationId: validOpId, // Agora passa na validação!
                        vesselId: s.vesselId,
                        resourceId: s.dockId || "STS-GENERIC",
                        dockId: undefined,
                        startTime: s.startTime,
                        endTime: s.endTime,
                        durationMinutes: s.endTime - s.startTime,
                        assignedCranes: s.assignedCrane,
                        delay: s.delay
                    };
                })
            };

            return Result.ok<IOperationPlanDTO>(planDTO);

        } catch (e: unknown) {
            return Result.fail<IOperationPlanDTO>(String(e));
        }
    }

    private generateMockSchedule(date: Date, algorithm: string, targetVvnId: string): IPlanningModuleResponseDTO {
        const schedule = [
            {
                vesselId: targetVvnId,
                dockId: "STS-001", // Simulando uma Grua
                startTime: 600,
                endTime: 720,
                assignedCrane: ["STS-001"],
                delay: 0
            },
            {
                vesselId: targetVvnId,
                dockId: "STS-001",
                startTime: 780,
                endTime: 900,
                assignedCrane: ["STS-001"],
                delay: 0
            },
            {
                vesselId: targetVvnId,
                dockId: "STS-001",
                startTime: 960,
                endTime: 1080,
                assignedCrane: ["STS-001"],
                delay: 0
            }
        ];

        return {
            requestId: "mock-" + Date.now(),
            success: true,
            algorithm: algorithm,
            totalDelay: 0,
            schedule: schedule,
            warnings: []
        };
    }
}