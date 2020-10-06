import { UnlockProcessRequest } from '../model';
import { processesAPI } from '../api/processes';
import * as toastr from 'toastr';
import { toastrOptions } from '../common/utils';
import { loading } from './getProcessess';
import { actionsDef } from '../actions/actionsDef';

export const unlockProcessAction = (request: UnlockProcessRequest) => (dispatch: any) => {
    dispatch(loading(true));
    processesAPI.unlockProcess(request)
        .then((response) => {
            dispatch(unlockProcessCompleted(response));
            toastr.success('Process has been properly unlocked', '', toastrOptions);
        }).catch((error) => {
            toastr.error(error ? error : toastr("There was an error processing the request"), '', toastrOptions);
        }).finally(() => {
            dispatch(loading(false));
        });
};

export const unlockProcessCompleted = (response: boolean) => ({
    type: actionsDef.processes.UNLOCK_PROCESS,
    payload: response,
    meta: {
        httpEnd: true
    }
});
