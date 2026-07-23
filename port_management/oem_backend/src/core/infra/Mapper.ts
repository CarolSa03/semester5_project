export abstract class Mapper<T> {
    // Em TypeScript, não podemos definir métodos estáticos abstratos.
    // Mas espera-se que qualquer classe que estenda Mapper implemente:

    // public static toDTO(t: T): DTO;
    // public static toDomain(raw: any): T;
    //public static toPersistence(t: T): any;
}