import { actionsDef } from '../actions/actionsDef';
import { IError } from '../model';

export const ErrorReducer = (state: IError = { message: "", statusCode: 0 }, action) => {
  switch (action.type) {
    case actionsDef.error.REDIRECT_TO_ERRORPAGE:
      return handleRedirectToErrorPage(state, action.payload);
  }

  return state;
};

const handleRedirectToErrorPage = (state: IError, payload: IError) => {
    return payload;
};