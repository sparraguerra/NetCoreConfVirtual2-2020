import { ProcessData, ProcessLaunchData, ProcessLaunchRequest, HistoryResponse, UnlockProcessRequest } from "../../model";
import { ProcessStatusEnum } from "../../model/enums/ProcessStatusEnum";
import { get, post } from "../fetch-wrapper";
import { HttpResponse } from "../http-response";
import { DocumentStatus } from "../../model/DocumentStatus";

const baseURL = process.env.REACT_APP_API + '/api/document';

const mapToProcessData = (process: any): ProcessData => {
    if (!process) {
        return null;
    }

    return {
        date: process.date ? new Date(process.date) : null,
        id: process.id,
        name: process.name,
        status: process.status,
        user: process.user,
        uniqueId: process.uniqueId,
        processType: process.processType
    };
};

const mapToProcessesData = (processes: any[]): ProcessData[] => {
    return processes.map(mapToProcessData);
};

const mapToHistory = (history: any): HistoryResponse => {
    return {
        currentPage: history.currentPage,
        totalCount: history.totalCount,
        processList: history.processList ? history.processList.map(mapToProcessData) : []
    };
};

const getProcessesHistory = async (query: string): Promise<HistoryResponse> => {
    return get<HistoryResponse>(`${baseURL}/traces${query}`).then(async (response) => (await getResponseProcessesHistory(response, mapToHistory)));
};

const getResponseProcessesHistory = async (response: HttpResponse<any>, mapResponse: (history: any) => HistoryResponse) => {
    if (response.status === 200 && response.parsedBody) {
        return mapResponse(await response.parsedBody);
    } else {
        throw new Error();
    }
};

const mapToProcessLaunchData = (process: any): ProcessLaunchData => {
    if (!process) {
        return null;
    }

    return {
        processType: process.id,
        name: process.name
    };
};

const mapToProcesses = (processes: any): ProcessLaunchData[] => {
    return processes.map(mapToProcessLaunchData);
};

const getResponseProcesses = async (response: HttpResponse<any>, mapResponse: (processes: any) => ProcessLaunchData[]) => {
    if (response.status === 200 && response.parsedBody) {
        return mapResponse(await response.parsedBody);
    } else {
        return [];
    }
};

const getProcesses = async (): Promise<ProcessLaunchData[]> => {
    return get<ProcessLaunchData[]>(`${baseURL}/availables`).then(async (response) => (await getResponseProcesses(response, mapToProcesses)));
};

const launchProcess = async (request: ProcessLaunchRequest): Promise<boolean> => {
    return post<boolean>(`${baseURL}/launch`, request).then(async (response) => (await getResponseLaunchProcess(response)));
};

const getResponseLaunchProcess = async (response: HttpResponse<any>) => {
    if (response.status < 203 && response.parsedBody) {
        return await response.parsedBody;
    } else {
        throw new Error(response.parsedBody ? response.parsedBody : '');
    }
};

const unlockProcess = async (request: UnlockProcessRequest): Promise<boolean> => {
    return post<boolean>(`${baseURL}/unlock`, request).then(async (response) => (await getResponseUnlockProcess(response)));
}

const getResponseUnlockProcess = async (response: HttpResponse<any>) => {
    if (response.status < 203 && response.parsedBody) {
        return await response.parsedBody;
    } else {
        throw new Error(response.parsedBody ? response.parsedBody : '');
    }
};

const getProcessDetails = async (process: ProcessData): Promise<DocumentStatus[]> => {
    return post<DocumentStatus[]>(`${baseURL}/detail`, process).then(async (response) => (await getResponseProcessDetails(response)));
}

const getResponseProcessDetails = async (response: HttpResponse<any>) => {
    if (response.status < 203 && response.parsedBody) {
        return await response.parsedBody;
    } else {
        throw new Error(response.parsedBody ? response.parsedBody : '');
    }
}

export const processesAPI = {
    getProcessesHistory,
    getProcesses,
    launchProcess,
    getProcessDetails,
    unlockProcess
};
