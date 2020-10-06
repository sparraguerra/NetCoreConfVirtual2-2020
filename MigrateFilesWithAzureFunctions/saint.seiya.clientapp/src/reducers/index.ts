import { combineReducers } from 'redux';
import { httpReducer, HttpState } from './http';
import { AuthState, authReducer } from './auth';
import { ErrorReducer } from './error';
import { IError, ProcessData, ProcessLaunchData } from '../model';
import { i18nReducer } from './i18n';
import { HistoryReducer, IHistoryState } from './history';
import { ProcessesReducer, IProcessesState } from './processes';
import { IProcessDetailsState, ProcessDetailsReducer } from './processDetails';


export interface StateReducer {
    http: HttpState;
    auth: AuthState;
    error: IError;
    currentLanguage: string;
    history: IHistoryState;
    processes: IProcessesState;
    processDetails: IProcessDetailsState
}

export const state = combineReducers<StateReducer>({
    http: httpReducer,
    auth: authReducer,
    error: ErrorReducer,
    currentLanguage: i18nReducer,
    history: HistoryReducer,
    processes: ProcessesReducer,
    processDetails: ProcessDetailsReducer
});
