import { ProcessStatusEnum } from './enums/ProcessStatusEnum';
import { ProcessData } from './';

export interface HistoryResponse {
    processList: ProcessData[],
    totalCount: number;
    currentPage: number;
}
