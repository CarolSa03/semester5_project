import 'reflect-metadata';
import * as sinon from 'sinon';
import {expect} from 'chai';
import {Container} from 'typedi';
import CreateIncidentService from './CreateIncidentService';

describe('Integration Test: Incident Services (US 4.1.13)', () => {
    let incidentRepoMock: any;
    let incidentTypeRepoMock: any;
    let createService: CreateIncidentService;

    beforeEach(() => {
        Container.reset();
        const vveRepoMock = {
            findByVVN: async () => ({ id: 'VVE-123', vvnId: 'VVN-001' }), // Retorna algo válido
            save: async () => {},
            exists: async () => true
        };
        Container.set('VesselVisitExecutionRepo', vveRepoMock);
        incidentRepoMock = { save: sinon.stub().resolves(), findById: sinon.stub() };
        incidentTypeRepoMock = { findByCode: sinon.stub().resolves({ id: 't1', severity: 'Major' }) };

        Container.set('IncidentRepo', incidentRepoMock);
        Container.set('IncidentTypeRepo', incidentTypeRepoMock);
        createService = Container.get(CreateIncidentService);
    });

    afterEach(() => { sinon.restore(); });

    it('deve criar um incidente com sucesso', async () => {
        const result = await createService.execute({ incidentTypeCode: 'AC-01', description: 'Test', createdBy: 'U1' } as any);
        expect(result.isSuccess).to.be.true;
        expect(incidentRepoMock.save.calledOnce).to.be.true;
    });

    it('deve falhar ao criar incidente com tipo inexistente', async () => {
        incidentTypeRepoMock.findByCode.resolves(null);
        const result = await createService.execute({
            incidentTypeCode: 'INVALID',
            description: 'Test',
            createdBy: 'U1'
        } as any);
        expect(result.isFailure).to.be.true;
        expect(incidentRepoMock.save.called).to.be.false;
    });

    it('deve falhar ao criar incidente sem descrição', async () => {
        const result = await createService.execute({
            incidentTypeCode: 'AC-01',
            description: '',
            createdBy: 'U1'
        } as any);
        expect(result.isFailure).to.be.true;
        expect(incidentRepoMock.save.called).to.be.false;
    });

    it('deve falhar ao criar incidente sem createdBy', async () => {
        const result = await createService.execute({
            incidentTypeCode: 'AC-01',
            description: 'Test',
            createdBy: null
        } as any);
        expect(result.isFailure).to.be.true;
        expect(incidentRepoMock.save.called).to.be.false;
    });

    it('deve falhar quando o repositório não conseguir salvar', async () => {
        incidentRepoMock.save.rejects(new Error('Database error'));
        const result = await createService.execute({
            incidentTypeCode: 'AC-01',
            description: 'Test',
            createdBy: 'U1'
        } as any);
        expect(result.isFailure).to.be.true;
    });

    it('deve criar um incidente com sucesso incluindo campos opcionais', async () => {
        const result = await createService.execute({
            incidentTypeCode: 'AC-01',
            description: 'Detailed test description',
            createdBy: 'U1',
            location: 'Dock A',
            affectedResources: ['Crane-1', 'Crane-2']
        } as any);
        expect(result.isSuccess).to.be.true;
        expect(incidentRepoMock.save.calledOnce).to.be.true;
    });

});