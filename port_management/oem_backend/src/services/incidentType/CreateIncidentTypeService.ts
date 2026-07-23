import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IIncidentTypeRepo} from "../../domain/incidentType/IIncidentTypeRepo";
import {IIncidentTypeDTO} from "../../dto/incidentType/IIncidentTypeDTO";
import {IncidentType} from "../../domain/incidentType/IncidentType";
import {IncidentTypeSeverity} from "../../domain/incidentType/enums/IncidentTypeSeverity.enum";
import {IncidentTypeMap} from "../../mappers/IncidentTypeMap";

@Service()
export default class CreateIncidentTypeService {
    constructor(
        @Inject('IncidentTypeRepo') private repo: IIncidentTypeRepo
    ) {}

    public async execute(dto: IIncidentTypeDTO): Promise<Result<IIncidentTypeDTO>> {
        try {
            // 1. Verificar se o código já existe
            const exists = await this.repo.findByCode(dto.code);
            if (exists) return Result.fail("Incident Type Code already exists.");

            // 2. Verificar se o Parent existe (se for fornecido)
            if (dto.parentId) {
                // Assumindo que o parentId no DTO é o ID interno ou o CODE.
                // Vamos assumir que o frontend envia o ID do domínio do pai.
                const parent = await this.repo.findById(dto.parentId);
                if (!parent) return Result.fail("Parent Incident Type not found.");
            }

            // 3. Criar Agregado
            const incidentTypeOrError = IncidentType.create({
                code: dto.code,
                name: dto.name,
                description: dto.description,
                severity: dto.severity as IncidentTypeSeverity,
                parentId: dto.parentId
            });

            if (incidentTypeOrError.isFailure) {
                return Result.fail(incidentTypeOrError.error);
            }

            const incidentType = incidentTypeOrError.getValue();

            // 4. Persistir
            await this.repo.save(incidentType);

            // 5. Retornar DTO
            return Result.ok<IIncidentTypeDTO>(IncidentTypeMap.toDTO(incidentType));

        } catch (e) {
            return Result.fail(e);
        }
    }
}