import { actionsDef } from '../../actions/actionsDef';
import history from '../../history';
import { routeUrls } from '../../consts';

export const errorMiddleware = (store) => (next) => (action) => {
  if (action.type === actionsDef.error.REDIRECT_TO_ERRORPAGE) {
    history.push(routeUrls.errorPage);
  } else {
    next(action);
  }
};
