import './../../css/site.scss';
import * as React from 'react';
import logo from '../../images/logo.png';
import { RouteComponentProps, NavLink } from 'react-router-dom';
// import * as auth from '../../security/adalConfig';
// import { UserComponent } from './user';
import { HeaderBar, HeaderLogo, HeaderMenu, MenuItem } from './Header.styled';
import { routeUrls } from '../../consts/routes';
import { withTranslation, TransProps } from 'react-i18next';

interface Props extends TransProps {
    token: string;
    tenantId: string;
    getToken: () => Promise<string>;
}

class HeaderComponent extends React.Component<Props & RouteComponentProps<{}>> {
    constructor(props) {
        super(props);
    }

    public render() {
        return (
            <HeaderBar>
                <HeaderLogo>
                    <img src={logo} />
                </HeaderLogo>
                <HeaderMenu>
                    <MenuItem>
                        <NavLink exact to={routeUrls.home} data-menuitem-selected={!window.location.pathname.includes(routeUrls.processeshistory)} activeClassName='active'>
                            {this.props.t('HEADER.PROCESSES')}
                        </NavLink>
                    </MenuItem>
                    <MenuItem>
                        <NavLink exact to={routeUrls.processeshistory} data-menuitem-selected={window.location.pathname.includes(routeUrls.processeshistory)} activeClassName='active'>
                            {this.props.t('HEADER.HISTORY')}
                        </NavLink>
                    </MenuItem>
                </HeaderMenu>
            </HeaderBar>
        );
    }
}

export const Header = withTranslation('common')(HeaderComponent);