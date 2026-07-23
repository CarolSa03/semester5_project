import mongoose from 'mongoose';
import {IIncidentPersistence} from '../../dataschema/IIncidentPersistence';

const IncidentSchema = new mongoose.Schema(
    {
        domainId: { type: String, unique: true }, // Mantém o domainId
        incidentTypeId: { type: String, required: true },
        description: { type: String, required: true },
        severity: { type: String, required: true },
        startTime: { type: Date, required: true },
        endTime: { type: Date },
        affectedVvnIds: [{ type: String }],
        createdBy: { type: String, required: true }
    },
    {
        timestamps: true
    }
);

export default mongoose.model<IIncidentPersistence & mongoose.Document>('Incident', IncidentSchema);