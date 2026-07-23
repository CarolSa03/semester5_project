import 'reflect-metadata';
import request = require('supertest');
import express = require('express');
import { Container } from 'typedi';
import { Result } from '../../src/core/logic/Result';
import { isCelebrateError } from 'celebrate';
import incidentTypeRoute from "../../src/api/routes/incidentTypeRoute";
import CreateIncidentTypeService from "../../src/services/incidentType/CreateIncidentTypeService";
import { expect } from "chai";

describe('E2E API Test: Incident Types (Application Level)', () => {
    let app: express.Application;

    before(() => {
        Container.reset();

        // =========================================================
        // MOCK DO SERVIÇO (Isolamento da Aplicação)
        // =========================================================
        // Aqui definimos comportamentos fixos para o serviço, sem lógica real.
        const createServiceMock = {
            execute: async (dto: any) => {
                // Simulação de erro de conflito
                if (dto.code === 'DUPLICATE') {
                    return Result.fail('Incident Type code already exists');
                }

                // Simulação de sucesso
                return Result.ok({
                    id: "123", // ID fictício
                    code: dto.code,
                    name: dto.name,
                    description: dto.description,
                    severity: dto.severity
                });
            }
        };

        // Injetamos o MOCK do serviço, logo o serviço real NÃO é chamado
        Container.set(CreateIncidentTypeService, createServiceMock);

        // Mocks para outros serviços que o controller possa carregar
        Container.set('GetIncidentTypesService', {});
        Container.set('UpdateIncidentTypeService', {});
        Container.set('DeleteIncidentTypeService', {});

        app = express();
        app.use(express.json());
        incidentTypeRoute(app);

        app.use((err: any, req: any, res: any, next: any) => {
            if (isCelebrateError(err)) return res.status(400).json({ error: "Validation Failed" });
            res.status(500).json({ error: err.message });
        });
    });

    it('POST /incident-types deve retornar 201 (Sucesso Simulado)', async () => {
        const res = await request(app)
            .post('/incident-types')
            .send({
                code: "NEW-TYPE",
                name: "Test Type",
                description: "Desc",
                severity: "Minor"
            });

        expect(res.status).to.equal(201);
        expect(res.body).to.have.property('code', 'NEW-TYPE');
        // Confirma que é um mock (o ID fictício)
        expect(res.body).to.have.property('id', '123');
    });

    it('POST /incident-types deve retornar 400 se o serviço retornar erro', async () => {
        const res = await request(app)
            .post('/incident-types')
            .send({
                code: "DUPLICATE", // Trigger do mock
                name: "Test Type",
                description: "Desc",
                severity: "Minor"
            });

        expect(res.status).to.equal(400);
        expect(res.body.error).to.include('already exists');
    });

    it('POST /incident-types deve ser validado pelo Joi (Middleware)', async () => {
        const res = await request(app)
            .post('/incident-types')
            .send({
                name: "Sem Código"
                // Falta o campo code
            });

        expect(res.status).to.equal(400);
        expect(res.body.error).to.equal('Validation Failed');
    });
});