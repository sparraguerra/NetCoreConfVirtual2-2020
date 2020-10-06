import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { BrowserRouter } from 'react-router-dom';
import { Provider } from 'react-redux';
import * as serviceWorker from './serviceWorker';
import * as RoutesModule from './routes';
import { store } from './store';
import TelemetryProvider from './services/telemetryProvider';
import { getAppInsights } from './services/telemetryService';
import { initializeIcons } from '@uifabric/icons';
import { authProvider } from './security/authProvider';
import AzureAD from 'react-aad-msal';
import './services/i18n';
import '../node_modules/office-ui-fabric-core/dist/css/fabric.min.css';
import './scss/fabric.min.css'; 
import './scss/toastr.min.scss';

const routes = RoutesModule.routes;
let appInsights = null;
const instrumentationKey = process.env.REACT_APP_APPINSIGHTS_KEY;

function renderApp() {
  initializeIcons(undefined, { disableWarnings: true });

  ReactDOM.render(
    <AppContainer>
      <Provider store={store}>
        <BrowserRouter basename={process.env.PUBLIC_URL}>
          <TelemetryProvider
            instrumentationKey={instrumentationKey}
            after={() => {
              appInsights = getAppInsights();
            }}
          >
            <AzureAD
              provider={authProvider}
              forceLogin={true}
              reduxStore={store}
            >
              {routes}
            </AzureAD>
          </TelemetryProvider>
        </BrowserRouter>
      </Provider>
    </AppContainer>,
    document.getElementById('root')
  );
}

renderApp();

serviceWorker.unregister();
