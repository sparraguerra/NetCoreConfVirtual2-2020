import { ProcessLaunchData, ProcessLaunchRequest } from '../model';
import { actionsDef } from '../actions/actionsDef';
import { processesAPI } from '../api/processes';
import { loading } from './getProcessess';
import * as toastr from 'toastr';
import { toastrOptions } from '../common/utils'

export const launchProcessAction = (request: ProcessLaunchRequest, t: any) => (dispatch: any) => {
    dispatch(loading(true));
    processesAPI.launchProcess(request)
        .then((response) => {
            dispatch(launchProcessCompleted(response));
            toastr.success(t("PROCESSES.LAUNCH_SUCCESS"), '', toastrOptions);
        }).catch((error) => {
            toastr.error(error ? error : t("PROCESSES.LAUNCH_ERROR"), '', toastrOptions);
        }).finally(() => {
            dispatch(loading(false));
        });
};

export const launchProcessCompleted = (response: boolean) => ({
    type: actionsDef.processes.LAUNCH_PROCESS,
    payload: response,
    meta: {
        httpEnd: true
    }
});