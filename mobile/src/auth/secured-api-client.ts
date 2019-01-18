/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { AuthenticatedState, AuthenticationState } from './authentication-state';
import moment from 'moment';
import { Observable } from 'rxjs';
import { ajax } from 'rxjs/ajax';
import { first, flatMap, map, timeout } from 'rxjs/operators';

//============================================================================
export class SecuredApiClient {
    //----------------------------------------------------------------------------
    constructor(private apiRootUrl: string, private authState: Observable<AuthenticationState>) {
    }

    //----------------------------------------------------------------------------
    public getJSON<T>
    (relativeUrl: string, headers?: Object) {
        return this.getHeaders(headers).pipe(flatMap(newHeaders => ajax.getJSON<T>(this.getFullUrl(relativeUrl), newHeaders)));
    }

    //----------------------------------------------------------------------------
    public post(relativeUrl: string, body?: any, headers?: Object) {
        return this.getHeaders(headers).pipe(flatMap(newHeaders => ajax.post(this.getFullUrl(relativeUrl), body, newHeaders)));
    }

    //----------------------------------------------------------------------------
    public put(relativeUrl: string, body?: any, headers?: Object) {
        return this.getHeaders(headers).pipe(flatMap(newHeaders => ajax.put(this.getFullUrl(relativeUrl), body, newHeaders)));
    }

    //----------------------------------------------------------------------------
    public delete(relativeUrl: string, headers?: Object) {
        return this.getHeaders(headers).pipe(flatMap(newHeaders => ajax.delete(this.getFullUrl(relativeUrl), newHeaders)));
    }

    //----------------------------------------------------------------------------
    private getFullUrl(relativeUrl: string) {
        const url = this.apiRootUrl + relativeUrl;
        return url.replace(/([^:]\/)\/+/g, '$1');
    }

    //----------------------------------------------------------------------------
    private isAuthenticated(state: AuthenticationState): state is AuthenticatedState {
        if (state.isAuthenticated && state.jwtToken) {
            return moment().isBefore(state.validUntil);
        }
        return false;
    }

    //----------------------------------------------------------------------------
    private getHeaders(headers?: Object) {
        return this.authState.pipe(
            first(this.isAuthenticated),
            timeout(30000),
            map(x => ({
                ...headers,
                'Authorization': `Bearer ${x.jwtToken}`
            })),
        );
    }
}
