import styled from 'styled-components';

export const HeaderBar = styled.div`
    background: #fff;
    box-shadow: 0 0 2px 1px rgba(0, 0, 0, 0.22);
    overflow: hidden;
    display: flex;
    min-height: 69px;
    align-items: center;

    & .separator {
        flex: 0 1 1px;
        flex-wrap: nowrap;
        justify-content: space-between;
    }
`

export const HeaderLogo = styled.div`
    height: fit-content;
    margin: 0px 30px;
    @media screen and ( max-width: 479px ) {
        margin: 0px 15px;
    }

    > img {
        display: block;
        width: 160px;

        @media screen and ( max-width: 479px ) {
            width: 100px;
        }
    }
`

export const HeaderMenu = styled.div`
    padding-left: 20px;
    border-left: 1px solid #EAEDF3;
`

export const MenuItem = styled.div`
    display: inline-block;
    height: 40px;
    margin: 5px;

    > a {
        text-decoration: none;
        color: #3E3F42;
        font-family: 'RobotoRegular', 'Segoe UI', sans-serif;
        font-size: 14px;
        height: 100%;
        display: flex;
        align-items: center;
        padding: 0px 20px;
    }

    .active {
        color: #0084cf;
        font-family: 'RobotoMedium', 'Segoe UI', sans-serif;
    }
`