import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { withTranslation, TransProps } from 'react-i18next';
import { ProcessesHistory } from '../../components/processesHistory/processesHistory';
import 'moment/locale/es';
import moment from 'moment';
import { ProcessesList } from '../../components/processes/processes';
import { ProcessData, ProcessLaunchData, ProcessLaunchRequest } from '../../model';
import { Account } from 'msal';

interface Props extends TransProps {
    processes: ProcessLaunchData[]; 
    getProcesses: () => Promise<void>;
    account: Account;
    launchProcess: (request: ProcessLaunchRequest, t: any) => Promise<void>;
    requestLoading: boolean;
}

class HomeComponent extends React.Component<RouteComponentProps<{}> & Props, {}> {
    constructor(props: any) {
        super(props);
    }
    
    public render() {
        moment.locale(this.props.t("LOCALE"));
        
        return (
            <ProcessesList
                processes={this.props.processes}
                t={this.props.t}
                getProcesses={this.props.getProcesses}
                account={this.props.account}
                launchProcess={this.props.launchProcess}
                requestLoading={this.props.requestLoading}
            />
        );
    }
}

export const HomePage = withTranslation('common')(HomeComponent);