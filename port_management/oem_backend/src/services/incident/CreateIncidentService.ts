import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IIncidentRepo} from "../../domain/incident/IIncidentRepo";
import {IIncidentTypeRepo} from "../../domain/incidentType/IIncidentTypeRepo";
import {IVesselVisitExecutionRepo} from "../../domain/vesselVE/IVesselVisitExecutionRepo"; // Nova dependência
import {ICreateIncidentDTO} from "../../dto/incident/ICreateIncidentDTO";
import {IIncidentDTO} from "../../dto/incident/IIncidentDTO";
import {Incident} from "../../domain/incident/Incident";
import {IncidentMap} from "../../mappers/IncidentMap";
import {IncidentTypeSeverity} from "../../domain/incidentType/enums/IncidentTypeSeverity.enum";

@Service()
export default class CreateIncidentService {
    constructor(
        @Inject('IncidentRepo') private incidentRepo: IIncidentRepo,
        @Inject('IncidentTypeRepo') private incidentTypeRepo: IIncidentTypeRepo,
        // Injetamos o repositório de VVE para validar se os navios existem
        @Inject('VesselVisitExecutionRepo') private vveRepo: IVesselVisitExecutionRepo
    ) {}

    public async execute(dto: ICreateIncidentDTO): Promise<Result<IIncidentDTO>> {
        try {
            // Validações básicas
            if (!dto.description || dto.description.trim().length === 0) {
                return Result.fail<IIncidentDTO>("Description is required.");
            }

            // 1. Validar se o IncidentType existe (pelo Código)
            const incidentType = await this.incidentTypeRepo.findByCode(dto.incidentTypeCode);
            if (!incidentType) {
                return Result.fail<IIncidentDTO>(`Incident Type with code '${dto.incidentTypeCode}' not found.`);
            }

            // 2. Validar Existência dos VVEs Afetados (Lógica Adicionada)
            // Se o incidente reporta navios afetados, temos de garantir que eles existem no sistema.
            if (dto.affectedVvnIds && dto.affectedVvnIds.length > 0) {
                for (const vvnId of dto.affectedVvnIds) {
                    const vve = await this.vveRepo.findByVVN(vvnId);
                    if (!vve) {
                        return Result.fail<IIncidentDTO>(`Cannot create incident: Vessel Visit '${vvnId}' not found or not started.`);
                    }
                }
            }

            // 3. Determinar a Severidade
            // Se o DTO traz severidade, usa-a (override). Se não, herda do Tipo de Incidente.
            const severity = dto.severity
                ? (dto.severity as IncidentTypeSeverity)
                : incidentType.severity;

            // 4. Criar a Entidade de Domínio
            const incidentOrError = Incident.create({
                incidentTypeId: incidentType.id.toString(), // ID Interno do tipo
                description: dto.description,
                severity: severity,
                affectedVvnIds: dto.affectedVvnIds || [],
                createdBy: dto.createdBy
            });

            if (incidentOrError.isFailure) {
                return Result.fail<IIncidentDTO>(incidentOrError.error);
            }

            const incident = incidentOrError.getValue();

            // 5. Persistir
            await this.incidentRepo.save(incident);

            // 6. Retornar DTO
            const incidentDTO = IncidentMap.toDTO(incident);
            return Result.ok<IIncidentDTO>(incidentDTO);

        } catch (e) {
            console.error(e);
            return Result.fail<IIncidentDTO>(e instanceof Error ? e.message : "Unknown error creating incident");
        }
    }
}