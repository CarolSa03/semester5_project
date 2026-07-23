import 'reflect-metadata';
import request = require('supertest');
import express = require('express');
import { expect } from 'chai';
import { Container } from 'typedi';
import { isCelebrateError } from 'celebrate';

import complementaryTaskCategoryRoute from '../../src/api/routes/complementaryTaskCategoryRoute';
import { ComplementaryTaskCategoryMap } from '../../src/mappers/ComplementaryTaskCategoryMap';

describe('E2E API Test: Complementary Task Categories (System Level)', () => {
    let app: express.Application;
    let memoryDb: any[] = [];

    beforeEach(() => {
        Container.reset();
        memoryDb = [];

        // 1. MOCK APENAS DO REPOSITÓRIO
        const categoryRepoMock = {
            save: async (category: any) => {
                const persistence = ComplementaryTaskCategoryMap.toPersistence(category);
                const index = memoryDb.findIndex(c => c.id === persistence.domainID); // Nota: persistence usa domainID
                if (index >= 0) {
                    memoryDb[index] = persistence;
                } else {
                    memoryDb.push(persistence);
                }
            },
            findByCode: async (code: string) => {
                const found = memoryDb.find(c => c.code === code);
                return found ? ComplementaryTaskCategoryMap.toDomain({
                    ...found,
                    // Pequeno ajuste para garantir que o Mapper consegue reconstruir o objeto se necessário
                    // O seu Mapper.toDomain usa 'raw.code', etc.
                }) : null;
            },
            findAll: async () => {
                return memoryDb.map(item => ComplementaryTaskCategoryMap.toDomain(item));
            },
            delete: async (id: string) => {
                const initialLen = memoryDb.length;
                memoryDb = memoryDb.filter(c => c.domainID !== id);
                return memoryDb.length < initialLen;
            }
        };

        Container.set('ComplementaryTaskCategoryRepo', categoryRepoMock);

        // 2. SETUP DA APP
        app = express();
        app.use(express.json());
        complementaryTaskCategoryRoute(app);

        app.use((err: any, req: any, res: any, next: any) => {
            if (isCelebrateError(err)) {
                return res.status(400).json({ error: "Validation Failed", details: err.details });
            }
            return res.status(500).json({ error: err.message });
        });
    });

    // =========================================================
    // CASOS DE TESTE CORRIGIDOS
    // =========================================================

    it('POST /complementary-task-categories deve criar uma categoria com sucesso', async () => {
        const payload = {
            name: "Security Check",
            code: "SEC-001",
            description: "Standard security validation"
        };

        const res = await request(app)
            .post('/complementary-task-categories')
            .send(payload);

        // 1. Verificações de HTTP
        expect(res.status).to.equal(201);

        // CORREÇÃO: Removemos a expectativa de 'id' pois o DTO não o devolve
        // expect(res.body).to.have.property('id');

        expect(res.body).to.have.property('code', 'SEC-001');
        expect(res.body).to.have.property('name', 'Security Check');

        // 2. Verificação de "Persistência"
        expect(memoryDb).to.have.lengthOf(1);
        expect(memoryDb[0].code).to.equal('SEC-001');
    });

    it('POST /complementary-task-categories deve falhar ao duplicar código', async () => {
        memoryDb.push({
            domainID: 'existing-id', // Simula estrutura de persistência
            code: 'SEC-001',
            name: 'Old One',
            description: 'desc'
        });

        const res = await request(app)
            .post('/complementary-task-categories')
            .send({
                name: "New Security",
                code: "SEC-001",
                description: "Should fail"
            });

        expect(res.status).to.equal(400);
        expect(res.body.error).to.include('already exists');
    });

    it('POST /complementary-task-categories deve falhar validação de input', async () => {
        const res = await request(app)
            .post('/complementary-task-categories')
            .send({
                name: "No Code"
            });

        expect(res.status).to.equal(400);
        expect(res.body.error).to.equal('Validation Failed');
    });
});