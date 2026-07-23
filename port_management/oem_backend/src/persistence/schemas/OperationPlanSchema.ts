import { IOperationPlanPersistence } from '../../dataschema/IOperationPlanPersistence'; // Ajuste o path se necessário
import mongoose from 'mongoose';

const OperationPlanSchema = new mongoose.Schema(
    {
        domainId: { type: String, unique: true },
        vvnId: { type: String, required: true },
        date: { type: Date, required: true },
        algorithm: { type: String, required: true },

        // Métricas
        totalDelay: { type: Number, default: 0 },
        computationTime: { type: Number, default: 0 },

        // Status e Auditoria
        status: { type: String, default: 'ACTIVE' },
        lastModified: { type: Date },
        lastModifiedBy: { type: String },
        modificationReason: { type: String },

        // --- CORREÇÃO CRÍTICA: Definição Explícita das Operações ---
        operations: [
            {
                _id: { type: String, required: true },
                operationId: { type: String, required: true },
                type: { type: String, required: true },    // UNLOADING, etc.
                status: { type: String, required: true },  // PLANNED, COMPLETED
                resourceId: { type: String, required: true },
                startTime: { type: Date, required: true }, // Data completa
                endTime: { type: Date, required: true }    // Data completa
            }
        ]
    },
    {
        timestamps: true
    }
);

export default mongoose.model<IOperationPlanPersistence & mongoose.Document>('OperationPlan', OperationPlanSchema);