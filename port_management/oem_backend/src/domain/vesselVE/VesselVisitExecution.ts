import {AggregateRoot} from "../../core/domain/AggregateRoot";
import {UniqueEntityID} from "../../core/domain/UniqueEntityID";
import {ExecutedOperation} from "./ExecutedOperation";
import {Result} from "../../core/logic/Result";

interface VVEProps {
    vvnId: string; // ID da Vessel Visit Notification
    executedOperations: ExecutedOperation[];
    status: string; // "IN_PROGRESS", "COMPLETED"
    arrivalDate?: Date; // Dados da US 4.1.7 (cabeçalho)
    departureDate?: Date; // Dados da US 4.1.11
}

export class VesselVisitExecution extends AggregateRoot<VVEProps> {

    get vvnId(): string { return this.props.vvnId; }
    get executedOperations(): ExecutedOperation[] { return this.props.executedOperations; }
    get status(): string { return this.props.status; } // US 4.1.10
    get arrivalDate(): Date | undefined { return this.props.arrivalDate; } // US 4.1.10
    get departureDate(): Date | undefined { return this.props.departureDate; } // US 4.1.10

    private constructor(props: VVEProps, id?: UniqueEntityID) {
        super(props, id);
    }

    // Nota: O Create principal seria parte da US 4.1.7 (Membro 3)
    // Mas podes usar este metodo para criar mocks ou testes
    public static create(props: {
        vvnId: string;
        arrivalDate?: Date;
    }, id?: UniqueEntityID): Result<VesselVisitExecution> {

        const vve = new VesselVisitExecution({
            vvnId: props.vvnId,
            executedOperations: [],
            status: "IN_PROGRESS",
            arrivalDate: props.arrivalDate
        }, id);

        return Result.ok<VesselVisitExecution>(vve);
    }

    /**
     * Lógica principal da US 4.1.9
     * Adiciona ou Atualiza uma operação executada na lista
     */
    public registerOperation(operation: ExecutedOperation): void {
        const existingIndex = this.props.executedOperations.findIndex(
            op => op.operationId.equals(operation.operationId)
        );

        if (existingIndex !== -1) {
            // Se já existe, substituímos (update)
            // Num cenário real complexo, poderiamos querer guardar histórico de versões
            this.props.executedOperations[existingIndex] = operation;
        } else {
            // Se não existe, adicionamos
            this.props.executedOperations.push(operation);
        }
    }
}