import { actionsDef } from '../actions/actionsDef';

export interface HttpState {
  callCount: number;
  inProgress: boolean;
}

const newState = (): HttpState => ({
  callCount: 0,
  inProgress: false,
});

export const httpReducer = (state = newState(), action) => {
  switch (action.type) {
      case actionsDef.http.HTTP_CALL_START:
      return handleHttpCallStart(state, action.payload);
      case actionsDef.http.HTTP_CALL_END:
      return handleHttpCallEnd(state, action.payload);
  }

  return state;
}

const handleHttpCallStart = (state: HttpState, payload): HttpState => ({
  callCount: 1,
  inProgress: true,
});

const handleHttpCallEnd = (state: HttpState, payload): HttpState => {
  const callCount = state.callCount - 1;

  return {
    callCount,
    inProgress: callCount > 0,
  };
};