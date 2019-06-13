/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ajax } from 'rxjs/ajax';
import { flatMap, map } from 'rxjs/operators';
import { JwtTokenHandler } from './jwt-token-handler';
import { from } from 'rxjs';

//============================================================================
export class SecuredApiClient {
    //----------------------------------------------------------------------------
    constructor(private apiRootUrl: string, private jwtTokenHandler: JwtTokenHandler) {
    }

    //----------------------------------------------------------------------------
    public getJSON<T>(relativeUrl: string, headers?: Object) {
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
    private getHeaders(headers?: Object) {
        const jwtToken = this.jwtTokenHandler.get();
        return from(jwtToken).pipe(
            map(x => ({
                ...headers,
                'Authorization': `Bearer ${x.value}`
            })));
    }
}
