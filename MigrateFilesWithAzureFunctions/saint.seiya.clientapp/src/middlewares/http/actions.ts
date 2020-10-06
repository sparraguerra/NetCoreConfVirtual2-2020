import { actionsDef } from '../../actions/actionsDef';

export const httpCallStartAction = () => ({
    type: actionsDef.http.HTTP_CALL_START,
});

export const httpCallEndAction = () => ({
    type: actionsDef.http.HTTP_CALL_END,
});