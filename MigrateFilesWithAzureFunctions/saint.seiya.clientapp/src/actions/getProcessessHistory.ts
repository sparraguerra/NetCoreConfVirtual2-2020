import { ProcessData, HistoryResponse } from '../model';
import { actionsDef } from '../actions/actionsDef';
import { processesAPI } from '../api/processes';

export const fetchProcessesHistoryAction = (query: string) => (dispatch: any) => {
    dispatch(loading(true));
    processesAPI.getProcessesHistory(query)
        .then((history) => {
            dispatch(fetchProcessesHistoryCompleted(history));
        })
        .catch((error) => {
        }).finally(() => {
            dispatch(loading(false));
        });
};

export const fetchProcessesHistoryCompleted = (history: HistoryResponse) => ({
    type: actionsDef.history.FETCH_HISTORY,
    payload: history,
    meta: {
        httpEnd: true
    }
});

export const loading = (loading: boolean) => ({
    type: actionsDef.history.LOADING,
    payload: loading,
    meta: {
        httpEnd: true
    }
});