import styled, { css } from 'styled-components';

export const PaginationContainer = styled.div`
    display: flex;
    justify-content: center;

    .pagination {
        margin-top: 40px;
        display: flex;
        justify-content: space-between;
        padding-inline-start: 0px;

        .hidden {
            display: none;
        }

        &_icon {
            widt: auto;
        }

        li {
            list-style: none;
            margin-right: 8px;

            a {
                font-family: 'Segoe UI', sans-serif;
                background-color: #FFFFFF;
                border: 1px solid #EEEEEE;
                border-radius: 4px;
                width: 48px;
                height: 48px;
                float: left;
                color: #B8B8B8;
                text-decoration: none;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: all .2s;
                font-size: 15px;

                @media screen and ( max-width: 479px ) {
                    width: 40px;
                    height: 40px;
                }

                &:visited {
                    color: inherit;
                }

                &:hover:not(.active) {
                    background-color: #F8F8F8;
                }
            }
        }

        li.active {
            &:hover {
                cursor: default;
            }
        }

        a.active {
            font-weight: bold;
            background-color: #ffcb00;
            color: black;

            &:hover {
                cursor: default;
            }
        }

        li.disabled {
            a:hover {
                cursor: default;
                background-color: #FFFFFF;
            }
        }
    }
`