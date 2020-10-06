import { connect } from 'react-redux';
import { StateReducer } from '../../reducers';
import { fetchProcessesHistoryAction } from '../../actions/getProcessessHistory'
import { ProcessesHistory } from './processesHistory';
import { UnlockProcessRequest } from '../../model';
import { unlockProcessAction } from '../../actions/unlockProcess';

const mapStateToProps = (state: StateReducer) => {
    return {
        processes: state.history.processes,
        requestLoading: state.history.requestLoading,
        processesCount: state.history.processesCount
    }
};

const mapDispatchToProps = (dispatch: any) => {
    return {
        getProcessesHistory: (query: string) => {
            return dispatch(fetchProcessesHistoryAction(query))
        },
        unlockProcess: (request: UnlockProcessRequest) => {
            return dispatch(unlockProcessAction(request))
        }
    }
};

export const ProcessesHistoryContainer: any = connect(mapStateToProps, mapDispatchToProps)(ProcessesHistory);