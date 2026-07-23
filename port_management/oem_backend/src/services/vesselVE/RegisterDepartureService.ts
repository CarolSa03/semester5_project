import { Service, Inject } from 'typedi';
import { IVesselVisitExecutionRepo } from '../../domain/vesselVE/IVesselVisitExecutionRepo';
import { Result } from '../../core/logic/Result';
import config from '../../config';

@Service()
export default class RegisterDepartureService {
  constructor(
    @Inject('VesselVisitExecutionRepo') private repo: IVesselVisitExecutionRepo
  ) {}

  public async execute(vvnId: string, departureDateStr: string): Promise<Result<any>> {
    try {
      const visit = await this.repo.findByVVN(vvnId);

      if (!visit) {
        return Result.fail<any>(`Vessel Visit with ID ${vvnId} not found.`);
      }

      const props = (visit as any).props;
      
      if (props.status === 'COMPLETED') {
        return Result.fail<any>(`Visit ${vvnId} is already completed.`);
      }

      props.departureDate = new Date(departureDateStr);
      props.status = 'COMPLETED';
      
      await this.repo.save(visit);

      return Result.ok<any>({
        vvnId: props.vvnId,
        status: props.status,
        departureDate: props.departureDate
      });

    } catch (e: any) {
      return Result.fail<any>(e.message || "Unexpected error registering departure");
    }
  }
}
