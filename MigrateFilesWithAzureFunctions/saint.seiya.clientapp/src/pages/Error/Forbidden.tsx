import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import styled from 'styled-components';
import { DefaultButton } from 'office-ui-fabric-react';
import forbiddenImage from '../../images/403.svg';

const AccessDeniedContainer = styled.div`
    .errorHeroImage {
        @media (max-width: 960px){
            display: none;
        }
    }
`

export class ForbiddenComponent extends React.Component<RouteComponentProps<{}>, {}> {
    constructor(props: any) {
        super(props);

        this.goBack = this.goBack.bind(this);
    }

    render() {
        return <AccessDeniedContainer>
            <div className="error-container" >
                <div className="error-content" >
                    <div className="error-code" >
                        <h2>403</h2>
                    </div>
                    <div className="error-title" >
                        <h3>Access denied</h3>
                    </div>
                    <div>
                        <span className="errorMessage">
                            You can't use your superpower here!
                        </span>
                    </div>
                    <div>
                        <DefaultButton
                            onClick={this.goBack}
                            className="backButton"
                            iconProps={{ iconName: 'Reply' }}
                            text='Go back'
                        />
                    </div>
                </div>
                <div className="errorHeroImage">
                    <img src={forbiddenImage} alt="Forbidden" />
                </div>
            </div>
        </AccessDeniedContainer>
    }

    private goBack() {
        this.props.history.goBack();
    }
}