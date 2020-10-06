import { connect } from 'react-redux';
import { StateReducer } from '../../reducers';
import { fetchProcessDetailsAction } from '../../actions/getProcessDetails'
import { ProcessDetails } from './processDetails'
import { ProcessData } from '../../model'

const mapStateToProps = (state: StateReducer) => {
    return {
        processDetails: state.processDetails.processDetails,
        requestLoading: state.processDetails.requestLoading
    }
};

const mapDispatchToProps = (dispatch: any) => {
    return {
        getProcessDetails: (process: ProcessData) => {
            return dispatch(fetchProcessDetailsAction(process))
        }
    }
};

export const ProcessDetailsContainer: any = connect(mapStateToProps, mapDispatchToProps)(ProcessDetails);
