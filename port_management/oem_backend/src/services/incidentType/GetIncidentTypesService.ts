import {Inject, Service} from 'typedi';
import {IIncidentTypeRepo} from '../../domain/incidentType/IIncidentTypeRepo';
import {IIncidentTypeDTO} from '../../dto/incidentType/IIncidentTypeDTO';
import {IncidentTypeMap} from '../../mappers/IncidentTypeMap';
import {Result} from '../../core/logic/Result';
import {IncidentType} from '../../domain/incidentType/IncidentType';

@Service()
export default class GetIncidentTypesService {
    constructor(
        @Inject('IncidentTypeRepo') private repo: IIncidentTypeRepo
    ) {}

    // Aceita parentId opcional
    public async execute(parentId?: string): Promise<Result<IIncidentTypeDTO[]>> {
        let types: IncidentType[];

        if (parentId) {
            types = await this.repo.findByParentId(parentId);
        } else {
            types = await this.repo.findAll();
        }

        const dtos = types.map(t => IncidentTypeMap.toDTO(t));
        return Result.ok<IIncidentTypeDTO[]>(dtos);
    }
}