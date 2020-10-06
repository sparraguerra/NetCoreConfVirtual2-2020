import { createBrowserHistory } from 'history';

const baseUrl = process.env.PUBLIC_URL!;
export default createBrowserHistory({ basename: baseUrl });