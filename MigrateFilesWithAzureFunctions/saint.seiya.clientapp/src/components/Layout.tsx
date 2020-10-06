import * as React from 'react';
import { HeaderContainer } from './Header/HeaderContainer';
import styled from 'styled-components';
import '../css/site.scss';


const MainDiv = styled.div`
    display: flex;
    min-height: 100vh;
    flex-direction: column;
    background-color: #FBFBFD;
    font-family: 'RobotoRegular', 'Segoe UI', sans-serif !important;
    color: #3E3F42 !important;

    .mainDivContent {
        flex: 1;
    }
`

const Body = styled.div`
    width: 80%;
    align-self: center;
`

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps> {
    public render() {
        return (
            <MainDiv>
                <input type="hidden" defaultValue={JSON.stringify(process.env)} />
                <HeaderContainer />
                <Body>
                    {this.props.children}
                </Body>
            </MainDiv>
        );
    }
}