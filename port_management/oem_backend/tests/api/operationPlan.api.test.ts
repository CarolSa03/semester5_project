import 'reflect-metadata';
import request = require('supertest');
import express = require('express');
import { expect } from 'chai';
import { Container } from 'typedi';
import { isCelebrateError } from 'celebrate';
import { Result } from '../../src/core/logic/Result';
import operationPlanRoute from '../../src/api/routes/operationPlanRoute';

// Interfaces e DTOs necessários para o Mock
import { IOperationPlanDTO } from '../../src/dto/IOperationPlanDTO';

describe('E2E API Test: Operation Plans (System Level)', () => {
    let app: express.Application;
    let memoryDb: any[] = []; // Simula a BD em memória

    beforeEach(() => {
        Container.reset();
        memoryDb = []; // Limpa a BD antes de cada teste

        // =========================================================
        // 1. Mocks de INFRAESTRUTURA (Exterior do Sistema)
        // =========================================================

        // Mock do Repositório: Comporta-se como uma BD em memória
        const planRepoMock = {
            save: async (plan: any) => {
                // Simula o comportamento do Mongoose (upsert)
                const index = memoryDb.findIndex(p => p.id.toString() === plan.id.toString());
                if (index >= 0) {
                    memoryDb[index] = plan;
                } else {
                    memoryDb.push(plan);
                }
            },
            findById: async (id: string) => {
                return memoryDb.find(p => p.id.toString() === id) || null;
            },
            findAll: async () => {
                return [...memoryDb];
            },
            findByDateRange: async (start: Date, end: Date) => {
                // Filtro simples para teste
                return memoryDb;
            },
            findVVNsWithoutPlan: async (vvnIds: string[]) => {
                // Retorna IDs que não estão na BD
                const existing = memoryDb.map(p => p.vvnId);
                return vvnIds.filter(id => !existing.includes(id));
            },
            deleteByDate: async (date: Date) => {
                const initialLen = memoryDb.length;
                // Remove tudo (simplificação para teste)
                memoryDb = [];
                return initialLen;
            },
            update: async (plan: any) => {
                await planRepoMock.save(plan);
            }
        };

        // Mock do Planeamento Externo: Retorna dados válidos para o Domínio
        const planningAdapterMock = {
            fetchSchedule: async (date: Date, algo: string, vvnId: string) => {
                const mockPlanDTO: IOperationPlanDTO = {
                    id: "plan-mock-001",
                    vvnId: vvnId || "VVN-123",
                    date: date.toISOString(),
                    metrics: {
                        algorithm: algo || "genetic",
                        totalDelay: 0,
                        computationTime: 0.1
                    },
                    schedule: [
                        {
                            operationId: "OP-001",
                            vesselId: vvnId || "VVN-123",
                            resourceId: "CRANE-01",
                            startTime: 600, // 10:00
                            endTime: 720,   // 12:00
                            durationMinutes: 120,
                            assignedCranes: ["CRANE-01"],
                            delay: 0,
                            status: "PLANNED"
                        }
                    ]
                };
                return Result.ok(mockPlanDTO);
            }
        };

        // Mock do Port Module
        const portAdapterMock = {
            fetchApprovedVVNs: async () => [
                { vvnId: "VVN-123", vesselName: "Test Vessel", expectedArrival: new Date() }
            ]
        };

        // Injeção dos Mocks de Infraestrutura
        Container.set('OperationPlanRepo', planRepoMock);
        Container.set('PlanningAdapter', planningAdapterMock);
        Container.set('PortAdapter', portAdapterMock);

        // =========================================================
        // 2. SERVIÇOS REAIS (Não fazemos Mock aqui!)
        // =========================================================
        // Ao não fazer Container.set(Service, mock), o Typedi vai instanciar
        // os serviços reais definidos nos ficheiros .ts, usando os repositórios mockados acima.

        // 3. Setup do Express
        app = express();
        app.use(express.json());
        operationPlanRoute(app);

        // Middleware de erro
        app.use((err: any, req: any, res: any, next: any) => {
            if (isCelebrateError(err)) return res.status(400).json({ error: "Validation Failed" });
            res.status(500).json({ error: err.message });
        });
    });

    // =========================================================
    // TESTES
    // =========================================================

    it('POST /generate deve executar o fluxo completo e criar plano', async () => {
        // Data futura para não falhar na validação de domínio
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 5);
        const dateStr = futureDate.toISOString().split('T')[0];

        const res = await request(app)
            .post('/operation-plans/generate')
            .send({
                vvnId: "VVN-123",
                date: dateStr,
                algorithm: "genetic"
            });

        expect(res.status).to.equal(201);
        expect(res.body).to.have.property('vvnId', 'VVN-123');
        // Verifica se guardou no "banco de dados" em memória
        expect(memoryDb).to.have.lengthOf(1);
        expect(memoryDb[0].vvnId).to.equal('VVN-123');
    });

    it('GET /operation-plans deve retornar a lista vinda do repositório', async () => {
        // Pré-popular a BD em memória
        const mockPlan = {
            id: "plan-existing",
            vvnId: "VVN-999",
            // Mock parcial suficiente para o Mapper.toDTO
            date: new Date(),
            algorithm: "astar",
            totalDelay: 0,
            computationTime: 0,
            operations: []
        };
        memoryDb.push(mockPlan);

        const res = await request(app).get('/operation-plans');

        expect(res.status).to.equal(200);
        expect(res.body).to.be.an('array');
        expect(res.body).to.have.lengthOf(1);
        expect(res.body[0]).to.have.property('vvnId', 'VVN-999');
    });

    it('POST /generate com dados inválidos deve ser barrado pelo Controller/Middleware', async () => {
        const res = await request(app)
            .post('/operation-plans/generate')
            .send({
                algorithm: "genetic"
                // Faltam campos obrigatórios
            });

        expect(res.status).to.equal(400); // Bad Request (Validation)
    });
});