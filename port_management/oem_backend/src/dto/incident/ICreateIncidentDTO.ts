export interface ICreateIncidentDTO {
    incidentTypeCode: string; // O user envia o código (ex: "AC-01"), o serviço procura o ID
    description: string;
    severity?: string; // Opcional (se não enviar, usa a do Tipo)
    affectedVvnIds?: string[]; // Opcional (pode afetar 0 ou N)
    createdBy: string;
}