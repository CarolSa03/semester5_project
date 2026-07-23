import {AggregateRoot} from "../../core/domain/AggregateRoot";
import {PlanId} from "./value-objects/PlanId.vo";
import {Result} from "../../core/logic/Result";
import {PlanStatus} from "./enums/PlanStatus.enum";
import {PlannedOperation} from "./PlannedOperation";
import {OperationStatus} from "./enums/OperationStatus.enum";

interface OperationTask {
    vesselId: string;
    dockId: string;
    startTime: number;
    endTime: number;
    assignedCranes: string[];
    delay: number;
}

interface OperationPlanProps {
    vvnId: string;
    date: Date;
    algorithm: string;
    totalDelay: number;
    computationTime: number;
    status: PlanStatus;
    operations: PlannedOperation[];
    lastModified?: Date;
    lastModifiedBy?: string;
    modificationReason?: string;
}

export class OperationPlan extends AggregateRoot<OperationPlanProps> {
    get id(): PlanId { return this._id as PlanId; }
    get vvnId(): string { return this.props.vvnId; }
    get date(): Date { return this.props.date; }
    get algorithm(): string { return this.props.algorithm; }
    get status(): PlanStatus { return this.props.status; }
    get totalDelay(): number { return this.props.totalDelay; }
    get operations(): PlannedOperation[] { return this.props.operations; } // Getter atualizado
    get computationTime(): number { return this.props.computationTime; }
    get lastModified(): Date | undefined { return this.props.lastModified; }
    get lastModifiedBy(): string | undefined { return this.props.lastModifiedBy; }
    get modificationReason(): string | undefined { return this.props.modificationReason; }

    private constructor(props: OperationPlanProps, id?: PlanId) {
        super(props, id);
    }

    public static create(props: {
        vvnId: string;
        date: Date;
        algorithm: string;
        totalDelay: number;
        computationTime: number;
        operations: PlannedOperation[]; 
    }, id?: PlanId): Result<OperationPlan> {
        // Validate vvnId
        if (!props.vvnId || props.vvnId.trim().length === 0) {
            return Result.fail<OperationPlan>("vvnId cannot be empty");
        }

        // Validate date
        if (!(props.date instanceof Date) || isNaN(props.date.getTime())) {
            return Result.fail<OperationPlan>("date must be a valid Date object");
        }
        //if (props.date < new Date(new Date().setHours(0, 0, 0, 0))) {
        //    return Result.fail<OperationPlan>("date cannot be in the past");
        //}

        // Validate algorithm
        if (!props.algorithm || props.algorithm.trim().length === 0) {
            return Result.fail<OperationPlan>("algorithm cannot be empty");
        }

        // Validate totalDelay
        if (props.totalDelay < 0) {
            return Result.fail<OperationPlan>("totalDelay cannot be negative");
        }

        // Validate computationTime
        if (props.computationTime < 0) {
            return Result.fail<OperationPlan>("computationTime cannot be negative");
        }

        // Validate operations
        if (!props.operations || props.operations.length === 0) {
            return Result.fail<OperationPlan>("operations array cannot be empty");
        }

        const plan = new OperationPlan({
            ...props,
            status: PlanStatus.ACTIVE
        }, id || PlanId.generate());
        return Result.ok<OperationPlan>(plan);
    }

    public updatePlan(updates: any, author: string, reason: string): void {
        if (updates.status) this.props.status = updates.status;
        if (updates.operations) this.props.operations = updates.operations;
        this.props.lastModified = new Date();
        this.props.lastModifiedBy = author;
        this.props.modificationReason = reason;
    }

    public updateOperationStatus(opId: string, newStatus: OperationStatus, author: string): Result<void> {
        // Encontra a operação pelo ID (convertendo para string se necessário)
        const operation = this.props.operations.find(op => op.id.toString() === opId);

        if (!operation) {
            return Result.fail<void>(`Operation ${opId} not found in plan for VVN ${this.vvnId}`);
        }

        // Atualiza a operação
        operation.updateStatus(newStatus);

        // Atualiza metadados de auditoria do plano
        this.props.lastModified = new Date();
        this.props.lastModifiedBy = author;
        this.props.modificationReason = `Execution Update: Operation ${opId} marked as ${newStatus}`;

        return Result.ok<void>();
    }
}