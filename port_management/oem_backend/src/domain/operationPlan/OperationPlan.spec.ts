// typescript
import {expect} from 'chai';
import {OperationPlan} from './OperationPlan';
import {PlannedOperation} from './PlannedOperation';
import {OperationType} from './enums/OperationType.enum';
import {TimeRange} from '../shared/TimeRange.vo';
import {PlanStatus} from './enums/PlanStatus.enum';

describe('Unit Test: OperationPlan (Domain)', () => {
    it('deve criar com sucesso quando os dados são válidos', () => {
        // Criar uma operação válida primeiro
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const result = OperationPlan.create({
            vvnId: 'VVN-123',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: 10,
            computationTime: 1.2,
            operations: [operation] // Alterado de tasks para operations
        });

        expect(result.isSuccess).to.be.true;
        expect(result.getValue().algorithm).to.equal('IARTI_Genetic');
        expect(result.getValue().operations).to.have.lengthOf(1);
    });

    it('deve falhar ao criar com vvnId vazio', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const result = OperationPlan.create({
            vvnId: '',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: 10,
            computationTime: 1.2,
            operations: [operation]
        });

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('vvnId cannot be empty');
    });

    it('deve falhar ao criar com data inválida', () => {
        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(new Date(), new Date(Date.now() + 3600000)),
            'D1'
        );

        const result = OperationPlan.create({
            vvnId: 'VVN-123',
            date: new Date('invalid'),
            algorithm: 'IARTI_Genetic',
            totalDelay: 10,
            computationTime: 1.2,
            operations: [operation]
        });

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('date must be a valid Date object');
    });

    it('deve falhar ao criar com algoritmo vazio', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const result = OperationPlan.create({
            vvnId: 'VVN-123',
            date: futureDate,
            algorithm: '',
            totalDelay: 10,
            computationTime: 1.2,
            operations: [operation]
        });

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('algorithm cannot be empty');
    });

    it('deve falhar ao criar com totalDelay negativo', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const result = OperationPlan.create({
            vvnId: 'VVN-123',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: -5,
            computationTime: 1.2,
            operations: [operation]
        });

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('totalDelay cannot be negative');
    });

    it('deve falhar ao criar com computationTime negativo', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const result = OperationPlan.create({
            vvnId: 'VVN-123',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: 10,
            computationTime: -1.5,
            operations: [operation]
        });

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('computationTime cannot be negative');
    });

    it('deve falhar ao criar com array de operations vazio', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const result = OperationPlan.create({
            vvnId: 'VVN-123',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: 10,
            computationTime: 1.2,
            operations: []
        });

        expect(result.isFailure).to.be.true;
        expect(result.error).to.equal('operations array cannot be empty');
    });

    it('deve criar com sucesso com múltiplas operações', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation1 = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const operation2 = PlannedOperation.create(
            OperationType.UNLOADING,
            TimeRange.create(new Date(futureDate.getTime() + 3600000), new Date(futureDate.getTime() + 7200000)),
            'D2'
        );

        const result = OperationPlan.create({
            vvnId: 'VVN-456',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: 15,
            computationTime: 2.5,
            operations: [operation1, operation2]
        });

        expect(result.isSuccess).to.be.true;
        expect(result.getValue().operations).to.have.lengthOf(2);
        expect(result.getValue().vvnId).to.equal('VVN-456');
    });

    it('deve atualizar plano com updatePlan', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const plan = OperationPlan.create({
            vvnId: 'VVN-123',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: 10,
            computationTime: 1.2,
            operations: [operation]
        }).getValue();

        const newOperation = PlannedOperation.create(
            OperationType.UNLOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 1800000)),
            'D2'
        );

        plan.updatePlan(
            {status: PlanStatus.ARCHIVED, operations: [newOperation]},
            'admin',
            'Schedule change'
        );

        expect(plan.status).to.equal(PlanStatus.ARCHIVED);
        expect(plan.operations).to.have.lengthOf(1);
        expect(plan.lastModifiedBy).to.equal('admin');
        expect(plan.modificationReason).to.equal('Schedule change');
        expect(plan.lastModified).to.be.instanceOf(Date);
    });

    it('deve ter status ACTIVE por padrão ao criar', () => {
        const futureDate = new Date();
        futureDate.setDate(futureDate.getDate() + 1);

        const operation = PlannedOperation.create(
            OperationType.LOADING,
            TimeRange.create(futureDate, new Date(futureDate.getTime() + 3600000)),
            'D1'
        );

        const result = OperationPlan.create({
            vvnId: 'VVN-789',
            date: futureDate,
            algorithm: 'IARTI_Genetic',
            totalDelay: 0,
            computationTime: 0.5,
            operations: [operation]
        });

        expect(result.isSuccess).to.be.true;
        expect(result.getValue().status).to.equal(PlanStatus.ACTIVE);
    });

});