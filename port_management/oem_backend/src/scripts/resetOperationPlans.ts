import 'reflect-metadata';
import mongoose from 'mongoose';
import 'dotenv/config';

async function reset() {
    console.log("🧹 A limpar Operation Plans...");
    try {
        await mongoose.connect(process.env.MONGO_URI || 'mongodb://127.0.0.1:27017/lei-project-db');
        const result = await mongoose.connection.collection('operationplans').deleteMany({});
        console.log(`✅ Apagados ${result.deletedCount} planos.`);
        await mongoose.disconnect();
    } catch (e) {
        console.error("❌ Erro:", e);
    }
}
reset();