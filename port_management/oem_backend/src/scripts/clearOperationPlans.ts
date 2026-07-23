import 'reflect-metadata'; // Importante para o typedi
import 'dotenv/config';
import { Container } from 'typedi';
import mongoose from 'mongoose';
import { OperationPlan } from '../domain/operationPlan/OperationPlan';

async function clearDB() {
    try {
        // 1. Ligar à BD
        await mongoose.connect(process.env.MONGO_URI || 'mongodb://127.0.0.1:27017/lei-project-db');
        console.log("✅ Ligado à BD");

        // 2. Aceder à coleção e apagar tudo
        // Nota: O nome da coleção no Mongoose é geralmente o plural minúsculo do modelo
        const collection = mongoose.connection.collection('operationplans');

        await collection.deleteMany({});
        console.log("🗑️ Coleção 'operationplans' limpa com sucesso!");

        // 3. Fechar ligação
        await mongoose.disconnect();
        process.exit(0);
    } catch (e) {
        console.error(e);
        process.exit(1);
    }
}

clearDB();