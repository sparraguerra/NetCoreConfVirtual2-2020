import { ProcessStatusEnum } from './enums/ProcessStatusEnum';

export interface ProcessData {
    id: number;
    name: string;
    user: string;
    date: Date;
    status: ProcessStatusEnum,
    uniqueId: string,
    processType: string
}