import { AuthenticationState, AuthenticationActions } from 'react-aad-msal';
import { Account } from 'msal';

export interface AuthState {
  account: Account;
  initializing: boolean;
  initialized: boolean;
  idToken: string;
  accessToken: string;
  state: AuthenticationState;
  isAuthorized: boolean;
}

const initialState: AuthState = {
  account: null,
  initializing: false,
  initialized: false,
  idToken: null,
  accessToken: null,
  state: AuthenticationState.Unauthenticated,
  isAuthorized: null,
};

export const authReducer = (state = initialState, action) => {
  switch (action.type) {
    case AuthenticationActions.Initializing:
      return {
        ...state,
        initializing: true,
        initialized: false,
      };
    case AuthenticationActions.Initialized:
      return {
        ...state,
        initializing: false,
        initialized: true,
      };
    case AuthenticationActions.AcquiredIdTokenSuccess:
      return {
        ...state,
        idToken: action.payload,
      };
    case AuthenticationActions.AcquiredAccessTokenSuccess:
      return {
        ...state,
        accessToken: action.payload,
      };
    case AuthenticationActions.AcquiredAccessTokenError:
      return {
        ...state,
        accessToken: null,
      };
    case AuthenticationActions.LoginSuccess:
      return {
        ...state,
        account: action.payload.account,
      };

    case AuthenticationActions.LoginError:
    case AuthenticationActions.AcquiredIdTokenError:
    case AuthenticationActions.LogoutSuccess:
      return { ...state, idToken: null, accessToken: null, account: null };
    case AuthenticationActions.AuthenticatedStateChanged:
      return {
        ...state,
        state: action.payload,
      };
    default:
      return state;
  }
};
