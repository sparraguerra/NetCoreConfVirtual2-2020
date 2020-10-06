import { ProcessData } from '../model';
import { actionsDef } from '../actions/actionsDef';

export interface IProcessDetailsState {
    processDetails: ProcessData[];
    requestLoading: boolean;
}

const defaultProcessDetailsState = (): IProcessDetailsState => {
    return {
        processDetails: [],
        requestLoading: false,
    }
}

export const ProcessDetailsReducer = (state: IProcessDetailsState = defaultProcessDetailsState(), action): IProcessDetailsState => {
    switch (action.type) {
        case actionsDef.processDetails.FETCH_PROCESSDETAILS:
            return handleFetchProcessDetailsCompleted(state, action.payload);
        case actionsDef.processes.LOADING:
            return handleLoading(state, action.payload);
        default:
            return state;
    }
};

const handleFetchProcessDetailsCompleted = (state: IProcessDetailsState, payload: ProcessData[]): IProcessDetailsState => {
    return {
        ...state,
        processDetails: payload
    };
};

const handleLoading = (state: IProcessDetailsState, payload: boolean): IProcessDetailsState => {
    return {
        ...state,
        requestLoading: payload
    };
};