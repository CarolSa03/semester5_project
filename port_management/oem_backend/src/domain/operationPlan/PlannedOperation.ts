import {Entity} from "../../core/domain/Entity";
import {OperationId} from "./value-objects/OperationId.vo";
import {OperationType} from "./enums/OperationType.enum";
import {OperationStatus} from "./enums/OperationStatus.enum";
import {TimeRange} from "../shared/TimeRange.vo";

interface PlannedOperationProps {
    type: OperationType;
    status: OperationStatus;
    timeWindow: TimeRange;
    resourceId: string;
}

export class PlannedOperation extends Entity<PlannedOperationProps> {
    get id(): OperationId {
        return this._id as OperationId;
    }

    get type(): OperationType { return this.props.type; }
    get status(): OperationStatus { return this.props.status; }
    get timeWindow(): TimeRange { return this.props.timeWindow; }
    get resourceId(): string { return this.props.resourceId; }

    private constructor(props: PlannedOperationProps, id?: OperationId) {
        super(props, id);
    }

    public static create(
        type: OperationType,
        timeWindow: TimeRange,
        resourceId: string,
        id?: OperationId
    ): PlannedOperation {
        const props: PlannedOperationProps = {
            type,
            status: OperationStatus.PLANNED,
            timeWindow,
            resourceId
        };
        // Se passarmos ID usa-o, senão gera um novo
        return new PlannedOperation(props, id || OperationId.generate());
    }

    // --- NOVO METODO: Permite atualizar o estado da operação ---
    public updateStatus(newStatus: OperationStatus): void {
        this.props.status = newStatus;
    }
}