import {expect} from 'chai';
import {IncidentType} from './IncidentType';
import {IncidentTypeSeverity} from './enums/IncidentTypeSeverity.enum';

describe('Unit Test: IncidentType (Domain Entity)', () => {
    it('deve criar um IncidentType válido com sucesso', () => {
        const props = {
            code: 'T-UNIT-01',
            name: 'Teste Unitário',
            description: 'Descrição válida',
            severity: IncidentTypeSeverity.MINOR
        };
        const result = IncidentType.create(props);
        expect(result.isSuccess).to.be.true;
        expect(result.getValue().code).to.equal('T-UNIT-01');
    });

    it('deve falhar se o Código não for fornecido', () => {
        const props = { code: '', name: 'Falha', description: 'Sem código', severity: IncidentTypeSeverity.MAJOR };
        const result = IncidentType.create(props);
        expect(result.isFailure).to.be.true;
    });

    it('deve falhar se o Nome não for fornecido', () => {
        const props = {code: 'T-UNIT-02', name: '', description: 'Sem nome', severity: IncidentTypeSeverity.MINOR};
        const result = IncidentType.create(props);
        expect(result.isFailure).to.be.true;
    });

    it('deve falhar se a Descrição não for fornecida', () => {
        const props = {code: 'T-UNIT-03', name: 'Teste', description: '', severity: IncidentTypeSeverity.MAJOR};
        const result = IncidentType.create(props);
        expect(result.isFailure).to.be.true;
    });

    it('deve falhar se a Severidade não for fornecida', () => {
        const props = {code: 'T-UNIT-04', name: 'Teste', description: 'Sem severidade', severity: null as any};
        const result = IncidentType.create(props);
        expect(result.isFailure).to.be.true;
    });

    it('deve criar um IncidentType válido com severidade CRITICAL', () => {
        const props = {
            code: 'T-UNIT-05',
            name: 'Teste Critical',
            description: 'Descrição com severidade crítica',
            severity: IncidentTypeSeverity.CRITICAL
        };
        const result = IncidentType.create(props);
        expect(result.isSuccess).to.be.true;
        expect(result.getValue().severity).to.equal(IncidentTypeSeverity.CRITICAL);
    });

    it('deve criar um IncidentType válido com severidade MAJOR', () => {
        const props = {
            code: 'T-UNIT-06',
            name: 'Teste Major',
            description: 'Descrição com severidade major',
            severity: IncidentTypeSeverity.MAJOR
        };
        const result = IncidentType.create(props);
        expect(result.isSuccess).to.be.true;
        expect(result.getValue().severity).to.equal(IncidentTypeSeverity.MAJOR);
    });

    it('deve criar um IncidentType válido e verificar todas as propriedades', () => {
        const props = {
            code: 'T-UNIT-07',
            name: 'Teste Completo',
            description: 'Descrição completa do teste',
            severity: IncidentTypeSeverity.MINOR
        };
        const result = IncidentType.create(props);
        expect(result.isSuccess).to.be.true;
        const incidentType = result.getValue();
        expect(incidentType.code).to.equal('T-UNIT-07');
        expect(incidentType.name).to.equal('Teste Completo');
        expect(incidentType.description).to.equal('Descrição completa do teste');
        expect(incidentType.severity).to.equal(IncidentTypeSeverity.MINOR);
    });

});