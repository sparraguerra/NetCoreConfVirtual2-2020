import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import styled from 'styled-components';
import { DefaultButton } from 'office-ui-fabric-react';
import notFoundImage from '../../images/404.svg';

const NotFoundContainer = styled.div`
    .errorHeroImage {
        @media (max-width: 960px){
            display: none;
        }
    }
`

export class NotFoundComponent extends React.Component<RouteComponentProps<{}>, {}> {
    constructor(props: any) {
        super(props);
    }

    render() {
        return <NotFoundContainer>
            <div className="error-container" >
                <div className="error-content" >
                    <div className="error-code" >
                        <h2>404</h2>
                    </div>
                    <div className="error-title" >
                        <h3>Not found</h3>
                    </div>
                    <div>
                        <span className="errorMessage">
                            You were flying so fast that you lost your teammates!
                        </span>
                    </div>
                    <div>
                        <DefaultButton
                            onClick={() => this.goBack()}
                            className="backButton"
                            iconProps={{ iconName: 'Reply' }}
                            text='Go back'
                        />
                    </div>
                </div>
                <div className="errorHeroImage">
                    <img src={notFoundImage} alt="Not found" />
                </div>
            </div>
        </NotFoundContainer>
    }

    private goBack = () => {
        this.props.history.push('/');
    }
}