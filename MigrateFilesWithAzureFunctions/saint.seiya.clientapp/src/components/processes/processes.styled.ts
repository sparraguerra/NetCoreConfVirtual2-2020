import styled, { css } from 'styled-components';

export const BodyContainer = styled.div`
    display: block;
    max-width: 900px;
    margin-left: -20px;

    .processesMessageBar {
        background-color: #FFFFFF;
        height: 50px;
        align-items: center; 
        border: 1px solid #EAEDF3; 

        span {
            font-family: 'RobotoRegular', 'Segoe UI', sans-serif;
            color: #6B6C6F;
        }
    }
`

export const ShimmerTile = styled.div`
    width: 250px;
    height: 100px;
    background-color: transparent;
    margin: 0px 20px 40px;
    display: inline-table;
    transition: all .2s;
`

export const ProcessTile = styled.div`
    width: 250px;
    height: 100px;
    background-color: #0084cf;
    color: #FFFFFF;
    border-radius: 4px;
    display: inline-block;
    margin: 0px 20px 40px;
    display: inline-table;
    align-items: center;
    transition: all .2s;

    &:hover {
        cursor: pointer;
        background-color: #ffcb00;
        color: black;
    }

    // &:nth-child(3n+1) {
    //     margin-left: 0px;
    // }

    // &:nth-child(3n):not(:first-child) {
    //     margin-right: 0px;
    // }

    span {
        font-family: 'RobotoMedium', 'Segoe UI', sans-serif;
        text-transform: uppercase;
        margin: 20px;
        font-size: 14px;
        display: flex;
    }
`