export interface IPlanningModuleResponseDTO {
    // Campos de topo do JSON do Prolog
    requestId: string;
    success: boolean;
    algorithm: string; // Ex: "IARTI_Genetic_Algorithm_OX1"
    totalDelay: number;

    // A lista de agendamentos
    schedule: Array<{
        vesselId: string;       // Corresponde ao vvnId (Ex: "V001")
        dockId: string;         // Recurso principal (Ex: "D1")

        // MUITO IMPORTANTE: O Prolog devolve MINUTOS (números), não Strings ISO!
        startTime: number;      // Ex: 600 (10:00)
        endTime: number;        // Ex: 720 (12:00)

        assignedCrane: string[]; // Lista de gruas (Ex: ["C1", "C2"])
        delay: number;          // Atraso em minutos
    }>;

    warnings?: string[];
}