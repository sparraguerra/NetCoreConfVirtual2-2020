import React from 'react';
import { TransProps, withTranslation } from 'react-i18next';
import './processDetails.styled.css';
import { DocumentStatus } from '../../model/DocumentStatus'
import { ProcessData } from '../../model';
import { Table, HeaderContainer } from '../processesHistory/processesHistory.styled';
import { ShimmeredDetailsList } from 'office-ui-fabric-react/lib/ShimmeredDetailsList';
import {
    IDetailsRowProps,
    SelectionMode,
    DetailsListLayoutMode,
    IColumn,
    IDetailsHeaderProps,
    DetailsRow,
    DetailsHeader
} from 'office-ui-fabric-react/lib/DetailsList';

interface Props extends TransProps {
    processDetails: DocumentStatus[];
    getProcessDetails: (process: ProcessData) => Promise<void>;
    requestLoading: boolean
}

interface State {
    columns: IColumn[],
    process: ProcessData,
    processed: number,
    notProcessed: number,
    processDate: string
}

class ProcessDetailsClass extends React.Component<Props, State> {
    constructor(props: any) {
        super(props);

        const columns: IColumn[] = [
            {
                key: 'name',
                name: 'Nombre del documento',
                fieldName: 'name',
                minWidth: 350,
                maxWidth: 500,
                isRowHeader: true,
                isResizable: true,
                data: 'string'
            },
            {
                key: 'message',
                name: 'Mensaje',
                fieldName: 'message',
                minWidth: 950,
                maxWidth: 1100,
                isRowHeader: true,
                isResizable: true,
                data: 'string'
            },
            {
                key: 'status',
                name: this.props.t("HISTORY.COLUMNS.STATUS"),
                fieldName: 'status',
                minWidth: 150,
                maxWidth: 175,
                isRowHeader: true,
                isResizable: true,
                data: 'string'
            },
        ];

        this.state = {
            columns: columns,
            process: JSON.parse(localStorage.getItem('process')),
            processed: null,
            notProcessed: 0,
            processDate: localStorage.getItem('date')
        }
    }

    public componentDidMount = () => {
        this.props.getProcessDetails(this.state.process);
        setTimeout(() => {
            this.calculateProcesses();
        }, 2500);
    }

    public componentWillUnmount = () => {
        localStorage.removeItem('process');
    }

    private formatDate(string) {
        var optionsDate = { year: 'numeric', month: 'long', day: 'numeric' };
        var date = new Date(string).toLocaleDateString([], optionsDate);
        var time = new Date(string).toLocaleTimeString();
        var response = date + " | " + time;
        return response;
    }

    private calculateProcesses() {
        var processed = 0;
        var notProcessed = 0;

        this.props.processDetails.forEach(doc => {
            if (doc.status === 'OK') {
                processed += 1;
            } else {
                notProcessed += 1;
            }
        });

        this.setState({
            processed: processed,
            notProcessed: notProcessed
        })
    }

    private onRenderRow = (props: IDetailsRowProps) => {
        return (
            <DetailsRow
                {...props}
                onRenderItemColumn={(item?: any, index?: number, column?: IColumn) => this.onRenderItemColumn(item, index, column)}
            />
        );
    }

    private onRenderDetailsHeader = (props: IDetailsHeaderProps): JSX.Element => {
        return (
            <div>
                <DetailsHeader {...props} className='detailsHeader' />
            </div>
        );
    }

    private onRenderItemColumn = (item?: any, index?: number, column?: IColumn): any => {
        switch (column.key) {
            case 'name':
                return (
                    <div className='itemColumnTitle'>
                        <div className='docText' title={item[column.key]} style={item.status === 'OK' ? { color: "blue" } : { color: "red" }}>{item[column.key]}</div>
                    </div>
                );
                break;
            case 'message':
                return (
                    <div className='itemColumnMessage'>
                        <div className='messageText' title={item.message} style={item.status === 'OK' ? { color: "blue" } : { color: "red" }}>{item.message}</div>
                    </div>
                );
                break;
            case 'status':
                return (
                    <div className='itemColumnStatus'>
                        <span className='statusText' style={item.status === 'OK' ? { color: "blue" } : { color: "red" }}>{item.status}</span>
                    </div>
                );
                break;
            default:
                return (
                    <div>
                        {item[column.key]}
                    </div>
                );
                break;
        }
    }

    public render() {
        return (
            <div>
                <HeaderContainer>
                    {this.props.t('PROCESSES.DETAILS_TITLE')}
                </HeaderContainer>

                <h3>UniqueId: {this.state.process.uniqueId}</h3>
                <h3>Fecha: {this.formatDate(this.state.processDate)}</h3>
                {this.props.processDetails.length > 0 && this.state.processed != null ?
                    <h4>{this.state.processed} documentos procesados de un total de {this.props.processDetails.length}</h4>
                    :
                    ""
                }

                <Table>
                    <ShimmeredDetailsList
                        items={this.props.processDetails && this.props.processDetails.length > 0 ? this.props.processDetails : []}
                        setKey='set'
                        key='processdetailslist'
                        columns={this.state.columns}
                        selectionMode={SelectionMode.none}
                        onRenderRow={(rowProps: IDetailsRowProps) => this.onRenderRow(rowProps)}
                        onRenderDetailsHeader={(headeProps: IDetailsHeaderProps) => this.onRenderDetailsHeader(headeProps)}
                        layoutMode={DetailsListLayoutMode.justified}
                        enableShimmer={this.props.requestLoading}
                        shimmerLines={10}
                        selectionPreservedOnEmptyClick={true}
                        isHeaderVisible={true}                        
                    />
                </Table>

            </div >
        );
    }
}

export const ProcessDetails = withTranslation('common')(ProcessDetailsClass);
