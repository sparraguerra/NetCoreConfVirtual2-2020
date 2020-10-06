import { actionsDef } from '../actions/actionsDef';
import { ProcessLaunchData } from '../model';

export interface IProcessesState {
    processes: ProcessLaunchData[];
    requestLoading: boolean;
}

const defaultProcessesState = (): IProcessesState => {
    return {
        processes: [],
        requestLoading: false,
    }
}

export const ProcessesReducer = (state: IProcessesState = defaultProcessesState(), action): IProcessesState => {
    switch (action.type) {
        case actionsDef.processes.FETCH_PROCESSES:
            return handleFetchProcessesCompleted(state, action.payload);
        case actionsDef.processes.LOADING:
            return handleLoading(state, action.payload);
        default:
            return state;
    }
};

const handleFetchProcessesCompleted = (state: IProcessesState, payload: ProcessLaunchData[]): IProcessesState => {
    return {
        ...state,
        processes: payload
    };
};

const handleLoading = (state: IProcessesState, payload: boolean): IProcessesState => {
    return {
        ...state,
        requestLoading: payload
    };
};
