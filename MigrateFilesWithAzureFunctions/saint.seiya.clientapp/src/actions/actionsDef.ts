export const actionsDef = {
    http: {
        HTTP_CALL_START: 'HTTP_CALL_START',
        HTTP_CALL_END: 'HTTP_CALL_END',
    },
    error: {
        REDIRECT_TO_ERRORPAGE: 'REDIRECT_TO_ERRORPAGE',
    },
    i18n: {
        CHANGE_LANGUAGE: 'CHANGE_LANGUAGE',
    },
    history: {
        FETCH_HISTORY: 'FETCH_HISTORY',
        LOADING: 'HISTORY_LOADING'
    },
    processes: {
        FETCH_PROCESSES: 'FETCH_PROCESSES',
        LAUNCH_PROCESS: 'LAUNCH_PROCESS',
        LOADING: 'PROCESSES_LOADING',
        UNLOCK_PROCESS: 'UNLOCK_PROCESS'
    },
    processDetails: {
        FETCH_PROCESSDETAILS: 'FETCH_PROCESSDETAILS',
        LOADING: 'PROCESSDETAILS_LOADING'
    }
};
