import {Container} from 'typedi';
import VesselVisitExecutionSchema from '../persistence/schemas/VesselVisitExecutionSchema';
import VesselVisitExecutionRepo from '../persistence/repos/VesselVisitExecutionRepo';

// Importa o Schema e o Repo
import OperationPlanSchema from '../persistence/schemas/OperationPlanSchema';
import OperationPlanRepo from '../persistence/repos/OperationPlanRepo';
import PlanningModuleAdapter from '../adapters/PlanningModuleAdapter';
import PortModuleAdapter from '../adapters/PortModuleAdapter';
import IncidentSchema from "../persistence/schemas/IncidentSchema";
import IncidentRepo from "../persistence/repos/IncidentRepo";
import IncidentTypeSchema from "../persistence/schemas/IncidentTypeSchema";
import IncidentTypeRepo from "../persistence/repos/IncidentTypeRepo";

export default () => {
    try {
        // 1. Injetar o Schema do Mongoose
        // O nome 'operationPlanSchema' tem de bater certo com o @Inject no Repo
        Container.set('operationPlanSchema', OperationPlanSchema);

        // 2. Injetar o Adapter Externo
        // O nome 'PlanningAdapter' tem de bater certo com o @Inject no Service
        Container.set('PlanningAdapter', new PlanningModuleAdapter());

        // US 4.1.5 - Injetar o Adapter do Port Module
        Container.set('PortAdapter', new PortModuleAdapter());

        // 3. Injetar o Repositório
        // O nome 'OperationPlanRepo' tem de bater certo com o @Inject no Service
        // Nota: Como o Repo tem dependências (@Inject no construtor), instanciamos via Container ou deixamos a classe
        const planRepo = new OperationPlanRepo(OperationPlanSchema);
        Container.set('OperationPlanRepo', planRepo);

        Container.set('VesselVisitExecutionSchema', VesselVisitExecutionSchema);

        // Repositório do VVE
        const vveRepo = new VesselVisitExecutionRepo(VesselVisitExecutionSchema);
        Container.set('VesselVisitExecutionRepo', vveRepo);

        // Incident Type
        Container.set('IncidentTypeSchema', IncidentTypeSchema);
        const incidentTypeRepo = new IncidentTypeRepo(IncidentTypeSchema);
        Container.set('IncidentTypeRepo', incidentTypeRepo);

        // Incident
        Container.set('incidentSchema', IncidentSchema);
        const incidentRepo = new IncidentRepo(IncidentSchema);
        Container.set('IncidentRepo', incidentRepo);

        console.log('✌️ Dependency Injector loaded');
    } catch (e) {
        console.error('🔥 Error on dependency injector loader: %o', e);
        throw e;
    }
};