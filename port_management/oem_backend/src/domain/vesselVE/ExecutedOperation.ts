import {Entity} from "../../core/domain/Entity";
import {UniqueEntityID} from "../../core/domain/UniqueEntityID";
import {OperationId} from "../operationPlan/value-objects/OperationId.vo";
import {TimeRange} from "../shared/TimeRange.vo";
import {OperationStatus} from "../operationPlan/enums/OperationStatus.enum";

interface ExecutedOperationProps {
    operationId: OperationId; // Referência à operação original do plano
    realTimeWindow: TimeRange; // O tempo real que demorou
    realResource: string; // O recurso que foi efetivamente usado
    status: OperationStatus; // COMPLETED ou DELAYED
    completedBy: string; // ID do utilizador que registou
    registeredAt: Date; // Timestamp do registo
}

export class ExecutedOperation extends Entity<ExecutedOperationProps> {

    get operationId(): OperationId { return this.props.operationId; }
    get realTimeWindow(): TimeRange { return this.props.realTimeWindow; }
    get realResource(): string { return this.props.realResource; }
    get status(): OperationStatus { return this.props.status; }
    get completedBy(): string { return this.props.completedBy; }
    get registeredAt(): Date { return this.props.registeredAt; }

    private constructor(props: ExecutedOperationProps, id?: UniqueEntityID) {
        super(props, id);
    }

    public static create(props: {
        operationId: OperationId;
        realTimeWindow: TimeRange;
        realResource: string;
        status: OperationStatus;
        completedBy: string;
    }, id?: UniqueEntityID): ExecutedOperation {

        return new ExecutedOperation({
            ...props,
            registeredAt: new Date()
        }, id);
    }

    // Metodo para permitir correções posteriores (auditoria simples)
    public updateExecution(
        newTimeWindow: TimeRange,
        newResource: string,
        newStatus: OperationStatus
    ): void {
        this.props.realTimeWindow = newTimeWindow;
        this.props.realResource = newResource;
        this.props.status = newStatus;
        this.props.registeredAt = new Date(); // Atualiza data de modificação
    }
}