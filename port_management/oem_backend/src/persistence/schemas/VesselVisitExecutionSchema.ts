import mongoose from 'mongoose';

const VesselVisitExecutionSchema = new mongoose.Schema({
    // Identificador único da execução (pode ser o mesmo que o VVN ou gerado)
    vvnId: { type: String, unique: true, required: true },

    status: { type: String, required: true }, // IN_PROGRESS, COMPLETED

    completed: { type: Boolean, default: false },

    arrivalDate: { type: Date },    // US 4.1.7 (Registo de Chegada)
    departureDate: { type: Date },  // US 4.1.11 (Registo de Partida)

    // Lista de operações já realizadas
    executedOperations: [{
        operationId: { type: String, required: true },
        realStartTime: { type: Date, required: true },
        realEndTime: { type: Date, required: true },
        realResource: { type: String, required: true },
        status: { type: String, required: true }, // COMPLETED / DELAYED
        completedBy: { type: String, required: true },
        registeredAt: { type: Date, default: Date.now }
    }]
}, { timestamps: true });

export default mongoose.model('VesselVisitExecution', VesselVisitExecutionSchema);
