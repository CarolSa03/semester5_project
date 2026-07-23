import 'reflect-metadata';
import request = require('supertest');
import express = require('express');
import { expect } from 'chai';
import { Container } from 'typedi';
import { isCelebrateError } from 'celebrate';
import VesselVisitExecutionRoute from '../../src/api/routes/vesselVisitExecutionRoute';

// Interfaces e DTOs (se necessário importar tipos)
import { VesselVisitExecution } from '../../src/domain/vesselVE/VesselVisitExecution';
import { VesselVisitExecutionMap } from '../../src/mappers/VesselVisitExecutionMap';

describe('E2E API Test: Vessel Visit Execution (System Level)', () => {
    let app: express.Application;
    let memoryDb: any[] = []; // Simula a BD em memória

    beforeEach(() => {
        Container.reset();
        memoryDb = []; // Limpar BD

        // =========================================================
        // 1. Mocks de INFRAESTRUTURA (Exterior do Sistema)
        // =========================================================

        const vveRepoMock = {
            save: async (vve: VesselVisitExecution) => {
                const persistence = VesselVisitExecutionMap.toPersistence(vve);
                const index = memoryDb.findIndex(item => item.vvnId === persistence.vvnId);
                if (index >= 0) {
                    memoryDb[index] = persistence;
                } else {
                    memoryDb.push(persistence);
                }
            },
            findByVVN: async (vvnId: string) => {
                const found = memoryDb.find(item => item.vvnId === vvnId);
                if (!found) return null;
                return VesselVisitExecutionMap.toDomain(found);
            },
            // Mock básico para o search (se for chamado)
            findByFilters: async () => []
        };

        // Injeção do Mock de Repositório
        Container.set('VesselVisitExecutionRepo', vveRepoMock);

        // Mocks adicionais para dependências do Controller (Search Service, etc.)
        // Como estamos a testar o CREATE (POST), o SearchService pode ser mockado de forma simples se o controller o exigir no construtor
        const portAdapterMock = { fetchApprovedVVNs: async () => [] };
        const operationPlanRepoMock = { findByVVN: async () => null };

        Container.set('PortAdapter', portAdapterMock);
        Container.set('OperationPlanRepo', operationPlanRepoMock);

        // NOTA: Para o SearchVVEService, podemos deixar o Container resolver ou mockar se falhar
        // Se o controller injecta SearchVVEService, e este precisa de VVE Repo (já mockado), deve funcionar.

        // =========================================================
        // 2. SERVIÇOS REAIS
        // =========================================================
        // Não fazemos mock do CreateVesselVisitExecutionService.
        // O Container vai instanciar o serviço real definido no código.

        // 3. Setup Express
        app = express();
        app.use(express.json());
        VesselVisitExecutionRoute(app);

        // Middleware de erro
        app.use((err: any, req: any, res: any, next: any) => {
            if (isCelebrateError(err)) return res.status(400).json({ error: "Validation Failed" });
            res.status(500).json({ error: err.message });
        });
    });

    it('POST /vessel-visit-executions deve criar com sucesso (Fluxo Real)', async () => {
        const res = await request(app)
            .post('/vessel-visit-executions')
            .send({
                vvnId: 'VVN-REAL-001',
                arrivalDate: '2026-01-03T12:00:00Z'
            });

        expect(res.status).to.equal(201);
        expect(res.body).to.have.property('vvnId', 'VVN-REAL-001');

        // Verifica se persistiu na "BD"
        expect(memoryDb).to.have.lengthOf(1);
        expect(memoryDb[0].vvnId).to.equal('VVN-REAL-001');
    });

    it('POST /vessel-visit-executions deve falhar se duplicado (Regra de Negócio Real)', async () => {
        // Pré-popular a BD
        memoryDb.push({
            vvnId: 'VVN-DUP',
            status: 'IN_PROGRESS',
            executedOperations: []
        });

        const res = await request(app)
            .post('/vessel-visit-executions')
            .send({
                vvnId: 'VVN-DUP',
                arrivalDate: '2026-01-03T12:00:00Z'
            });

        expect(res.status).to.equal(400);
        expect(res.body.error).to.include('already exists');
    });

    it('POST /vessel-visit-executions deve falhar com data inválida (Regra de Domínio Real)', async () => {
        const res = await request(app)
            .post('/vessel-visit-executions')
            .send({
                vvnId: 'VVN-NEW',
                arrivalDate: 'invalid-date'
            });

        expect(res.status).to.equal(400);
        expect(res.body.error).to.include('Invalid date');
    });
});