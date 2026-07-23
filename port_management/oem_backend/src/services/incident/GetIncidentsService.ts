import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IIncidentRepo} from "../../domain/incident/IIncidentRepo";
import {IIncidentDTO} from "../../dto/incident/IIncidentDTO";
import {IncidentMap} from "../../mappers/IncidentMap";
import {Incident} from "../../domain/incident/Incident"; // Importar a Classe

@Service()
export default class GetIncidentsService {
    constructor(
        @Inject('IncidentRepo') private incidentRepo: IIncidentRepo
    ) {}

    public async execute(filters: {
        status?: 'active' | 'resolved',
        vvnId?: string,
        startDate?: string,
        endDate?: string,
        severity?: string // Adicionado
    }): Promise<Result<IIncidentDTO[]>> {
        try {
            // FIX: Tipar explicitamente como array de Incidents para evitar o erro de 'any'
            let incidents: Incident[] = [];

            if (filters.status === 'active') {
                incidents = await this.incidentRepo.findActive();
            } else if (filters.vvnId) {
                incidents = await this.incidentRepo.findByVessel(filters.vvnId);
            } else if (filters.severity) {
                // Agora o metodo existe no Repo
                incidents = await this.incidentRepo.findBySeverity(filters.severity);
            } else if (filters.startDate && filters.endDate) {
                const start = new Date(filters.startDate);
                const end = new Date(filters.endDate);
                incidents = await this.incidentRepo.findByDateRange(start, end);
            } else {
                incidents = await this.incidentRepo.findAll();
            }

            // O erro 'implicit any' desaparece porque 'incidents' agora é Incident[]
            const dtos = incidents.map(incident => IncidentMap.toDTO(incident));
            return Result.ok<IIncidentDTO[]>(dtos);

        } catch (e) {
            return Result.fail<IIncidentDTO[]>(e);
        }
    }
}