import * as React from 'react';
import i18next from 'i18next';
import { MainContainer, HeaderContainer, BodyContainer, TableHeader, Table } from './processesHistory.styled';
import {
    IDetailsRowProps,
    SelectionMode,
    DetailsListLayoutMode,
    IColumn,
    IDetailsHeaderProps,
    DetailsRow,
    DetailsHeader
} from 'office-ui-fabric-react/lib/DetailsList';
import { ShimmeredDetailsList } from 'office-ui-fabric-react/lib/ShimmeredDetailsList';
import { ProcessData, UnlockProcessRequest } from '../../model';
import { utils } from '../../common/utils';
import { ProcessStatusEnum } from '../../model/enums/ProcessStatusEnum';
import moment from 'moment';
import { TransProps, withTranslation } from 'react-i18next';
import Paging from '../../common/components/pagination';
import { MessageBar, MessageBarType } from 'office-ui-fabric-react/lib/MessageBar';
import { PrimaryButton } from 'office-ui-fabric-react/lib/components/Button/PrimaryButton/PrimaryButton';
import { DefaultButton } from 'office-ui-fabric-react/lib/components/Button/DefaultButton/DefaultButton';
import { Dialog } from 'office-ui-fabric-react/lib/components/Dialog/Dialog';
import { DialogType } from 'office-ui-fabric-react/lib/components/Dialog/DialogContent.types';
import { DialogFooter } from 'office-ui-fabric-react/lib/components/Dialog/DialogFooter';
import { DeleteOutlined, InfoCircleOutlined } from '@ant-design/icons';

interface IFilter {
    key: number,
    text: string;
}

interface Props extends TransProps {
    processes: ProcessData[];
    getProcessesHistory: (query: string) => Promise<void>;
    processesCount: number;
    requestLoading: boolean;
    unlockProcess: (request: UnlockProcessRequest) => Promise<void>;
}

interface State {
    tableFilter: IFilter;
    columns: IColumn[];
    currentPage: number;
    orderBy: string;
    descending: boolean;
    isFirstLoad: boolean,
    dialog: any,
    reload: boolean
}

class ProcessesHistoryClass extends React.Component<Props, State> {
    private readonly tableHeaderElements: IFilter[] = [
        { key: 0, text: this.props.t('HISTORY.STATUS.0') },
        { key: ProcessStatusEnum.Completed, text: this.props.t('HISTORY.STATUS.1') },
        { key: ProcessStatusEnum.WithErrors, text: this.props.t('HISTORY.STATUS.2') },
        { key: ProcessStatusEnum.InExecution, text: this.props.t('HISTORY.STATUS.3') },
    ];

    private readonly elementsPerPage = 10;

    constructor(props: any) {
        super(props);

        const columns: IColumn[] = [
            {
                key: 'name',
                name: this.props.t("HISTORY.COLUMNS.PROCESS"),
                fieldName: 'name',
                minWidth: 360,
                maxWidth: 500,
                isRowHeader: true,
                isResizable: true,
                data: 'string',
                onColumnClick: this.onColumnClick
            },
            {
                key: 'date',
                name: this.props.t("HISTORY.COLUMNS.DATE"),
                fieldName: 'date',
                minWidth: 180,
                maxWidth: 300,
                isRowHeader: true,
                isResizable: true,
                data: 'string',
                isSorted: true,
                isSortedDescending: true,
                onColumnClick: this.onColumnClick
            },
            {
                key: 'status',
                name: this.props.t("HISTORY.COLUMNS.STATUS"),
                fieldName: 'status',
                minWidth: 200,
                maxWidth: 400,
                isRowHeader: true,
                isResizable: true,
                data: 'string',
                onColumnClick: this.onColumnClick
            },
            {
                key: 'buttons',
                name: '',
                minWidth: 200,
                maxWidth: 400,
                isRowHeader: true,
                isResizable: true,
            },
        ];

        this.state = {
            tableFilter: { key: 0, text: this.props.t('HISTORY.STATUS.ALL') },
            columns: columns,
            currentPage: 1,
            orderBy: 'date',
            descending: true,
            isFirstLoad: true,
            dialog: {
                show: false,
                text: '',
                onAccept: null
            },
            reload: false
        }
    }

    public componentDidMount = () => {
        this.props.getProcessesHistory(this.buildQuery(1, this.state.orderBy, this.state.descending, this.state.tableFilter.key));

        this.setState({ isFirstLoad: false })
    }

    componentDidUpdate(prevProps, prevState) {
        if (prevState.reload === false && this.state.reload === true) {
            setTimeout(() => {
                this.props.getProcessesHistory(this.buildQuery(1, this.state.orderBy, this.state.descending, this.state.tableFilter.key));
            }, 1500);

            this.setState({
                reload: false
            })
        }
    }

    private onClickFilter = (newFilter: IFilter) => {
        if (newFilter.key != this.state.tableFilter.key) {
            this.props.getProcessesHistory(this.buildQuery(1, this.state.orderBy, this.state.descending, newFilter.key));

            this.setState({
                tableFilter: newFilter,
                currentPage: 1
            });
        }
    }

    private buttonDisabled = (item?: any): boolean => {
        var d = new Date();
        d.setMonth(d.getMonth() - 2)

        if (item.date >= d && item.status !== "InExecution") {
            return false;
        }
        else {
            return true;
        }
    }

    private viewDetails = (item?: any) => {
        localStorage.setItem('process', JSON.stringify(item));
        localStorage.setItem('date', item.date + 'Z');
        window.open('./processdetails', '_blank');
    }

    private unlockProcess = (item: any) => {
        let request: UnlockProcessRequest = {
            id: item.id,
            processType: item.processType
        }

        this.setState({
            dialog: {
                text: "Are you sure you want to unlock this process?",
                onAccept: () => {
                    this.props.unlockProcess(request);
                    this.setState({ dialog: { ...this.state.dialog, show: false }, reload: true });
                },
                show: true
            }
        })
    }

    private onRenderItemColumn = (item?: any, index?: number, column?: IColumn): any => {
        switch (column.key) {
            case 'name':
                return (
                    <div className='itemColumnDate'>
                        <div className='principalText'>{item[column.key]}</div>
                        <div className='secondaryText'>{item.user}</div>
                    </div>
                );
                break;
            case 'date':
                return (
                    <div className='itemColumnDate'>
                        <div className='principalText'>{utils.getFormatedDate(item[column.key])}</div>
                        <div className='secondaryText'>{moment(item.date + 'Z').fromNow()}</div>
                    </div>
                );
                break;
            case 'status':
                return (
                    <div className='itemColumnStatus'>
                        <span className={`statusIcon status${ProcessStatusEnum[item[column.key]]}`}></span>
                        <span className='statusText'>{this.props.t(`HISTORY.STATUS.${[ProcessStatusEnum[item[column.key]]]}`)}</span>
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

    private onRenderRow = (props: IDetailsRowProps) => {
        return (
            <DetailsRow
                {...props}
                className={`tableRow ${props.item['status'] == 'WithErrors' ? 'withError' : ''}`}
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

    private onPageUpdate = (newPage: number) => {
        if (newPage != this.state.currentPage) {
            this.props.getProcessesHistory(this.buildQuery(newPage, this.state.orderBy, this.state.descending, this.state.tableFilter.key));

            this.setState({ currentPage: newPage });
        }
    }

    private buildQuery = (newPage: number, orderBy: string, descending: boolean, filter?: number): string => {
        let query = `?elementsPerPage=${this.elementsPerPage}&page=${newPage}`;

        if (filter) {
            query = query + `&filter=${filter}`
        }

        if (orderBy) {
            query = query + `&orderBy=${orderBy}`;
            if (descending) {
                query = query + `&descending=${descending}`;
            }
        }

        return query;
    }

    private onColumnClick = (ev: React.MouseEvent<HTMLElement>, column: IColumn): void => {
        const { columns } = this.state;
        const newColumns: IColumn[] = columns.slice();
        const currColumn: IColumn = newColumns.filter(currCol => column.key === currCol.key)[0];
        newColumns.forEach((newCol: IColumn) => {
            if (newCol === currColumn) {
                currColumn.isSortedDescending = !currColumn.isSortedDescending;
                currColumn.isSorted = true;
            } else {
                newCol.isSorted = false;
                newCol.isSortedDescending = true;
            }
        });

        this.props.getProcessesHistory(this.buildQuery(1, currColumn.key, currColumn.isSortedDescending ? true : false, this.state.tableFilter.key));

        this.setState({
            columns: newColumns,
            orderBy: currColumn.key,
            descending: currColumn.isSortedDescending ? true : false,
            currentPage: 1
        });
    };



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
                    {this.props.t('HISTORY.TITLE')}
                </HeaderContainer>
                <BodyContainer>
                    <TableHeader>
                        {this.tableHeaderElements.map((element, index) => {
                            let isActive = this.state.tableFilter && this.state.tableFilter.key == element.key ? 'active' : '';
                            return (
                                <button key={index} className={`menuItem ${isActive}`} onClick={() => this.onClickFilter(element)}>
                                    <span className={`menuItem_text ${isActive}`}>
                                        {element.text}
                                    </span>
                                </button>
                            )
                        })}
                    </TableHeader>
                    {!this.state.isFirstLoad && !this.props.requestLoading && this.props.processesCount == 0 ?
                        <MessageBar messageBarType={MessageBarType.info} isMultiline={false} dismissButtonAriaLabel={'Close'} className='historyMessageBar'>
                            {this.props.t('HISTORY.NO_ELEMENTS')}
                        </MessageBar> :
                        <Table>
                            <ShimmeredDetailsList
                                items={this.props.processes && this.props.processes.length > 0 ? this.props.processes : []}
                                setKey='set'
                                key='detailslist'
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
                    }
                </BodyContainer>
                {this.props.processesCount > this.elementsPerPage && !this.props.requestLoading ?
                    <Paging
                        totalItems={this.props.processesCount}
                        itemsCountPerPage={this.elementsPerPage}
                        onPageUpdate={this.onPageUpdate}
                        currentPage={this.state.currentPage}
                    /> : null
                }
            </MainContainer>
        )
    }
}

export const ProcessesHistory = withTranslation('common')(ProcessesHistoryClass);
