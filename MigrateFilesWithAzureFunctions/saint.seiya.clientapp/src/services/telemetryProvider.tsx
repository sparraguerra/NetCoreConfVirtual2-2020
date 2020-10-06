import React, {Component, Fragment} from 'react';
import {withAITracking} from '@microsoft/applicationinsights-react-js';
import {ai} from './telemetryService';
import {withRouter} from 'react-router-dom';
import history from '../history';

interface Props {
    instrumentationKey: string;
    after(): void;
}

class TelemetryProvider extends Component<Props> {
    state = {
        initialized: false
    };

    componentDidMount() {
        const {initialized} = this.state;
        const AppInsightsInstrumentationKey = this.props.instrumentationKey; 
        if (!Boolean(initialized) && Boolean(AppInsightsInstrumentationKey) && Boolean(history)) {
            ai.initialize(AppInsightsInstrumentationKey, history);
            this.setState({initialized: true});
        }

        this.props.after();
    }

    render() {
        const {children} = this.props;
        return (
            <Fragment>
                {children}
            </Fragment>
        );
    }
}

export default withRouter(withAITracking(ai.reactPlugin, TelemetryProvider));