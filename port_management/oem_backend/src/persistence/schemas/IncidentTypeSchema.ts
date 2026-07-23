import mongoose from 'mongoose';
import {IncidentTypeSeverity} from '../../domain/incidentType/enums/IncidentTypeSeverity.enum';

const IncidentTypeSchema = new mongoose.Schema({
    domainId: { type: String, unique: true }, // ID interno do Agregado

    code: { type: String, unique: true, required: true, index: true },
    name: { type: String, required: true },
    description: { type: String },
    severity: {
        type: String,
        enum: Object.values(IncidentTypeSeverity),
        required: true
    },
    // Auto-referência para criar a hierarquia
    parentId: { type: String, required: false, default: null },

    active: { type: Boolean, default: true }
}, { timestamps: true });

export default mongoose.model('IncidentType', IncidentTypeSchema);