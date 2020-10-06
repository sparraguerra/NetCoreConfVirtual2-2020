import { ProcessData, ProcessLaunchData } from '../model';
import { actionsDef } from '../actions/actionsDef';
import { processesAPI } from '../api/processes';

export const fetchProcessesAction = () => (dispatch: any) => {
    dispatch(loading(true));
    processesAPI.getProcesses()
        .then((history) => {
            dispatch(fetchProcessesCompleted(history));
        }).catch((error) => {
        }).finally(() => {
            dispatch(loading(false));
        });
};

export const fetchProcessesCompleted = (processes: ProcessLaunchData[]) => ({
    type: actionsDef.processes.FETCH_PROCESSES,
    payload: processes,
    meta: {
        httpEnd: true
    }
});

export const loading = (loading: boolean) => ({
    type: actionsDef.processes.LOADING,
    payload: loading,
    meta: {
        httpEnd: true
    }
});