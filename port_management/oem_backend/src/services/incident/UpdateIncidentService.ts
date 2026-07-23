import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IIncidentRepo} from "../../domain/incident/IIncidentRepo";
import {IVesselVisitExecutionRepo} from "../../domain/vesselVE/IVesselVisitExecutionRepo"; // Injetar Repo VVE
import {IUpdateIncidentDTO} from "../../dto/incident/IUpdateIncidentDTO";
import {IIncidentDTO} from "../../dto/incident/IIncidentDTO";
import {IncidentMap} from "../../mappers/IncidentMap";
import {IncidentTypeSeverity} from "../../domain/incidentType/enums/IncidentTypeSeverity.enum";

@Service()
export default class UpdateIncidentService {
    constructor(
        @Inject('IncidentRepo') private incidentRepo: IIncidentRepo,
        @Inject('VesselVisitExecutionRepo') private vveRepo: IVesselVisitExecutionRepo // Injeção necessária
    ) {}

    public async execute(dto: IUpdateIncidentDTO): Promise<Result<IIncidentDTO>> {
        try {
            const incident = await this.incidentRepo.findById(dto.id);

            if (!incident) {
                return Result.fail<IIncidentDTO>("Incident not found");
            }

            // 1. Atualizar Descrição
            if (dto.description) {
                incident.changeDescription(dto.description);
            }

            // 2. Atualizar Severidade
            if (dto.severity) {
                incident.changeSeverity(dto.severity as IncidentTypeSeverity);
            }

            // 3. Atualizar e Validar VVEs afetados
            if (dto.affectedVvnIds) {
                // Validação de Existência (Igual ao Create)
                for (const vvnId of dto.affectedVvnIds) {
                    const vve = await this.vveRepo.findByVVN(vvnId);
                    if (!vve) {
                        return Result.fail<IIncidentDTO>(`Cannot update incident: Vessel Visit '${vvnId}' not found.`);
                    }
                }
                incident.updateAffectedVVEs(dto.affectedVvnIds);
            }

            // 4. Resolver Incidente
            if (dto.endTime) {
                const endDate = new Date(dto.endTime);
                if (isNaN(endDate.getTime())) {
                    return Result.fail<IIncidentDTO>("Invalid endTime format.");
                }
                incident.resolve(endDate);
            }

            // 5. Persistir
            await this.incidentRepo.save(incident);

            return Result.ok<IIncidentDTO>(IncidentMap.toDTO(incident));

        } catch (e) {
            console.error(e);
            return Result.fail<IIncidentDTO>(e instanceof Error ? e.message : "Unknown error updating incident");
        }
    }
}