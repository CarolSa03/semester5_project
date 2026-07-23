import {Inject, Service} from 'typedi';
import {Result} from "../../core/logic/Result";
import {IIncidentRepo} from "../../domain/incident/IIncidentRepo";

@Service()
export default class DeleteIncidentService {
    constructor(
        @Inject('IncidentRepo') private incidentRepo: IIncidentRepo
    ) {}

    public async execute(id: string): Promise<Result<void>> {
        try {
            const incident = await this.incidentRepo.findById(id);

            if (!incident) {
                return Result.fail<void>("Incident not found");
            }

            await this.incidentRepo.delete(id);
            return Result.ok<void>();
        } catch (e) {
            return Result.fail<void>(e);
        }
    }
}