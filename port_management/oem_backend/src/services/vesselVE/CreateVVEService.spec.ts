import 'reflect-metadata';
import * as sinon from 'sinon';
import { expect } from 'chai';
import { Container } from 'typedi';
import { Result } from '../../core/logic/Result';
import CreateVesselVisitExecutionService from './CreateVesselVisitExecutionService';
import { VesselVisitExecution } from '../../domain/vesselVE/VesselVisitExecution';

describe('Integration Test: CreateVesselVisitExecutionService', () => {
    let service: CreateVesselVisitExecutionService;
    let vveRepoMock: any;

    beforeEach(() => {
        Container.reset();

        // Mock do repositório
        vveRepoMock = {
            findByVVN: sinon.stub().resolves(null),
            save: sinon.stub().resolves()
        };

        Container.set('VesselVisitExecutionRepo', vveRepoMock);

        service = Container.get(CreateVesselVisitExecutionService);
    });

    afterEach(() => {
        sinon.restore();
    });

    it('deve falhar quando a data fornecida é inválida', async () => {
        const result = await service.execute('VVN-001', 'invalid-date');

        expect(result.isFailure).to.be.true;
        expect(result.error).to.include('Invalid date format');
        expect(vveRepoMock.save.called).to.be.false;
    });

    it('deve falhar quando já existe um VVE para o mesmo VVN', async () => {
        const existingVVE = VesselVisitExecution.create({ vvnId: 'VVN-001', arrivalDate: new Date() }).getValue();
        vveRepoMock.findByVVN.resolves(existingVVE);

        const result = await service.execute('VVN-001', '2026-01-03T12:00:00Z');

        expect(result.isFailure).to.be.true;
        expect(result.error).to.include('already exists');
        expect(vveRepoMock.save.called).to.be.false;
    });

    it('deve criar com sucesso um novo VVE', async () => {
        const result = await service.execute('VVN-002', '2026-01-03T12:00:00Z');

        expect(result.isSuccess).to.be.true;
        expect(result.getValue()).to.have.property('vvnId', 'VVN-002');
        expect(result.getValue()).to.have.property('arrivalDate', '2026-01-03T12:00:00.000Z');
        expect(result.getValue()).to.have.property('status', 'IN_PROGRESS');
        expect(vveRepoMock.save.calledOnce).to.be.true;
    });

    it('deve propagar erro caso a criação falhe internamente', async () => {
        // Forçando falha ao criar
        sinon.stub(VesselVisitExecution, 'create').returns(Result.fail('Erro interno de criação'));

        const result = await service.execute('VVN-003', '2026-01-03T12:00:00Z');

        expect(result.isFailure).to.be.true;
        expect(result.error).to.include('Erro interno de criação');
        expect(vveRepoMock.save.called).to.be.false;
    });
});
