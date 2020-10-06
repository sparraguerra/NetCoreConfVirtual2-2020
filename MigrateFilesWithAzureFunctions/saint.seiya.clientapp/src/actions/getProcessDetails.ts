import { processesAPI } from '../api/processes';
import { actionsDef } from '../actions/actionsDef';
import { ProcessData } from '../model';
import { DocumentStatus } from '../model/DocumentStatus';


export const fetchProcessDetailsAction = (process: ProcessData) => (dispatch: any) => {
    dispatch(loading(true));
    processesAPI.getProcessDetails(process)
        .then((processDetails) => {
            dispatch(fetchProcessesCompleted(processDetails));
        }).catch((error) => {
        }).finally(() => {
            dispatch(loading(false));
        });
};

export const fetchProcessesCompleted = (processDetails: DocumentStatus[]) => ({
    type: actionsDef.processDetails.FETCH_PROCESSDETAILS,
    payload: processDetails,
    meta: {
        httpEnd: true
    }
});

export const loading = (loading: boolean) => ({
    type: actionsDef.processDetails.LOADING,
    payload: loading,
    meta: {
        httpEnd: true
    }
});