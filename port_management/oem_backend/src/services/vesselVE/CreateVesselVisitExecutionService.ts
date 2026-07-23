import {Inject, Service} from "typedi";
import {IVesselVisitExecutionRepo} from "../../domain/vesselVE/IVesselVisitExecutionRepo";
import {VesselVisitExecution} from "../../domain/vesselVE/VesselVisitExecution";
import {Result} from "../../core/logic/Result";
import {ICreateVesselVisitExecutionDTO} from "../../dto/vesselVE/ICreateVesselVisitExecutionDTO";
import {VesselVisitExecutionMap} from "../../mappers/VesselVisitExecutionMap";


@Service()
export default class CreateVesselVisitExecutionService {
    constructor(
        @Inject('VesselVisitExecutionRepo') private vveRepo: IVesselVisitExecutionRepo
    ) {
    }

    public async execute(vvnId: string, dateStr: string): Promise<Result<ICreateVesselVisitExecutionDTO>> {
        try {
            const date = new Date(dateStr);
            if (isNaN(date.getTime())) {
                return Result.fail<ICreateVesselVisitExecutionDTO>("Invalid date format");
            }

            // Check if VVE already exists
            const existingVVE = await this.vveRepo.findByVVN(vvnId);
            if (existingVVE) {
                return Result.fail<ICreateVesselVisitExecutionDTO>(`Vessel Visit Execution for ${vvnId} already exists`);
            }

            // Create new VesselVisitExecution
            const vveOrError = VesselVisitExecution.create({vvnId: vvnId, arrivalDate: date});
            if (vveOrError.isFailure) {
                return Result.fail<ICreateVesselVisitExecutionDTO>(vveOrError.error);
            }
            const vve = vveOrError.getValue();

            // Save to repository
            await this.vveRepo.save(vve);


            return Result.ok<ICreateVesselVisitExecutionDTO>(VesselVisitExecutionMap.toDTO(vve));

        } catch (err: unknown) {
            return Result.fail<ICreateVesselVisitExecutionDTO>(`Error creating Vessel Visit Execution: ${err}`);
        }

    }
}