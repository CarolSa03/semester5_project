import {OperationPlan} from "./OperationPlan";

export interface IOperationPlanRepo {
    save(plan: OperationPlan): Promise<void>;
    findByDate(date: Date): Promise<OperationPlan[]>;
    findByVVN(vvnId: string): Promise<OperationPlan | null>;
    findByDateRange(startDate: Date, endDate: Date): Promise<OperationPlan[]>;
    findByVVNAndDateRange(vvnId: string, startDate?: Date, endDate?: Date): Promise<OperationPlan[]>;
    findAll(): Promise<OperationPlan[]>;
    findById(id: string): Promise<OperationPlan | null>;
    update(plan: OperationPlan): Promise<void>;
    // US 4.1.5 - Métodos para identificar VVNs sem planos
    findVVNsWithoutPlan(vvnIds: string[]): Promise<string[]>;
    deleteByDate(date: Date): Promise<number>;
    // Outros métodos necessários
}