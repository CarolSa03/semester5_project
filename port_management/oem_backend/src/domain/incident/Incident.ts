import {AggregateRoot} from "../../core/domain/AggregateRoot";
import {UniqueEntityID} from "../../core/domain/UniqueEntityID";
import {Result} from "../../core/logic/Result";
import {Guard} from "../../core/logic/Guard";
import {IncidentTypeSeverity} from "../incidentType/enums/IncidentTypeSeverity.enum";

interface IncidentProps {
    incidentTypeId: string;
    description: string;
    severity: IncidentTypeSeverity;
    startTime: Date;
    endTime?: Date;
    affectedVvnIds: string[];
    createdBy: string;
}

export class Incident extends AggregateRoot<IncidentProps> {
    get id(): UniqueEntityID { return this._id; }
    get incidentTypeId(): string { return this.props.incidentTypeId; }
    get description(): string { return this.props.description; }
    get severity(): IncidentTypeSeverity { return this.props.severity; }
    get startTime(): Date { return this.props.startTime; }
    get endTime(): Date | undefined { return this.props.endTime; }
    get affectedVvnIds(): string[] { return this.props.affectedVvnIds; }
    get createdBy(): string { return this.props.createdBy; }

    get isActive(): boolean {
        return !this.props.endTime;
    }

    get durationMinutes(): number | undefined {
        if (!this.props.endTime) {
            return undefined;
        }
        const diffMs = this.props.endTime.getTime() - this.props.startTime.getTime();
        // Converte milissegundos para minutos e arredonda
        return Math.floor(diffMs / 60000);
    }

    private constructor(props: IncidentProps, id?: UniqueEntityID) {
        super(props, id);
    }

    public static create(props: {
        incidentTypeId: string;
        description: string;
        severity: IncidentTypeSeverity;
        startTime?: Date;
        createdBy: string;
        affectedVvnIds?: string[];
    }, id?: UniqueEntityID): Result<Incident> {

        const guardResult = Guard.combine([
            Guard.againstNullOrUndefined(props.incidentTypeId, 'incidentTypeId'),
            Guard.againstNullOrUndefined(props.description, 'description'),
            Guard.againstNullOrUndefined(props.createdBy, 'createdBy')
        ]);

        if (!guardResult.succeeded) {
            return Result.fail<Incident>(guardResult.message);
        }

        const incident = new Incident({
            ...props,
            startTime: props.startTime || new Date(),
            affectedVvnIds: props.affectedVvnIds || []
        }, id);

        return Result.ok<Incident>(incident);
    }

    // --- MÉTODOS DE NEGÓCIO (DDD) ---

    public resolve(endTime?: Date): void {
        if (!this.isActive) return; // Já está resolvido
        this.props.endTime = endTime || new Date();
    }

    public changeDescription(newDescription: string): void {
        if (!newDescription || newDescription.trim().length === 0) return;
        this.props.description = newDescription;
    }

    public changeSeverity(newSeverity: IncidentTypeSeverity): void {
        this.props.severity = newSeverity;
    }

    public updateAffectedVVEs(newVvnIds: string[]): void {
        this.props.affectedVvnIds = newVvnIds;
    }
}