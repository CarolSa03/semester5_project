import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IIncidentTypeRepo} from "../../domain/incidentType/IIncidentTypeRepo";
import {IIncidentTypeDTO} from "../../dto/incidentType/IIncidentTypeDTO";
import {IncidentTypeMap} from "../../mappers/IncidentTypeMap";
import {IncidentTypeSeverity} from "../../domain/incidentType/enums/IncidentTypeSeverity.enum";

@Service()
export default class UpdateIncidentTypeService {
    constructor(
        @Inject('IncidentTypeRepo') private repo: IIncidentTypeRepo
    ) {}

    public async execute(id: string, dto: Partial<IIncidentTypeDTO>): Promise<Result<IIncidentTypeDTO>> {
        try {
            const incidentType = await this.repo.findById(id);

            if (!incidentType) {
                return Result.fail("Incident Type not found.");
            }

            // Atualiza campos permitidos (Code geralmente não muda, mas depende do requisito)
            if (dto.name) incidentType.changeName(dto.name);
            if (dto.description) incidentType.changeDescription(dto.description);
            if (dto.severity) incidentType.changeSeverity(dto.severity as IncidentTypeSeverity);

            // Lógica de Parent
            if (dto.parentId !== undefined) {
                // Se enviar string vazia ou null, remove o pai
                if (dto.parentId === "" || dto.parentId === null) {
                    incidentType.changeParentId(undefined);
                } else {
                    // Valida se o novo pai existe e não é ele mesmo
                    if (dto.parentId === id) return Result.fail("Cannot be parent of itself.");
                    const parent = await this.repo.findById(dto.parentId);
                    if (!parent) return Result.fail("Parent Incident Type not found.");
                    incidentType.changeParentId(dto.parentId);
                }
            }

            await this.repo.save(incidentType);

            return Result.ok<IIncidentTypeDTO>(IncidentTypeMap.toDTO(incidentType));
        } catch (e) {
            return Result.fail(e);
        }
    }
}