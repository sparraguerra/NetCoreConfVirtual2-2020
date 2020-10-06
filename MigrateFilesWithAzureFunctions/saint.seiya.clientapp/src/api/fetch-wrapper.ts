import { HttpResponse } from './http-response';
import * as msal from '../security/authProvider';

export async function get<T>(path: string, args: RequestInit = { method: 'get' }): Promise<HttpResponse<T>> {
    return await http<T>(path, args);
}

export async function post<T>(
    path: string,
    body: any,
    args: RequestInit = { method: 'post', body: JSON.stringify(body) },
): Promise<HttpResponse<T>> {
    return await http<T>(path, args);
}

export async function put<T>(
    path: string,
    body: any,
    args: RequestInit = { method: 'put', body: JSON.stringify(body) },
): Promise<HttpResponse<T>> {
    return await http<T>(path, args);
}

export async function deleteAsync<T>(path: string, args: RequestInit = { method: 'delete' }): Promise<HttpResponse<T>> {
    return await http<T>(path, args);
}

async function http<T>(input: RequestInfo, init?: RequestInit): Promise<HttpResponse<T>> {
    const token = await msal.authProvider.getIdToken();
    const r: RequestInit = init || {};

    if (!r.headers) r.headers = {};

    r.headers = {
        Authorization: `Bearer ${token.idToken.rawIdToken}`,
        Accept: 'application/json',
        'Content-Type': 'application/json' 
    };

    const response: HttpResponse<T> = await fetch(input, init);

    try {
        // may error if there is no body (204 status code)
        response.parsedBody = await response.json();
    } catch (ex) { }

    return response;
}
