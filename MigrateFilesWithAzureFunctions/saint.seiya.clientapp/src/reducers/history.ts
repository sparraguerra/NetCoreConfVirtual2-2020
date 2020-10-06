import { actionsDef } from '../actions/actionsDef';
import { ProcessData, HistoryResponse } from '../model';

export interface IHistoryState {
    processes: ProcessData[];
    requestLoading: boolean;
    processesCount: number;
    processDetails: string[];
}

const defaultHistoryState = (): IHistoryState => {
    return {
        processes: [],
        requestLoading: false,
        processesCount: 0,
        processDetails: []
    }
}

export const HistoryReducer = (state: IHistoryState = defaultHistoryState(), action): IHistoryState => {
    switch (action.type) {
        case actionsDef.history.FETCH_HISTORY:
            return handleFetchHistoryCompleted(state, action.payload);
        case actionsDef.history.LOADING:
            return handleLoading(state, action.payload);
        default:
            return state;
    }
};

const handleFetchHistoryCompleted = (state: IHistoryState, payload: HistoryResponse): IHistoryState => {
    return {
        ...state,
        processes: payload.processList,
        processesCount: payload.totalCount
    };
};

const handleLoading = (state: IHistoryState, payload: boolean): IHistoryState => {
    return {
        ...state,
        requestLoading: payload
    };
};

const handleFetchProcessDetailsCompleted = (state: IHistoryState, payload: string[]): IHistoryState => {
    return {
        ...state,
        processDetails: payload
    };
};
