import {Mapper} from "../core/infra/Mapper";
import {VesselVisitExecution} from "../domain/vesselVE/VesselVisitExecution";
import {ExecutedOperation} from "../domain/vesselVE/ExecutedOperation";
import {UniqueEntityID} from "../core/domain/UniqueEntityID";
import {OperationId} from "../domain/operationPlan/value-objects/OperationId.vo";
import {TimeRange} from "../domain/shared/TimeRange.vo";
import {OperationStatus} from "../domain/operationPlan/enums/OperationStatus.enum";
import {ICreateVesselVisitExecutionDTO} from "../dto/vesselVE/ICreateVesselVisitExecutionDTO";

export class VesselVisitExecutionMap extends Mapper<VesselVisitExecution> {

    public static toPersistence(vve: VesselVisitExecution): any {
        return {
            vvnId: vve.vvnId,
            status: (vve.props as any).status, // Acesso direto se não houver getter público
            executedOperations: vve.executedOperations.map(op => ({
                operationId: op.operationId.toString(),
                realStartTime: op.realTimeWindow.getStartTime(),
                realEndTime: op.realTimeWindow.getEndTime(),
                realResource: op.realResource,
                status: op.status,
                completedBy: op.completedBy,
                registeredAt: op.registeredAt
            }))
        };
    }

    public static toDomain(raw: any): VesselVisitExecution | null {
        try {
            // 1. Reconstruir as operações filhas
            const executedOps = raw.executedOperations.map((op: any) => {
                return ExecutedOperation.create({
                    operationId: OperationId.create(op.operationId),
                    realTimeWindow: TimeRange.create(new Date(op.realStartTime), new Date(op.realEndTime)),
                    realResource: op.realResource,
                    status: op.status as OperationStatus,
                    completedBy: op.completedBy
                });
                // Nota: O 'registeredAt' será novo se usarmos o create(), 
                // idealmente devias passar o registeredAt original para dentro da entidade via props diretas
                // tal como fizemos no OperationPlanMap.
            });

            // 2. Reconstruir o Agregado Raiz
            const vveOrError = VesselVisitExecution.create({
                vvnId: raw.vvnId,
                arrivalDate: raw.arrivalDate ? new Date(raw.arrivalDate) : undefined
            }, new UniqueEntityID(raw._id)); // Usa o ID do Mongo se existir

            if (vveOrError.isFailure) {
                console.error(vveOrError.error);
                return null;
            }

            const vve = vveOrError.getValue();

            // Injetar o estado real que veio da BD
            // (Assumindo que tornas as props públicas ou usas reflection/any)
            (vve.props as any).executedOperations = executedOps;
            (vve.props as any).status = raw.status;

            return vve;
        } catch (err) {
            console.error("Error mapping VVE:", err);
            return null;
        }
    }


    public static toDTO(vve: VesselVisitExecution): ICreateVesselVisitExecutionDTO {
        return {
            vvnId: vve.vvnId,
            arrivalDate: vve.arrivalDate?.toISOString(),
            status: vve.status
        };
    }
}