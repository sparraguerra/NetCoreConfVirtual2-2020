import styled, { css } from 'styled-components';

export const MainContainer = styled.div`
    max-width: 1200px;
`

export const HeaderContainer = styled.div`
    margin: 40px 0px;
    text-transform: uppercase;
    font-family: 'RobotoMedium', 'Segoe UI', sans-serif;
    font-size: 18px;
`

export const BodyContainer = styled.div`
    width: 100%;
    background-color: #FFFFFF;
    border-radius: 3px; 
    border: 1px solid #EAEDF3; 
    height: auto;

    .historyMessageBar {
        background-color: transparent;
        height: 50px;
        align-items: center; 

        span {
            font-family: 'RobotoRegular', 'Segoe UI', sans-serif;
            color: #6B6C6F;
        }
    }
`

export const TableHeader = styled.div` 
    display: flex;
    border-bottom: 1px solid #EAEDF3; 
    flex-wrap: wrap;

    .menuItem {
        padding: 15px 30px;
        margin: 10px;
        background-color: transparent;
        border: none;
        border-radius: 3px;
        transition: all .2s;

        &:hover:not(.active) {
            cursor: pointer;
            background-color: #F8F8F8;
        }

        &:focus {
            border: none;
            outline: none;
        }

        &_text {
            font-size: 16px;
            font-family: 'RobotoRegular', 'Segoe UI', sans-serif;
            color: #6B6C6F;
            background-color: transparent;
            padding-bottom: 23px;
        }

        &_text.active {
            transition: all 0.2s;
            font-family: 'RobotoMedium', 'Segoe UI', sans-serif;
            color: #3E3F42;
        }
    }

    .menuItem.active {
        > .menuItem_text {
            border-bottom: 3px #0084cf solid;
        }
    }
`

export const Table = styled.div`
    .detailsHeader {
        padding: 0px;
        height: 55px;
        > div {
            height: 100%;
            > span {
                display: flex;
                align-items: center;
                > span {
                    width: 100%
                }
            }
        }
        span {
            color: #9EA0A5;
            font-size: 12px;
            font-family: 'RobotoMedium', 'Segoe UI', sans-serif;
            text-transform: uppercase;
        }
    }

    .tableRow {
        .principalText {
            color: #3E3F42;
            font-size: 14px;
            font-family: 'RobotoMedium', 'Segoe UI', sans-serif;
            margin-bottom: 8px;            
        }

        .secondaryText {
            color: #9EA0A5;
            font-size: 12px;
            font-family: 'RobotoRegular', 'Segoe UI', sans-serif;
        }

       .itemColumnStatus {
            height: 100%;
            display: flex;
            flex-direction: row;
            align-items: center;

            .statusText {
                color: #3E3F42;
                font-size: 14px;
                font-family: 'RobotoRegular', 'Segoe UI', sans-serif;
            }
        
           .statusIcon {
               width: 6px;
               height: 6px;
               display: inline-block;
                border-radius: 50%;
                margin-right: 20px;
           }

           .status1 {
            background-color: #3BAD4B;
           }

           .status2 {
            background-color: #E7492E;
           }

           .status3 {
                background-color: #1665D8;
           }

           .status4 {
            background-color: #F6AB2E;
           }

           
       } 
    }

    .withError {
        border-left: 3px #E7492E solid;

        > div {
            margin-left: -3px;
        }
    }
`