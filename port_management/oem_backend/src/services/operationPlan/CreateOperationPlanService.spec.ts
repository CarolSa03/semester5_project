import 'reflect-metadata';
import * as sinon from 'sinon';
import {expect} from 'chai';
import {Container} from 'typedi';
import {Result} from '../../core/logic/Result'; // Importante
import CreateOperationPlanService from './CreateOperationPlanService';

describe('Integration Test: CreateOperationPlanService', () => {
    let service: CreateOperationPlanService;
    let planRepoMock: any;
    let planningAdapterMock: any;

    beforeEach(() => {
        Container.reset();

        planRepoMock = {
            save: sinon.stub().resolves()
        };

        // CORREÇÃO: O Adapter deve retornar um Result.ok(...)
        planningAdapterMock = {
            fetchSchedule: sinon.stub().resolves(Result.ok({
                requestId: "req-test-001",
                success: true,
                totalDelay: 0,
                // A estrutura deve bater certo com o Mapper
                schedule: [
                    {
                        vesselId: "V001",
                        dockId: "D1",
                        startTime: 600,
                        endTime: 720,
                        assignedCrane: ["C1"],
                        operationId: "OP-001",
                        resourceId: "CRANE-01", // Adicionado para satisfazer o Mapper
                        realStartTime: new Date().toISOString()
                    }
                ]
            }))
        };

        Container.set('OperationPlanRepo', planRepoMock);
        Container.set('PlanningAdapter', planningAdapterMock);
        service = Container.get(CreateOperationPlanService);
    });

    afterEach(() => { sinon.restore(); });


    it('deve falhar quando o adaptador de planeamento retorna erro', async () => {
        // CORREÇÃO: Mock retorna Result.fail
        planningAdapterMock.fetchSchedule.resolves(Result.fail('Planning algorithm timeout'));

        // O dto deve ser compatível com a assinatura execute(dto: {...})
        const result = await service.execute({ vvnId: 'VVN-001', date: new Date('2025-12-25'), algorithm: 'genetic' });

        expect(result.isFailure).to.be.true;
        expect(result.error).to.include('Planning algorithm timeout');
    });

    it('deve falhar quando o schedule retornado está vazio', async () => {
        // CORREÇÃO: Mock retorna Result.ok com array vazio
        planningAdapterMock.fetchSchedule.resolves(Result.ok({
            success: true,
            algorithm: "IARTI_Genetic_Algorithm_OX1",
            totalDelay: 0,
            schedule: []
        }));

        const result = await service.execute({ vvnId: 'VVN-001', date: new Date('2025-12-25'), algorithm: 'genetic' });

        // O teu serviço falha se o Mapper retornar null ou se a validação do domínio falhar (operations vazio)
        expect(result.isFailure).to.be.true;
        expect(planRepoMock.save.called).to.be.false;
    });

    it('deve falhar quando o mapper retorna null para dados incompletos', async () => {
        // CORREÇÃO: Result.ok mas com dados que quebram o Mapper (sem vesselId)
        planningAdapterMock.fetchSchedule.resolves(Result.ok({
            success: true,
            schedule: [
                {
                    dockId: "D1",
                    startTime: 600,
                    endTime: 720
                    // vesselId ausente propositadamente
                }
            ]
        }));

        const result = await service.execute({ vvnId: 'VVN-001', date: new Date('2025-12-25'), algorithm: 'genetic' });

        expect(result.isFailure).to.be.true;
        expect(planRepoMock.save.called).to.be.false;
    });
});