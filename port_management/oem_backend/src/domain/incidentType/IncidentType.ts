import {AggregateRoot} from "../../core/domain/AggregateRoot";
import {UniqueEntityID} from "../../core/domain/UniqueEntityID";
import {Result} from "../../core/logic/Result";
import {IncidentTypeSeverity} from "./enums/IncidentTypeSeverity.enum";

interface IncidentTypeProps {
    code: string;           // ex: "T-INC001" (Negócio)
    name: string;           // ex: "Fog"
    description: string;
    severity: IncidentTypeSeverity;
    parentId?: string;      // ID do tipo pai (Opcional, pois o topo da hierarquia não tem pai)
    active: boolean;
}

export class IncidentType extends AggregateRoot<IncidentTypeProps> {

    get code(): string { return this.props.code; }
    get name(): string { return this.props.name; }
    get description(): string { return this.props.description; }
    get severity(): IncidentTypeSeverity { return this.props.severity; }
    get parentId(): string | undefined { return this.props.parentId; }
    get isActive(): boolean { return this.props.active; }

    private constructor(props: IncidentTypeProps, id?: UniqueEntityID) {
        super(props, id);
    }

    public static create(props: {
        code: string;
        name: string;
        description: string;
        severity: IncidentTypeSeverity;
        parentId?: string;
    }, id?: UniqueEntityID): Result<IncidentType> {

        if (!props.code || !props.name || !props.severity || !props.description) {
            return Result.fail<IncidentType>("Code, Name, Description and Severity are required.");
        }

        const incidentType = new IncidentType({
            ...props,
            active: true
        }, id);

        return Result.ok<IncidentType>(incidentType);
    }

    public changeName(name: string): void {
        this.props.name = name;
    }
    public changeDescription(description: string): void {
        this.props.description = description;
    }
    public changeSeverity(severity: IncidentTypeSeverity): void {
        this.props.severity = severity;
    }
    public changeParentId(parentId: string | undefined): void {
        this.props.parentId = parentId;
    }
}