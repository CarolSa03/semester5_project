import mongoose from 'mongoose';

const ComplementaryTaskCategorySchema = new mongoose.Schema({
    domainId: { type: String, unique: true, required: true }, // ID do AggregateRoot

    code: { type: String, unique: true, required: true, index: true },
    name: { type: String, required: true },
    description: { type: String, required: true },
    duration: { type: Number, required: false },

    active: { type: Boolean, default: true }
}, { timestamps: true });

export default mongoose.model('ComplementaryTaskCategory', ComplementaryTaskCategorySchema);
