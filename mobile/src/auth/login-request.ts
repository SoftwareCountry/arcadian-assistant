import { OauthError } from './oauth-error';
import { BrowserLogin } from './browser-login';
import uuid from 'uuid/v1';
import { AccessCodeRequest } from './access-code-request';

interface AuthorizationCodeRequestParams {
    clientId: string;
    responseType: 'code';
    redirectUri: 'urn:ietf:wg:oauth:2.0:oob' | string;
    responseMode: 'query' | 'form_post';

    //randomly generated value to prevent CSRF
    state: string;
    resource?: string; //app id uri of the web app
    prompt?: 'login' | 'consent' | 'admin_consent';

    //prefill username / login
    login_hint?: string;
    domain_hint?: string;
}

export interface RefreshToken {
    value: string;
}

export class LoginRequest {

    private readonly csrfSecret = uuid();

    constructor(
        private readonly clientId: string,
        private readonly redirectUri: string,
        private readonly authorizationUrl: string,
        private readonly tokenUrl: string
        ) {
    }

    public async getRefreshTokenFromLoginPage(forceLogin = false): Promise<RefreshToken> {
        let params: AuthorizationCodeRequestParams = {
            clientId: this.clientId,
            responseType: 'code',
            redirectUri: this.redirectUri,
            responseMode: 'query',
            //resource: clientId,
            state: this.csrfSecret
        };

        if (forceLogin) {
            params = { ...params, prompt: 'login' };
        }

        const url = this.getAuthorizationUrl(this.authorizationUrl, params);

        const response = await BrowserLogin.getAuthorizationCode(url, this.redirectUri);

        const refreshCode = await this.getRefreshTokenFromResponse(response);
        return refreshCode;
    }

    private async getRefreshTokenFromResponse(responseUrl: string): Promise<RefreshToken> {
        const match = responseUrl.match(/.*\?(.*)/);
        if (!match) {
            throw new OauthError('Auth Error: provided url is in wrong format');
        }

        const urlSearchParams = new URLSearchParams(match[1]);
        const secret = urlSearchParams.get('state');
        const code = urlSearchParams.get('code');

        if (secret !== this.csrfSecret) {
            throw new OauthError('Auth Error: csrf code is different');
        }

        if (!code) {
            throw new OauthError('Auth Error: no auth code is provided');
        }

        const refreshToken = await this.accessCodeToRefreshToken(code);

        return { value: refreshToken.refreshToken };
    }

    private accessCodeToRefreshToken(accessCode: string) {
        const accessCodeRequest = new AccessCodeRequest(this.clientId, this.redirectUri, this.tokenUrl);
        return accessCodeRequest.fetchNew(accessCode).toPromise();
    }

    private getAuthorizationUrl(authUrl: string, params: AuthorizationCodeRequestParams) {

        const searchParams = new URLSearchParams();

        function addOptionalParam(name: string, value: string | undefined) {
            if (value) {
                searchParams.append(name, value);
            }
        }

        searchParams.append('client_id', params.clientId);
        searchParams.append('response_type', params.responseType);
        searchParams.append('redirect_uri', params.redirectUri);
        searchParams.append('response_mode', params.responseMode);
        searchParams.append('state', params.state);

        addOptionalParam('resource', params.resource);
        addOptionalParam('prompt', params.prompt);
        addOptionalParam('login_hint', params.login_hint);
        addOptionalParam('domain_hint', params.domain_hint);

        return `${authUrl}?${searchParams.toString()}`;
    }
}
