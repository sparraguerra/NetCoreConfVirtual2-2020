import * as React from 'react';
import { Route, Switch } from 'react-router-dom';
import { Layout } from './components/Layout';
import { routeUrls } from './consts/routes';
import { HomeContainer } from './pages/Home/HomeContainer';
import { NotFoundComponent } from './pages/Error/NotFound';
import { ForbiddenComponent } from './pages/Error/Forbidden';
import { ProcessesHistoryContainer } from './components/processesHistory/processesHistoryContainer';
import { ProcessDetailsContainer } from './components/processDetails/processDetailsContainer'

export const routes =
    <Layout>
        <Switch>
            <Route exact path={routeUrls.home} component={HomeContainer} />
            <Route exact path={routeUrls.processeshistory} component={ProcessesHistoryContainer} />
            <Route exact path={routeUrls.processDetails} component={ProcessDetailsContainer} />
            <Route exact path={routeUrls.forbidden} component={ForbiddenComponent} />
            <Route component={NotFoundComponent} />
        </Switch>
    </Layout>;