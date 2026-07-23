import {Service} from "typedi";
import {IPortModuleService, IVVNInfo} from "../services/IServices/IPortModuleService";

@Service()
export default class PortModuleAdapter implements IPortModuleService {
    
    // MOCK DATA - Simula VVNs aprovados do Port Management
    private mockVVNs: IVVNInfo[] = [
        {
            vvnId: "VVN-2026-001",
            vesselName: "Ocean Pioneer",
            vesselId: "VESSEL-001",
            expectedArrival: new Date("2026-01-15T06:00:00Z"),
            expectedDeparture: new Date("2026-01-15T14:00:00Z"),
            assignedDock: "DOCK-A1"
        },
        {
            vvnId: "VVN-2026-002",
            vesselName: "Pacific Explorer",
            vesselId: "VESSEL-002",
            expectedArrival: new Date("2026-01-15T10:00:00Z"),
            expectedDeparture: new Date("2026-01-15T18:00:00Z"),
            assignedDock: "DOCK-B2"
        },
        {
            vvnId: "VVN-2026-003",
            vesselName: "Atlantic Star",
            vesselId: "VESSEL-003",
            expectedArrival: new Date("2026-01-15T14:00:00Z"),
            expectedDeparture: new Date("2026-01-15T22:00:00Z"),
            assignedDock: "DOCK-A2"
        },
        {
            vvnId: "VVN-2026-004",
            vesselName: "Mediterranean Queen",
            vesselId: "VESSEL-004",
            expectedArrival: new Date("2026-01-16T08:00:00Z"),
            expectedDeparture: new Date("2026-01-16T16:00:00Z"),
            assignedDock: "DOCK-C1"
        },
        {
            vvnId: "VVN-2026-005",
            vesselName: "Baltic Voyager",
            vesselId: "VESSEL-005",
            expectedArrival: new Date("2026-01-16T12:00:00Z"),
            expectedDeparture: new Date("2026-01-16T20:00:00Z"),
            assignedDock: "DOCK-B1"
        },
        {
            vvnId: "VVN-2026-006",
            vesselName: "Nordic Spirit",
            vesselId: "VESSEL-006",
            expectedArrival: new Date("2026-01-17T09:00:00Z"),
            expectedDeparture: new Date("2026-01-17T17:00:00Z"),
            assignedDock: "DOCK-A1"
        }
    ];

    public async fetchApprovedVVNs(
        date?: Date, 
        startDate?: Date, 
        endDate?: Date
    ): Promise<IVVNInfo[]> {
        
        console.log('🔧 [MOCK] PortModuleAdapter.fetchApprovedVVNs called');
        console.log('   Date:', date?.toISOString()?.split('T')[0]);
        console.log('   Range:', startDate?.toISOString()?.split('T')[0], '-', endDate?.toISOString()?.split('T')[0]);
        
        // Simula delay de rede
        await this.delay(200);
        
        // Filtrar por data
        let filtered = this.mockVVNs;
        
        if (date) {
            const targetDate = date.toISOString().split('T')[0];
            filtered = this.mockVVNs.filter(vvn => 
                vvn.expectedArrival && vvn.expectedArrival.toISOString().split('T')[0] === targetDate
            );
        } else if (startDate && endDate) {
            filtered = this.mockVVNs.filter(vvn => 
                vvn.expectedArrival && vvn.expectedArrival >= startDate && vvn.expectedArrival <= endDate
            );
        }
        
        console.log(`   ✅ Returning ${filtered.length} mock VVNs`);
        return filtered;
    }

    public async fetchVVNById(vvnId: string): Promise<IVVNInfo | null> {
        console.log('🔧 [MOCK] PortModuleAdapter.fetchVVNById:', vvnId);
        await this.delay(100);
        
        const found = this.mockVVNs.find(v => v.vvnId === vvnId);
        return found || null;
    }
    
    private delay(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}
