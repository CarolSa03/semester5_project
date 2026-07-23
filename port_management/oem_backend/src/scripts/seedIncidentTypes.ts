import 'dotenv/config';
import mongoose from 'mongoose';
import IncidentTypeSchema from '../persistence/schemas/IncidentTypeSchema';
import {IncidentType} from '../domain/incidentType/IncidentType';
import {IncidentTypeMap} from '../mappers/IncidentTypeMap';
import {IncidentTypeSeverity} from '../domain/incidentType/enums/IncidentTypeSeverity.enum';

// Renomeado para mockIT para bater certo com o teu export
const mockIT = [
    // 1. Condições Ambientais (Categoria Pai)
    {
        code: "ENV-001",
        name: "Environmental Conditions",
        description: "Adverse weather or sea conditions affecting operations.",
        severity: IncidentTypeSeverity.MINOR,
        parentId: null
    },
    // Filhos de Ambientais
    {
        code: "ENV-FOG",
        name: "Fog",
        description: "Reduced visibility due to fog.",
        severity: IncidentTypeSeverity.MAJOR,
        parentCode: "ENV-001"
    },
    {
        code: "ENV-WIND",
        name: "Strong Winds",
        description: "Wind speeds exceeding operational safety limits.",
        severity: IncidentTypeSeverity.CRITICAL,
        parentCode: "ENV-001"
    },
    {
        code: "ENV-RAIN",
        name: "Heavy Rain",
        description: "Precipitation intensity affecting equipment traction or visibility.",
        severity: IncidentTypeSeverity.MINOR,
        parentCode: "ENV-001"
    },

    // 2. Falhas Operacionais (Categoria Pai)
    {
        code: "OP-FAIL",
        name: "Operational Failures",
        description: "Equipment breakdowns or system failures.",
        severity: IncidentTypeSeverity.MAJOR,
        parentId: null
    },
    // Filhos de Operacionais
    {
        code: "OP-CRANE",
        name: "Crane Malfunction",
        description: "Mechanical or electrical failure of a quay crane.",
        severity: IncidentTypeSeverity.CRITICAL,
        parentCode: "OP-FAIL"
    },
    {
        code: "OP-POWER",
        name: "Power Outage",
        description: "Loss of electrical power in the terminal or specific zones.",
        severity: IncidentTypeSeverity.CRITICAL,
        parentCode: "OP-FAIL"
    },

    // 3. Eventos de Segurança (Categoria Pai)
    {
        code: "SEC-EVENT",
        name: "Safety/Security Events",
        description: "Security breaches, accidents or safety alerts.",
        severity: IncidentTypeSeverity.CRITICAL,
        parentId: null
    },
    // Filhos de Segurança
    {
        code: "SEC-ALERT",
        name: "Security Alert",
        description: "Unauthorized access or suspicious activity detected.",
        severity: IncidentTypeSeverity.CRITICAL,
        parentCode: "SEC-EVENT"
    }
];

async function seedIncidentTypes() {
    try {
        // 1. Ligar à BD (Verifica se a URI está correta no teu .env ou config)
        const mongoURI = process.env.MONGO_URI || "mongodb://127.0.0.1:27017/lei-project-db";

        // Evita ligar se já estiver ligado (caso seja importado)
        if (mongoose.connection.readyState === 0) {
            await mongoose.connect(mongoURI);
            console.log('✅ Connected to MongoDB');
        }

        // 2. Limpar Tipos Antigos
        await IncidentTypeSchema.deleteMany({});
        console.log('🗑️ Cleared old Incident Types');

        // 3. Inserir Tipos
        const codeToIdMap = new Map<string, string>();

        // A. Primeiro inserimos os PAIS
        const parents = mockIT.filter(i => !i.parentCode);

        for (const p of parents) {
            const domainOrError = IncidentType.create({
                code: p.code,
                name: p.name,
                description: p.description,
                severity: p.severity,
                parentId: undefined
            });

            if (domainOrError.isFailure) {
                console.error(`❌ Failed to create ${p.code}:`, domainOrError.error);
                continue;
            }

            const incidentType = domainOrError.getValue();
            const raw = IncidentTypeMap.toPersistence(incidentType);

            await IncidentTypeSchema.create(raw);

            codeToIdMap.set(p.code, incidentType.id.toString());
            console.log(`✅ Created Parent: ${p.name}`);
        }

        // B. Depois inserimos os FILHOS
        const children = mockIT.filter(i => i.parentCode);

        for (const c of children) {
            const parentId = codeToIdMap.get(c.parentCode as string);

            if (!parentId) {
                console.warn(`⚠️ Parent ${c.parentCode} not found for ${c.code}. Skipping.`);
                continue;
            }

            const domainOrError = IncidentType.create({
                code: c.code,
                name: c.name,
                description: c.description,
                severity: c.severity,
                parentId: parentId
            });

            if (domainOrError.isFailure) {
                console.error(`❌ Failed to create ${c.code}:`, domainOrError.error);
                continue;
            }

            const incidentType = domainOrError.getValue();
            const raw = IncidentTypeMap.toPersistence(incidentType);
            await IncidentTypeSchema.create(raw);
            console.log(`   ↳ Created Child: ${c.name}`);
        }

        console.log('\n✅ Incident Types Seeded Successfully!');

        // Só fecha a conexão se o script foi corrido diretamente
        if (require.main === module) {
            await mongoose.disconnect();
            process.exit(0);
        }

    } catch (error) {
        console.error('🔥 Error seeding incident types:', error);
        process.exit(1);
    }
}

// Executar apenas se for chamado diretamente via CLI
if (require.main === module) {
    seedIncidentTypes();
}

export { seedIncidentTypes, mockIT };