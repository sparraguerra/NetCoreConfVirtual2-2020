This project is the Front-End for Expedientes-Obra from Ferrovial, following the ENCAMINA guide-lines https://dev.azure.com/encamina/ENESFERA.Examples/_wiki/wikis/ENESFERA.Examples.wiki/8/React-Redux-Typescript

## Available Scripts

In the project directory, you can run:

### `npm run local` - `npm run build:development`

Runs the app with local environment variables.<br />
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits.<br />
You will also see any lint errors in the console.

### `npm run build`

Builds the app for production to the `build` folder.<br />
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.<br />

### `npm run build:int` - `npm run build:pre` - `npm run build:pro`

Builds the app for production to the `build` folder.<br />
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.<br />

Also, configure the appropiate environment variables.<br />

## Available Environments

We use env-cmd to configure the variables for each environment. 

In the file .env-cmdrc we define the appopiate environment variables for each deployment:

- LOCAL
- INT
- PRE
- PRO

## Localization and translation

For localize the application, we use react-i18next and i18next. 

We need to use the translation json files from the /assets/translation folder. 

The component must have the following HOC translation component:

`export const HomePage = withTranslation('name of the file namespace')(HomeComponent);`

Then we need to extend the Props of the component to have the translation object 't'

`interface Props extends TransProps { }`

## Application Insights

We implemented a TelemetryProvider component to inject the withAITracking HOC component in all our routes pages

If you want to send specific telemetry messages to Application Insights, you can import the TelemetryService and use the following AI object:

`import { getAppInsights } from '../../services/telemetryService';`<br />
`const ai = getAppInsights();`<br />

`ai.trackTrace({message: 'Starting fetch users'});` <br />
`ai.trackEvent({name: 'some event'});` <br />
`ai.trackException({exception: new Error('some error')});` <br />
`ai.trackMetric({name: 'some metric', average: 42});` <br />
`ai.trackDependencyData({absoluteUrl: 'some url', responseCode: 200, method: 'GET', id: 'some id'});` <br />
`ai.startTrackEvent("event");` <br />
`ai.stopTrackEvent("event", {customProp1: "some value"});` <br />