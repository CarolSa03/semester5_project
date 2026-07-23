import { AggregateRoot } from "../../core/domain/AggregateRoot";
import { UniqueEntityID } from "../../core/domain/UniqueEntityID";
import { Result } from "../../core/logic/Result";

interface ComplementaryTaskCategoryProps {
    code: string;        // CTC001
    name: string;        // Security Check
    description: string;
    duration?: number;  // minutos (opcional)
}

export class ComplementaryTaskCategory extends AggregateRoot<ComplementaryTaskCategoryProps> {

    get id(): UniqueEntityID {
        return this._id;
    }

    get code(): string {
        return this.props.code;
    }

    get name(): string {
        return this.props.name;
    }

    get description(): string {
        return this.props.description;
    }

    get duration(): number | undefined {
        return this.props.duration;
    }

    private constructor(props: ComplementaryTaskCategoryProps, id?: UniqueEntityID) {
        super(props, id);
    }

    public static create(
        props: ComplementaryTaskCategoryProps,
        id?: UniqueEntityID
    ): Result<ComplementaryTaskCategory> {

        if (!props.code || !props.name || !props.description) {
            return Result.fail("Code, name and description are required");
        }

        if (props.duration !== undefined && props.duration <= 0) {
            return Result.fail("Duration must be greater than zero");
        }

        const category = new ComplementaryTaskCategory(props, id);
        return Result.ok(category);
    }
}
