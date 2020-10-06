import { actionsDef } from "../../actions/actionsDef";

export const redirectToErrorPageAction = () => ({
    type: actionsDef.error.REDIRECT_TO_ERRORPAGE,
});