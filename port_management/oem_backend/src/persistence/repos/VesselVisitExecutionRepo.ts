import {Inject, Service} from 'typedi';
import {IVesselVisitExecutionRepo} from '../../domain/vesselVE/IVesselVisitExecutionRepo';
import {VesselVisitExecution} from '../../domain/vesselVE/VesselVisitExecution';
import {VesselVisitExecutionMap} from '../../mappers/VesselVisitExecutionMap';
import {ExecutedOperation} from '../../domain/vesselVE/ExecutedOperation';
import {OperationId} from '../../domain/operationPlan/value-objects/OperationId.vo';
import {TimeRange} from '../../domain/shared/TimeRange.vo';
import {OperationStatus} from '../../domain/operationPlan/enums/OperationStatus.enum';

@Service()
export default class VesselVisitExecutionRepo implements IVesselVisitExecutionRepo {
    // Mock data inline para testes sem BD
    private mockVVEs: any[] = [
        {
            vvnId: "VVN-2026-001",
            status: "COMPLETED",
            arrivalDate: new Date("2026-01-15T06:30:00Z"),
            departureDate: new Date("2026-01-15T14:45:00Z"),
            executedOperations: [
                {
                    operationId: "OP-001",
                    realStartTime: new Date("2026-01-15T07:00:00Z"),
                    realEndTime: new Date("2026-01-15T09:30:00Z"),
                    realResource: "CRANE-C1",
                    status: "COMPLETED",
                    completedBy: "operator-001"
                }
            ]
        },
        {
            vvnId: "VVN-2026-002",
            status: "COMPLETED",
            arrivalDate: new Date("2026-01-15T10:15:00Z"),
            departureDate: new Date("2026-01-15T19:00:00Z"),
            executedOperations: []
        },
        {
            vvnId: "VVN-2026-004",
            status: "IN_PROGRESS",
            arrivalDate: new Date("2026-01-16T08:00:00Z"),
            departureDate: null,
            executedOperations: []
        }
    ];

    constructor(
        // O nome 'VesselVisitExecutionSchema' tem de bater certo com o que definiste no loaders/index.ts
        @Inject('VesselVisitExecutionSchema') private executionSchema : any
    ) {}

    public async save(vve: VesselVisitExecution): Promise<void> {
        // Usamos o vvnId como chave única de negócio para encontrar o documento
        const query = { vvnId: vve.vvnId };

        // Convertemos o Agregado para o formato JSON simples que o Mongo entende
        const rawVVE = VesselVisitExecutionMap.toPersistence(vve);

        try {
            // 'upsert: true' é mágico:
            // - Se o documento não existir, CRIA.
            // - Se já existir, ATUALIZA.
            // Isto evita teres de fazer if(exists) save else create.
            await this.executionSchema.findOneAndUpdate(query, rawVVE, { upsert: true });
        } catch (err) {
            throw err;
        }
    }

    public async findByVVN(vvnId: string): Promise<VesselVisitExecution | null> {
        // Busca o documento cru no MongoDB
        const raw = await this.executionSchema.findOne({ vvnId });

        if (!raw) return null;

        // Reconstrói o Agregado com todas as regras de negócio
        return VesselVisitExecutionMap.toDomain(raw);
    }

    public async findById(id: string): Promise<VesselVisitExecution | null> {
        // Este metodo pode buscar pelo _id do Mongo ou pelo vvnId, depende da tua estratégia.
        // Aqui assumo que 'id' pode ser o vvnId, já que é o identificador principal do negócio.
        const raw = await this.executionSchema.findOne({ vvnId: id });

        if (!raw) return null;

        return VesselVisitExecutionMap.toDomain(raw);
    }

    // US 4.1.10 - Pesquisa com filtros (date range, vessel, status)
    public async findByFilters(filters: {
        startDate?: Date;
        endDate?: Date;
        vesselId?: string;
        status?: string;
    }): Promise<VesselVisitExecution[]> {
        
        try {
            const query: any = {};

            // Filtro por intervalo de datas (arrivalDate)
            if (filters.startDate || filters.endDate) {
                query.arrivalDate = {};
                if (filters.startDate) {
                    query.arrivalDate.$gte = filters.startDate;
                }
                if (filters.endDate) {
                    const endOfDay = new Date(filters.endDate);
                    endOfDay.setHours(23, 59, 59, 999);
                    query.arrivalDate.$lte = endOfDay;
                }
            }

            // Filtro por status
            if (filters.status) {
                query.status = filters.status;
            }

            const rawVVEs = await this.executionSchema.find(query).sort({ arrivalDate: -1 });

            // Mapear todos para domínio
            return rawVVEs.map((raw: any) => VesselVisitExecutionMap.toDomain(raw));
            
        } catch (err) {
            // Fallback para MOCK se não houver BD
            console.warn('⚠️ [VVERepo] MongoDB not available, using MOCK data');
            
            let filtered = this.mockVVEs;
            
            // Aplicar filtros aos mocks
            if (filters.startDate || filters.endDate) {
                filtered = filtered.filter(vve => {
                    if (!vve.arrivalDate) return false;
                    const arrivalDate = new Date(vve.arrivalDate);
                    
                    if (filters.startDate && arrivalDate < filters.startDate) return false;
                    if (filters.endDate) {
                        const endOfDay = new Date(filters.endDate);
                        endOfDay.setHours(23, 59, 59, 999);
                        if (arrivalDate > endOfDay) return false;
                    }
                    return true;
                });
            }
            
            if (filters.status) {
                filtered = filtered.filter(vve => vve.status === filters.status);
            }
            
            // Converter mocks para domínio
            return filtered.map(mockVVE => {
                const vveResult = VesselVisitExecution.create({
                    vvnId: mockVVE.vvnId,
                    arrivalDate: mockVVE.arrivalDate
                }).getValue();
                
                // Adicionar dados via props (hack temporário para testes sem BD)
                (vveResult as any).props.status = mockVVE.status;
                (vveResult as any).props.departureDate = mockVVE.departureDate;
                (vveResult as any).props.executedOperations = mockVVE.executedOperations.map((op: any) => 
                    ExecutedOperation.create({
                        operationId: OperationId.create(op.operationId),
                        realTimeWindow: TimeRange.create(
                            op.realStartTime,
                            op.realEndTime
                        ),
                        realResource: op.realResource,
                        status: op.status === 'COMPLETED' ? OperationStatus.COMPLETED : OperationStatus.DELAYED,
                        completedBy: op.completedBy
                    })
                );
                
                return vveResult;
            });
        }
    }
}
