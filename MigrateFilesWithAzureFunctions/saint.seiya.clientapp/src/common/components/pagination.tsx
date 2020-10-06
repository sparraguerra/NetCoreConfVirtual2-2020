import * as React from "react";
import Pagination from "react-js-pagination";
import { PaginationContainer } from "./pagination.styled";
import { Icon } from 'office-ui-fabric-react/lib/Icon';


interface IPagingProps {
    totalItems: number;
    itemsCountPerPage: number;
    onPageUpdate: (pageNumber: number) => void;
    currentPage: number;
}

export default class Paging extends React.Component<IPagingProps, null> {

    constructor(props: IPagingProps) {
        super(props);
    }

    public render(): React.ReactElement<IPagingProps> {

        return (
            <PaginationContainer>
                <Pagination
                    activePage={this.props.currentPage}
                    prevPageText={
                        <Icon
                            iconName='ChevronLeft'
                            aria-hidden='true'
                            title='Previous'
                            className='pagination_icon'
                        />
                    }
                    nextPageText={
                        <Icon
                            iconName='ChevronRight'
                            aria-hidden='true'
                            title='Previous'
                            className='pagination_icon'
                        />
                    }
                    activeLinkClass={'active'}
                    itemsCountPerPage={this.props.itemsCountPerPage}
                    totalItemsCount={this.props.totalItems}
                    pageRangeDisplayed={3}
                    onChange={this.props.onPageUpdate}
                    hideFirstLastPages={true}
                    disabledClass={'disabled'}
                />
            </PaginationContainer>
        );
    }
}
