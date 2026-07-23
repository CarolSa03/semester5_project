import {Inject, Service} from 'typedi';
import {IOperationPlanRepo} from '../../domain/operationPlan/IOperationPlanRepo';
import {OperationPlan} from '../../domain/operationPlan/OperationPlan';
import {OperationPlanMap} from '../../mappers/OperationPlanMap';

@Service()
export default class OperationPlanRepo implements IOperationPlanRepo {
    constructor(
        @Inject('operationPlanSchema') private operationPlanSchema : any
    ) {}

    public async findByDate(date: Date): Promise<OperationPlan[]> {
        const startOfDay = new Date(date);
        startOfDay.setHours(0, 0, 0, 0);
        const endOfDay = new Date(date);
        endOfDay.setHours(23, 59, 59, 999);

        const rawPlans = await this.operationPlanSchema.find({
            date: { $gte: startOfDay, $lte: endOfDay }
        });

        return rawPlans.map((raw: any) => OperationPlanMap.toDomain(raw)).filter((p: any) => p != null);
    }

    public async findByVVN(vvnId: string): Promise<OperationPlan | null> {
        const rawPlan = await this.operationPlanSchema.findOne({ vvnId });
        if (!rawPlan) return null;
        return OperationPlanMap.toDomain(rawPlan);
    }

    public async findByDateRange(startDate: Date, endDate: Date): Promise<OperationPlan[]> {
        const rawPlans = await this.operationPlanSchema.find({
            date: { $gte: startDate, $lte: endDate }
        });

        return rawPlans.map((raw: any) => OperationPlanMap.toDomain(raw)).filter((p: any) => p != null);
    }

    public async findByVVNAndDateRange(vvnId: string, startDate?: Date, endDate?: Date): Promise<OperationPlan[]> {
        const query: any = { vvnId };

        if (startDate || endDate) {
            query.date = {};
            if (startDate) query.date.$gte = startDate;
            if (endDate) query.date.$lte = endDate;
        }

        const rawPlans = await this.operationPlanSchema.find(query);
        return rawPlans.map((raw: any) => OperationPlanMap.toDomain(raw)).filter((p: any) => p != null);
    }

    public async save(plan: OperationPlan): Promise<void> {
        const query = { domainId: plan.id.toString() }; // Alinhado com o Schema
        const rawPlan = OperationPlanMap.toPersistence(plan);
        await this.operationPlanSchema.findOneAndUpdate(query, rawPlan, { upsert: true });
    }

    public async findAll(): Promise<OperationPlan[]> {
        const rawPlans = await this.operationPlanSchema.find({});
        return rawPlans.map((raw: any) => OperationPlanMap.toDomain(raw)).filter((p: any) => p != null);
    }

    public async findById(id: string): Promise<OperationPlan | null> {
        const rawPlan = await this.operationPlanSchema.findOne({ domainId: id });
        const domain = rawPlan ? OperationPlanMap.toDomain(rawPlan) : null;
        return domain && (domain as any).id ? domain : null;
    }

    public async update(plan: OperationPlan): Promise<void> {
        const query = { domainId: plan.id.toString() };
        const rawPlan = OperationPlanMap.toPersistence(plan);

        try {
            await this.operationPlanSchema.findOneAndUpdate(query, rawPlan, {
                new: true,
                upsert: true
            });
        } catch (err) {
            throw err;
        }
    }

    // US 4.1.5 - Encontrar VVNs que n�o t�m Operation Plans
    public async findVVNsWithoutPlan(vvnIds: string[]): Promise<string[]> {
        // Encontrar todos os planos que t�m estes VVN IDs
        const existingPlans = await this.operationPlanSchema.find({ 
            vvnId: { $in: vvnIds } 
        }).select('vvnId');
        
        const existingVvnIds = existingPlans.map((p: any) => p.vvnId);
        
        // Retornar VVNs que n�o t�m planos
        return vvnIds.filter(id => !existingVvnIds.includes(id));
    }

    // US 4.1.5 - Apagar todos os planos de uma data espec�fica
    public async deleteByDate(date: Date): Promise<number> {
        const startOfDay = new Date(date);
        startOfDay.setHours(0, 0, 0, 0);
        const endOfDay = new Date(date);
        endOfDay.setHours(23, 59, 59, 999);

        const result = await this.operationPlanSchema.deleteMany({
            date: { $gte: startOfDay, $lte: endOfDay }
        });

        return result.deletedCount || 0;
    }
}
