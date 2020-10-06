import * as React from 'react';
import i18next from 'i18next';
import { BodyContainer, ProcessTile, ShimmerTile } from './processes.styled';
import { ProcessLaunchData, ProcessLaunchRequest } from '../../model';
import { HeaderContainer, MainContainer } from '../processesHistory/processesHistory.styled';
import { Account } from 'msal';
import { Shimmer } from 'office-ui-fabric-react/lib/components/Shimmer/Shimmer';
import { ShimmerElementsGroup } from 'office-ui-fabric-react/lib/components/Shimmer/ShimmerElementsGroup/ShimmerElementsGroup';
import { ShimmerElementType } from 'office-ui-fabric-react/lib/components/Shimmer/Shimmer.types';
import { Dialog } from 'office-ui-fabric-react/lib/components/Dialog/Dialog';
import { DialogType } from 'office-ui-fabric-react/lib/components/Dialog/DialogContent.types';
import { DialogFooter } from 'office-ui-fabric-react/lib/components/Dialog/DialogFooter';
import { PrimaryButton } from 'office-ui-fabric-react/lib/components/Button/PrimaryButton/PrimaryButton';
import { DefaultButton } from 'office-ui-fabric-react/lib/components/Button/DefaultButton/DefaultButton';
import { MessageBar, MessageBarType } from 'office-ui-fabric-react/lib/MessageBar';


const getCustomShimmerElements = () => {
    return (
        <div style={{ display: 'flex' }}>
            <ShimmerElementsGroup
                flexWrap={true}
                width='100%'
                backgroundColor='#FBFBFD'
                shimmerElements={[
                    { type: ShimmerElementType.line, width: '90%', height: 10 },
                    { type: ShimmerElementType.gap, width: '10%', height: 10 },
                    { type: ShimmerElementType.gap, width: '100%', height: 10 },
                    { type: ShimmerElementType.line, width: '80%', height: 10 },
                    { type: ShimmerElementType.gap, width: '20%', height: 10 },
                    { type: ShimmerElementType.gap, width: '100%', height: 10 },
                    { type: ShimmerElementType.line, width: '30%', height: 10 },
                    { type: ShimmerElementType.gap, width: '70%', height: 10 },
                ]}
            />
        </div>
    );
};

interface Props {
    t: any;
    processes: ProcessLaunchData[];
    getProcesses: () => Promise<void>;
    account: Account;
    requestLoading: boolean;
    launchProcess: (request: ProcessLaunchRequest, t: any) => Promise<void>;
}

interface State {
    dialog: any;
    isFirstLoad: boolean;
}

export class ProcessesList extends React.Component<Props, State> {
    constructor(props: any) {
        super(props);

        this.state = {
            dialog: {
                show: false,
                text: '',
                onAccept: null
            },
            isFirstLoad: true
        }
    }

    public componentDidMount = () => {
        this.props.getProcesses();

        this.setState({ isFirstLoad: false });
    }

    private onClickProcess = (process: ProcessLaunchData) => {
        let request: ProcessLaunchRequest = {
            processType: process.processType,
            user: this.props.account.userName
        }
        this.setState({
            dialog: {
                text: this.props.t('PROCESSES.DIALOG').replace('*', process.name),
                onAccept: () => {
                    this.props.launchProcess(request, this.props.t);
                    this.setState({ dialog: { ...this.state.dialog, show: false } });
                },
                show: true
            }
        })
    }

    private getShimmer = (): JSX.Element[] => {
        let shimmer: JSX.Element[] = [];

        for (let index = 0; index < 6; index++) {
            shimmer.push(
                <ShimmerTile key={index}>
                    <Shimmer customElementsGroup={getCustomShimmerElements()} width='100%' />
                </ShimmerTile>
            );
        }

        return shimmer;
    }
    public render() {
        return (
            <MainContainer>
                <Dialog
                    hidden={!this.state.dialog.show}
                    onDismiss={() => this.setState({ dialog: { ...this.state.dialog, show: false } })}
                    dialogContentProps={{
                        type: DialogType.normal,
                        title: this.state.dialog ? this.state.dialog.text : '',
                        className: 'dialog'
                    }}
                    modalProps={{
                        styles: { main: { maxWidth: 450 } },
                    }}
                >
                    <br />
                    <DialogFooter>
                        <PrimaryButton
                            onClick={this.state.dialog ? () => this.state.dialog.onAccept() : null}
                            text={this.props.t('BUTTONS.ACCEPT')}
                        />
                        <DefaultButton onClick={() => this.setState({ dialog: { ...this.state.dialog, show: false } })} text={this.props.t('BUTTONS.CANCEL')} />
                    </DialogFooter>
                </Dialog>
                <HeaderContainer>
                    {this.props.t('PROCESSES.TITLE')}
                </HeaderContainer>
                <BodyContainer>
                    {!this.state.isFirstLoad && !this.props.requestLoading && this.props.processes && this.props.processes.length == 0 ?
                        <MessageBar messageBarType={MessageBarType.info} isMultiline={false} dismissButtonAriaLabel={'Close'} className='processesMessageBar'>
                            {this.props.t('HISTORY.NO_ELEMENTS')}
                        </MessageBar> :
                        this.props.requestLoading ?
                            this.getShimmer().map(item => item) :
                            this.props.processes.map((process, index) => {
                                return (
                                    <ProcessTile key={index} onClick={() => this.onClickProcess(process)}>
                                        <span className='text'>{process.name}</span>
                                    </ProcessTile>
                                );
                            })
                    }
                </BodyContainer>
            </MainContainer>
        )
    }
}