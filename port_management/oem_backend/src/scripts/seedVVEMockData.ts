import 'dotenv/config';
import mongoose from 'mongoose';
import VesselVisitExecutionSchema from '../persistence/schemas/VesselVisitExecutionSchema';
import {OperationId} from "../domain/operationPlan/value-objects/OperationId.vo";

/**
 * US 4.1.10 - Script para popular dados MOCK de VVEs para testes
 * Execute: ts-node src/scripts/seedVVEMockData.ts
 */

const mockVVEs = [
    {
        vvnId: "VVN-2026-001",
        status: "COMPLETED",
        arrivalDate: new Date("2026-01-15T06:30:00Z"), // 30 min de atraso
        departureDate: new Date("2026-01-15T14:45:00Z"), // 45 min de atraso
        executedOperations: [
            {
                operationId: OperationId.generate(),
                realStartTime: new Date("2026-01-15T07:00:00Z"),
                realEndTime: new Date("2026-01-15T09:30:00Z"),
                realResource: "CRANE-C1",
                status: "COMPLETED",
                completedBy: "operator-001",
                registeredAt: new Date("2026-01-15T09:30:00Z")
            },
            {
                operationId: OperationId.generate(),
                realStartTime: new Date("2026-01-15T10:00:00Z"),
                realEndTime: new Date("2026-01-15T13:00:00Z"),
                realResource: "CRANE-C2",
                status: "COMPLETED",
                completedBy: "operator-001",
                registeredAt: new Date("2026-01-15T13:00:00Z")
            }
        ]
    },
    {
        vvnId: "VVN-2026-002",
        status: "COMPLETED",
        arrivalDate: new Date("2026-01-15T10:15:00Z"), // 15 min de atraso
        departureDate: new Date("2026-01-15T19:00:00Z"), // 1h de atraso
        executedOperations: [
            {
                operationId: OperationId.generate(),
                realStartTime: new Date("2026-01-15T11:00:00Z"),
                realEndTime: new Date("2026-01-15T14:30:00Z"),
                realResource: "CRANE-C3",
                status: "DELAYED",
                completedBy: "operator-002",
                registeredAt: new Date("2026-01-15T14:30:00Z")
            },
            {
                operationId: OperationId.generate(),
                realStartTime: new Date("2026-01-15T15:00:00Z"),
                realEndTime: new Date("2026-01-15T18:00:00Z"),
                realResource: "CRANE-C1",
                status: "COMPLETED",
                completedBy: "operator-002",
                registeredAt: new Date("2026-01-15T18:00:00Z")
            }
        ]
    },
    {
        vvnId: "VVN-2026-003",
        status: "COMPLETED",
        arrivalDate: new Date("2026-01-15T14:00:00Z"), // No time
        departureDate: new Date("2026-01-15T22:30:00Z"), // 30 min de atraso
        executedOperations: [
            {
                operationId: "OP-005",
                realStartTime: new Date("2026-01-15T15:00:00Z"),
                realEndTime: new Date("2026-01-15T18:00:00Z"),
                realResource: "CRANE-C2",
                status: "COMPLETED",
                completedBy: "operator-003",
                registeredAt: new Date("2026-01-15T18:00:00Z")
            },
            {
                operationId: "OP-006",
                realStartTime: new Date("2026-01-15T18:30:00Z"),
                realEndTime: new Date("2026-01-15T21:30:00Z"),
                realResource: "CRANE-C4",
                status: "COMPLETED",
                completedBy: "operator-003",
                registeredAt: new Date("2026-01-15T21:30:00Z")
            }
        ]
    },
    {
        vvnId: "VVN-2026-004",
        status: "IN_PROGRESS",
        arrivalDate: new Date("2026-01-16T08:00:00Z"), // On time
        departureDate: null, // Still in progress
        executedOperations: [
            {
                operationId: "OP-007",
                realStartTime: new Date("2026-01-16T09:00:00Z"),
                realEndTime: new Date("2026-01-16T11:30:00Z"),
                realResource: "CRANE-C1",
                status: "COMPLETED",
                completedBy: "operator-004",
                registeredAt: new Date("2026-01-16T11:30:00Z")
            }
        ]
    },
    {
        vvnId: "VVN-2026-005",
        status: "IN_PROGRESS",
        arrivalDate: new Date("2026-01-16T12:45:00Z"), // 45 min de atraso
        departureDate: null,
        executedOperations: []
    },
    {
        vvnId: "VVN-2026-006",
        status: "COMPLETED",
        arrivalDate: new Date("2026-01-17T09:00:00Z"), // On time
        departureDate: new Date("2026-01-17T16:30:00Z"), // 30 min antes (early!)
        executedOperations: [
            {
                operationId: "OP-008",
                realStartTime: new Date("2026-01-17T10:00:00Z"),
                realEndTime: new Date("2026-01-17T12:00:00Z"),
                realResource: "CRANE-C3",
                status: "COMPLETED",
                completedBy: "operator-005",
                registeredAt: new Date("2026-01-17T12:00:00Z")
            },
            {
                operationId: "OP-009",
                realStartTime: new Date("2026-01-17T13:00:00Z"),
                realEndTime: new Date("2026-01-17T15:30:00Z"),
                realResource: "CRANE-C2",
                status: "COMPLETED",
                completedBy: "operator-005",
                registeredAt: new Date("2026-01-17T15:30:00Z")
            }
        ]
    }
];

async function seedVVEData() {
    try {
        // Conectar à base de dados
        await mongoose.connect(process.env.MONGO_URI || "mongodb://127.0.0.1:27017/oemdb");
        console.log('✅ Connected to MongoDB');

        // Limpar dados antigos (opcional)
        await VesselVisitExecutionSchema.deleteMany({});
        console.log('🗑️ Cleared old VVE data');

        // Inserir dados mock
        await VesselVisitExecutionSchema.insertMany(mockVVEs);
        console.log(`✅ Inserted ${mockVVEs.length} mock VVEs`);

        console.log('\n📊 Mock VVE Summary:');
        console.log(`  - COMPLETED: ${mockVVEs.filter(v => v.status === 'COMPLETED').length}`);
        console.log(`  - IN_PROGRESS: ${mockVVEs.filter(v => v.status === 'IN_PROGRESS').length}`);
        console.log(`  - Date range: 2026-01-15 to 2026-01-17`);

        await mongoose.disconnect();
        console.log('\n✅ Done! Mock data seeded successfully');

    } catch (error) {
        console.error('❌ Error seeding mock data:', error);
        process.exit(1);
    }
}

// Executar apenas se for chamado diretamente
if (require.main === module) {
    seedVVEData();
}

export { seedVVEData, mockVVEs };
