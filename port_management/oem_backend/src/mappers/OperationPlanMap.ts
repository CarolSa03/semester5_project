import {Mapper} from "../core/infra/Mapper";
import {OperationPlan} from "../domain/operationPlan/OperationPlan";
import {IOperationPlanDTO} from "../dto/IOperationPlanDTO";
import {PlannedOperation} from "../domain/operationPlan/PlannedOperation";
import {TimeRange} from "../domain/shared/TimeRange.vo";
import {OperationType} from "../domain/operationPlan/enums/OperationType.enum";
import {PlanId} from "../domain/operationPlan/value-objects/PlanId.vo";

export class OperationPlanMap extends Mapper<OperationPlan> {

    // --- DTO: Domínio -> JSON (Para o Frontend) ---
    public static toDTO(plan: OperationPlan): IOperationPlanDTO {
        if (!plan || !(plan as any).id) {
            return {
                id: '',
                vvnId: '',
                date: new Date().toISOString().split('T')[0],
                metrics: { algorithm: '', totalDelay: 0, computationTime: 0 },
                schedule: []
            } as IOperationPlanDTO;
        }

        return {
            id: plan.id.toString(),
            vvnId: plan.vvnId,
            date: plan.date.toISOString().split('T')[0],
            metrics: {
                algorithm: plan.algorithm,
                totalDelay: plan.totalDelay,
                computationTime: plan.computationTime
            },
            schedule: plan.operations.map(op => {
                const startTimeMs = op.timeWindow.getStartTime().getTime();
                const endTimeMs = op.timeWindow.getEndTime().getTime();

                return {
                    // ID CRÍTICO: Envia o ID para o frontend conseguir pedir updates
                    operationId: op.id.toString(),

                    vesselId: plan.vvnId,
                    resourceId: op.resourceId,
                    dockId: undefined, // Compatibilidade

                    startTime: startTimeMs,
                    endTime: endTimeMs,
                    durationMinutes: (endTimeMs - startTimeMs) / 60000,
                    assignedCranes: [op.resourceId],
                    delay: 0
                };
            })
        };
    }

    // --- DOMAIN: JSON/BD -> Entidade ---
    public static toDomain(raw: any): OperationPlan | null {
        try {
            const vvnId = (raw.schedule && raw.schedule[0]?.vesselId) || raw.vvnId;
            if (!vvnId) {
                // console.warn(`[Mapper] Ignorado registo sem vvnId`);
                return null;
            }

            const rawDateStr = typeof raw.date === 'string' ? raw.date : new Date(raw.date).toISOString();
            const datePart = rawDateStr.split('T')[0];
            const [year, month, day] = datePart.split('-').map(Number);
            const baseTimestampUTC = Date.UTC(year, month - 1, day);

            // Tenta ler operations (novo) ou schedule (antigo)
            const sourceList = raw.operations || raw.schedule || [];
            const operations: PlannedOperation[] = [];

            for (const item of sourceList) {
                try {
                    let startTime: Date;
                    let endTime: Date;
                    const resourceId = item.resourceId || item.dockId || "UNKNOWN";

                    // Detecção robusta de data (String ISO, Objeto Date ou Timestamp numérico)
                    if (item.startTime instanceof Date) {
                        startTime = item.startTime;
                        endTime = item.endTime;
                    } else if (typeof item.startTime === 'string' && item.startTime.includes('T')) {
                        startTime = new Date(item.startTime);
                        endTime = new Date(item.endTime);
                    } else {
                        // Assumindo minutos desde meia-noite (mock antigo)
                        const startMs = baseTimestampUTC + (Number(item.startTime) * 60000);
                        const endMs = baseTimestampUTC + (Number(item.endTime) * 60000);
                        startTime = new Date(startMs);
                        endTime = new Date(endMs);
                    }

                    const timeWindow = TimeRange.create(startTime, endTime);

                    // Recuperar o ID da BD (_id ou operationId ou id)
                    const existingId = item._id || item.operationId || item.id;

                    const op = PlannedOperation.create(
                        OperationType.UNLOADING,
                        timeWindow,
                        resourceId,
                        // Se a classe PlannedOperation permitir passar ID no create, passe aqui.
                        // Caso contrário, injetamos abaixo.
                    );

                    // Atualizar estado
                    if (item.status) op.updateStatus(item.status);

                    // INJEÇÃO DE ID: Garante que o domínio mantém o mesmo ID da BD
                    if (existingId) {
                        (op as any)._id = existingId;
                    }

                    operations.push(op);
                } catch (e) {
                    console.warn(`[Mapper] Falha ops: ${e}`);
                }
            }

            const algorithm = raw.algorithm || (raw.metrics && raw.metrics.algorithm) || "genetic";
            const planDate = new Date(baseTimestampUTC);

            const planOrError = OperationPlan.create({
                vvnId: vvnId,
                date: planDate,
                algorithm: algorithm,
                totalDelay: raw.totalDelay || (raw.metrics && raw.metrics.totalDelay) || 0,
                computationTime: raw.computationTime || (raw.metrics && raw.metrics.computationTime) || 0,
                operations: operations
            }, raw.domainId ? PlanId.create(raw.domainId) : undefined);

            if (planOrError.isFailure) return null;

            return planOrError.getValue();
        } catch (err) {
            console.error("❌ [Mapper] Erro Crítico:", err);
            return null;
        }
    }

    // --- PERSISTENCE: Domínio -> BD (A CORREÇÃO PRINCIPAL) ---
    public static toPersistence(plan: OperationPlan): any {
        return {
            domainId: plan.id.toString(),
            vvnId: plan.vvnId,
            date: plan.date,
            algorithm: plan.algorithm,
            status: plan.status,
            totalDelay: plan.totalDelay,
            computationTime: plan.computationTime,

            // Auditoria
            lastModified: plan.lastModified || new Date(),
            lastModifiedBy: plan.lastModifiedBy,
            modificationReason: plan.modificationReason,

            // Mapeamento de Operações com _id explícito
            operations: plan.operations.map(op => ({
                _id: op.id.toString(),        // <--- ISTO É FUNDAMENTAL PARA O MONGO
                operationId: op.id.toString(), // Redundância útil para queries

                type: op.type,
                status: op.status,

                // Garantir que são objetos Date reais
                startTime: op.timeWindow.getStartTime(),
                endTime: op.timeWindow.getEndTime(),

                resourceId: op.resourceId
            }))
        };
    }
}