import {ValidationError} from "../../../core/logic/ValidateError";
import {UniqueEntityID} from "../../../core/domain/UniqueEntityID";

export class OperationId extends UniqueEntityID{
    private constructor(id?: string) {
        super(id);
    }

    static create(value?: string): OperationId {
        if (value) {
            this.validate(value);
            return new OperationId(value);
        }
        return this.generate();
    }

    static generate(): OperationId {
        const timestamp = Date.now();
        const randomStr = Math.random().toString().substring(2, 6).toUpperCase();
        return new OperationId(`OPR-${timestamp}-${randomStr}`);
    }

    static validate(value: string): void {
        if (!value || value.trim().length === 0) {
            throw new ValidationError('Operation ID cannot be empty', 'operationId');
        }

        const pattern = /^OPR-\d+-[A-Z0-9]+$/;
        if (!pattern.test(value)) {
            throw new ValidationError('Invalid Operation ID format.', 'operationId');
        }
    }

    getValue(): string {
        return super.toString();
    }

    toString(): string {
        return super.toString();
    }
}