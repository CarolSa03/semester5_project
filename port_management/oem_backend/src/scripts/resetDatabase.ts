import mongoose from 'mongoose';
import 'dotenv/config';

const cleanDB = async () => {
    try {
        console.log("conectando à BD...");
        await mongoose.connect(process.env.MONGO_URI || "mongodb://127.0.0.1:27017/oemdb");
        console.log('✅ Connected to MongoDB');

        console.log("🧹 A limpar dados antigos...");

        // 1. Limpar Execuções de Visitas (Onde está o erro atual)
        await mongoose.connection.collection('vesselvisitexecutions').deleteMany({});
        console.log("✅ VesselVisitExecutions limpos.");

        // 2. Limpar Planos de Operação (Para garantir)
        await mongoose.connection.collection('operationplans').deleteMany({});
        console.log("✅ OperationPlans limpos.");

        console.log("🏁 Concluído!");
        process.exit(0);
    } catch (err) {
        console.error("Erro:", err);
        process.exit(1);
    }
};

cleanDB();