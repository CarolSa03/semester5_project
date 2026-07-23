import 'reflect-metadata';
import {expect} from 'chai';
import {Incident} from './Incident';
import {IncidentTypeSeverity} from '../incidentType/enums/IncidentTypeSeverity.enum';

describe('Unit Test: Incident (Domain Logic)', () => {
    it('deve calcular a duração automaticamente ao resolver', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const end = new Date('2023-01-01T12:30:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-123', description: 'Test', severity: IncidentTypeSeverity.MINOR, createdBy: 'User', startTime: start
        }).getValue();

        incident.resolve(end);
        expect(incident.isActive).to.be.false;
        expect(incident.durationMinutes).to.equal(150);
    });

    it('deve criar um incidente com os parâmetros corretos', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const incidentResult = Incident.create({
            incidentTypeId: 'type-456',
            description: 'Test incident',
            severity: IncidentTypeSeverity.MAJOR,
            createdBy: 'TestUser',
            startTime: start
        });

        expect(incidentResult.isSuccess).to.be.true;
        const incident = incidentResult.getValue();
        expect(incident.incidentTypeId).to.equal('type-456');
        expect(incident.description).to.equal('Test incident');
        expect(incident.createdBy).to.equal('TestUser');
    });

    it('deve manter o incidente ativo após criação', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-789',
            description: 'Active incident',
            severity: IncidentTypeSeverity.CRITICAL,
            createdBy: 'User',
            startTime: start
        }).getValue();

        expect(incident.isActive).to.be.true;
    });

    it('deve calcular duração zero quando resolvido no mesmo momento', () => {
        const time = new Date('2023-01-01T10:00:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-101',
            description: 'Instant resolve',
            severity: IncidentTypeSeverity.MINOR,
            createdBy: 'User',
            startTime: time
        }).getValue();

        incident.resolve(time);
        expect(incident.isActive).to.be.false;
        expect(incident.durationMinutes).to.equal(0);
    });

    it('deve armazenar corretamente a severidade do incidente', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-202',
            description: 'Severity test',
            severity: IncidentTypeSeverity.CRITICAL,
            createdBy: 'User',
            startTime: start
        }).getValue();

        expect(incident.severity).to.equal(IncidentTypeSeverity.CRITICAL);
    });

    it('não deve permitir resolver com tempo de fim anterior ao tempo de início', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const end = new Date('2023-01-01T09:00:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-303',
            description: 'Invalid end time',
            severity: IncidentTypeSeverity.MINOR,
            createdBy: 'User',
            startTime: start
        }).getValue();

        incident.resolve(end);
        expect(incident.durationMinutes).to.be.lessThan(0);
    });

    it('deve armazenar corretamente a descrição do incidente', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const expectedDescription = 'Test description for incident';
        const incident = Incident.create({
            incidentTypeId: 'type-505',
            description: expectedDescription,
            severity: IncidentTypeSeverity.MINOR,
            createdBy: 'User',
            startTime: start
        }).getValue();

        expect(incident.description).to.equal(expectedDescription);
    });

    it('deve armazenar corretamente o incidentTypeId', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const expectedTypeId = 'type-606';
        const incident = Incident.create({
            incidentTypeId: expectedTypeId,
            description: 'Type ID test',
            severity: IncidentTypeSeverity.CRITICAL,
            createdBy: 'User',
            startTime: start
        }).getValue();

        expect(incident.incidentTypeId).to.equal(expectedTypeId);
    });

    it('deve armazenar corretamente o criador do incidente', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const expectedCreator = 'TestCreator';
        const incident = Incident.create({
            incidentTypeId: 'type-707',
            description: 'Creator test',
            severity: IncidentTypeSeverity.MAJOR,
            createdBy: expectedCreator,
            startTime: start
        }).getValue();

        expect(incident.createdBy).to.equal(expectedCreator);
    });

    it('deve armazenar corretamente o tempo de início', () => {
        const expectedStart = new Date('2023-01-01T10:00:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-808',
            description: 'Start time test',
            severity: IncidentTypeSeverity.MINOR,
            createdBy: 'User',
            startTime: expectedStart
        }).getValue();

        expect(incident.startTime).to.deep.equal(expectedStart);
    });

    it('deve ter endTime nulo quando o incidente não está resolvido', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-909',
            description: 'Unresolved incident',
            severity: IncidentTypeSeverity.CRITICAL,
            createdBy: 'User',
            startTime: start
        }).getValue();

        expect(incident.endTime).to.be.undefined;
    });

    it('deve definir endTime corretamente após resolução', () => {
        const start = new Date('2023-01-01T10:00:00Z');
        const end = new Date('2023-01-01T12:00:00Z');
        const incident = Incident.create({
            incidentTypeId: 'type-1010',
            description: 'End time test',
            severity: IncidentTypeSeverity.MAJOR,
            createdBy: 'User',
            startTime: start
        }).getValue();

        incident.resolve(end);
        expect(incident.endTime).to.deep.equal(end);
    });

});