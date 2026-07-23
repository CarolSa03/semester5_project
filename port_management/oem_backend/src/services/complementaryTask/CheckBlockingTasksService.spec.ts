import 'reflect-metadata';
import * as sinon from 'sinon';
import { expect } from 'chai';
import { Container } from 'typedi';
import { CheckBlockingTasksService } from './CheckBlockingTasksService';
import { TaskStatus } from '../../domain/complementaryTask/enums/TaskStatus.enum';

describe('Integration Test: CheckBlockingTasksService', () => {
    let service: CheckBlockingTasksService;
    let taskRepoMock: any;
    let categoryRepoMock: any;

    beforeEach(() => {
        Container.reset();

        // Mock dos dados retornados pelo repositório
        const mockTask = {
            id: 'task-1',
            categoryId: 'cat-1',
            vveId: 'VVN-001',
            status: TaskStatus.IN_PROGRESS,
            isBlocking: true,
            responsible: 'John',
            timeWindow: { getStartTime: () => new Date(), getEndTime: () => new Date() },
            createdAt: new Date()
        };

        const mockCategory = {
            id: 'cat-1',
            code: 'SEC-001',
            name: 'Security Check'
        };

        taskRepoMock = {
            // Simula que encontrou 1 tarefa bloqueante
            findBlockingTasksByVVE: sinon.stub().resolves([mockTask])
        };

        categoryRepoMock = {
            findById: sinon.stub().resolves(mockCategory)
        };

        Container.set('ComplementaryTaskRepo', taskRepoMock);
        Container.set('ComplementaryTaskCategoryRepo', categoryRepoMock);
        service = Container.get(CheckBlockingTasksService);
    });

    afterEach(() => { sinon.restore(); });

    it('deve indicar suspensão de operações quando existem tarefas bloqueantes', async () => {
        const result = await service.execute('VVN-001');

        expect(result.isSuccess).to.be.true;
        const dto = result.getValue();

        expect(dto.hasBlockingTasks).to.be.true;
        expect(dto.suspendCargoOperations).to.be.true;
        expect(dto.blockingTasks).to.have.lengthOf(1);
        expect(dto.message).to.include('SUSPENDED');
    });

    it('deve permitir operações quando não há tarefas bloqueantes', async () => {
        // Altera o comportamento do mock para retornar array vazio
        taskRepoMock.findBlockingTasksByVVE.resolves([]);

        const result = await service.execute('VVN-001');

        expect(result.isSuccess).to.be.true;
        const dto = result.getValue();

        expect(dto.hasBlockingTasks).to.be.false;
        expect(dto.suspendCargoOperations).to.be.false;
        expect(dto.message).to.include('can proceed');
    });
});