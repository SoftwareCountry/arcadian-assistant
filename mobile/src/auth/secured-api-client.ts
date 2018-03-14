import { ajaxGetJSON, ajaxPost, ajaxPut, ajaxDelete } from 'rxjs/observable/dom/AjaxObservable';
import { Observable } from 'rxjs/Observable';
import { AuthenticationState, AuthenticatedState } from './authentication-state';

export class SecuredApiClient {

    constructor(private apiRootUrl: string, private authState: Observable<AuthenticationState>) {
    }

    public getJSON(relativeUrl: string, headers?: Object) {
        return this.getHeaders(headers)
            .flatMap(newHeaders => ajaxGetJSON(this.getFullUrl(relativeUrl), newHeaders));
    }

    public post(relativeUrl: string, body?: any, headers?: Object) {
        return this.getHeaders(headers)
            .flatMap(newHeaders => ajaxPost(this.getFullUrl(relativeUrl), body, newHeaders));
    }

    public put(relativeUrl: string, body?: any, headers?: Object) {
        return this.getHeaders(headers)
            .flatMap(newHeaders => ajaxPut(this.getFullUrl(relativeUrl), body, newHeaders));
    }

    public delete(relativeUrl: string, headers?: Object) {
        return this.getHeaders(headers)
            .flatMap(newHeaders => ajaxDelete(this.getFullUrl(relativeUrl), newHeaders));
    }

    private getFullUrl(relativeUrl: string) {
        return new URL(relativeUrl, this.apiRootUrl).toString();
    }

    private getHeaders(headers?: Object) {
        return this.authState
            .first(x => x.isAuthenticated)
            .map((x: AuthenticatedState) => ({
                ...headers,
                'Authorization': `Bearer ${x.jwtToken}`
            }));
    }
}