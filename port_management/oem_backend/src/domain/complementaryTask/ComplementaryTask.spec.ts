import 'reflect-metadata';
import { expect } from 'chai';
import { ComplementaryTask } from './ComplementaryTask';
import { UniqueEntityID } from '../../core/domain/UniqueEntityID';
import { TimeRange } from '../shared/TimeRange.vo';
import { TaskStatus } from './enums/TaskStatus.enum';

describe('Unit Test: ComplementaryTask (Domain)', () => {

    it('deve criar tarefa com estado PENDING por defeito', () => {
        const timeWindow = TimeRange.create(new Date(), new Date(Date.now() + 3600000));
        const result = ComplementaryTask.create({
            categoryId: new UniqueEntityID(),
            vveId: 'VVN-001',
            responsible: 'John Doe',
            timeWindow: timeWindow,
            isBlocking: true,
            createdBy: 'Admin'
        });

        expect(result.isSuccess).to.be.true;
        expect(result.getValue().status).to.equal(TaskStatus.PENDING);
    });

    it('isActiveAndBlocking deve retornar true apenas se a tarefa for bloqueante e estiver ativa', () => {
        const timeWindow = TimeRange.create(new Date(), new Date(Date.now() + 3600000));
        const task = ComplementaryTask.create({
            categoryId: new UniqueEntityID(),
            vveId: 'VVN-001',
            responsible: 'John Doe',
            timeWindow: timeWindow,
            isBlocking: true, // É bloqueante
            createdBy: 'Admin'
        }).getValue();

        // 1. PENDING + Blocking = True
        expect(task.isActiveAndBlocking()).to.be.true;

        // 2. IN_PROGRESS + Blocking = True
        task.startTask();
        expect(task.isActiveAndBlocking()).to.be.true;

        // 3. COMPLETED + Blocking = False
        task.markAsCompleted();
        expect(task.isActiveAndBlocking()).to.be.false;
    });

    it('não deve permitir iniciar uma tarefa já concluída', () => {
        const timeWindow = TimeRange.create(new Date(), new Date(Date.now() + 3600000));
        const task = ComplementaryTask.create({
            categoryId: new UniqueEntityID(),
            vveId: 'VVN-001',
            responsible: 'John Doe',
            timeWindow: timeWindow,
            isBlocking: false,
            createdBy: 'Admin'
        }).getValue();

        task.startTask();
        task.markAsCompleted();

        const result = task.startTask(); // Tentar iniciar novamente
        expect(result.isFailure).to.be.true;
        expect(result.error).to.include('Cannot start task');
    });
});