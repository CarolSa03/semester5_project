import {v4 as uuid} from 'uuid';
import {Identifier} from './Identifier';

export class UniqueEntityID extends Identifier<string | number> {
    constructor(id?: string | number) {
        // Se não for passado um ID, gera um novo UUID v4
        super(id ? id : uuid());
    }
}