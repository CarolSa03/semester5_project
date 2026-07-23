import {ValidationError} from "../../../core/logic/ValidateError";
import {UniqueEntityID} from "../../../core/domain/UniqueEntityID";

export class PlanId extends UniqueEntityID {
    private constructor(id?: string) {
        super(id);
    }

    static create(value?: string): PlanId {
        if (value) {
            this.validate(value);
            return new PlanId(value);
        }
        return this.generate();
    }

    static generate(): PlanId {
        const timestamp = Date.now();
        const randomStr = Math.random().toString().substring(2, 8).toUpperCase();
        return new PlanId(`OP-${timestamp}-${randomStr}`);
    }

    static validate(value: string): void {
        if (!value || value.trim().length === 0) {
            throw new ValidationError('Plan ID cannot be empty', 'planId');
        }

        const pattern = /^OP-\d+-[A-Z0-9]+$/;
        if (!pattern.test(value)) {
            throw new ValidationError('Invalid Plan ID format.', 'planId');
        }
    }

    getValue(): string {
        return super.toString();
    }

    equals(id?: any): boolean {
        if (!id || !(id instanceof PlanId)) return false;
        return this.toString() === id.toString();
    }

    toString(): string {
        return super.toString();
    }
}