import { MsalAuthProvider, LoginType } from 'react-aad-msal';

const cacheLocation: 'sessionStorage' = 'sessionStorage';

const config = {
	auth: {
		clientId: process.env.REACT_APP_CLIENTID,
		authority: `https://login.microsoftonline.com/${process.env.REACT_APP_TENANTID}`,
		redirectUri: process.env.REACT_APP_REDIRECT_URI,
	},
	cache: {
		cacheLocation: cacheLocation,
		storeAuthStateInCookie: true,
	},
};

const authenticationParameters = {
	scopes: ['user.read', `api://${process.env.REACT_APP_API_CLIENTID}/SPA`],
};

const options = {
	loginType: LoginType.Redirect,
	tokenRefreshUri: window.location.origin,
};

export async function msalFetch(fetch, url, options?) {
	const token = await authProvider.getAccessToken();
	const o = options || {};
	if (!o.headers) o.headers = {};
	o.headers.Authorization = `Bearer ${token.accessToken}`;
	return fetch(url, o);
}

export async function msalGraphFetch(fetch, url, options?) {
	const token = await authProvider.getAccessToken({ scopes: ['user.read'] });
	const o = options || {};
	if (!o.headers) o.headers = {};
	o.headers.Authorization = `Bearer ${token.accessToken}`;
	return fetch(url, o);
}

export const authProvider = new MsalAuthProvider(config, authenticationParameters, options);
