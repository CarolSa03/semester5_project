import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IIncidentTypeRepo} from "../../domain/incidentType/IIncidentTypeRepo";
import {IIncidentRepo} from "../../domain/incident/IIncidentRepo"; // Importar Interface

@Service()
export default class DeleteIncidentTypeService {
    constructor(
        @Inject('IncidentTypeRepo') private incidentTypeRepo: IIncidentTypeRepo,
        @Inject('IncidentRepo') private incidentRepo: IIncidentRepo // INJEÇÃO NOVA
    ) {}

    public async execute(id: string): Promise<Result<void>> {
        try {
            const incidentType = await this.incidentTypeRepo.findById(id);

            if (!incidentType) {
                return Result.fail<void>("Incident Type not found.");
            }

            // --- PROTEÇÃO DE INTEGRIDADE ---
            // Verifica se este tipo está a ser usado por algum incidente
            const usageCount = await this.incidentRepo.countByTypeId(id);
            if (usageCount > 0) {
                return Result.fail<void>(`Cannot delete Incident Type: It is currently used by ${usageCount} incident(s).`);
            }
            // -------------------------------

            // Se não for usado, prossegue com o Soft Delete (ou Hard Delete, conforme preferires)
            incidentType.props.active = false; // Soft delete: apenas desativa

            // Se preferisses apagar mesmo da BD, usarias: await this.incidentTypeRepo.delete(id);
            // Mas manter como inativo (soft delete) é mais seguro para históricos futuros.
            await this.incidentTypeRepo.save(incidentType);

            return Result.ok<void>();
        } catch (e) {
            return Result.fail<void>(e instanceof Error ? e.message : "Unknown error deleting incident type");
        }
    }
}