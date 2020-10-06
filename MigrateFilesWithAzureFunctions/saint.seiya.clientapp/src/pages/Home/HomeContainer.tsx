import { connect } from 'react-redux';
import { StateReducer } from '../../reducers';
import { HomePage } from './home';
import { fetchProcessesAction } from '../../actions/getProcessess';
import { launchProcessAction } from '../../actions/launchProcess';
import { ProcessLaunchRequest } from '../../model';


const mapStateToProps = (state: StateReducer) => {
    return {
        processes: state.processes.processes,
        requestLoading: state.processes.requestLoading,
        account: state.auth.account
    }
};

const mapDispatchToProps = (dispatch: any) => {
    return {
        getProcesses: () => {
            return dispatch(fetchProcessesAction())
        },
        launchProcess: (request: ProcessLaunchRequest, t: any) => {
            return dispatch(launchProcessAction(request, t))
        }
    }
};

export const HomeContainer: any = connect(mapStateToProps, mapDispatchToProps)(HomePage);
