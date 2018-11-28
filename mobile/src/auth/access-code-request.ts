import { dataMember, required } from 'santee-dcts';
import { deserialize } from 'santee-dcts/src/deserializer';
import { ajax } from 'rxjs/ajax';
import { map } from 'rxjs/operators';

interface AccessCodeRequestParamsBase {
    'client_id': string;
    'redirect_uri': string;
    'resource': string;
}

interface AccessCodeRequestWithAuthCodeParams extends AccessCodeRequestParamsBase {
    'grant_type': 'authorization_code';
    'code': string;
}

interface AccessCodeRequestWithRefreshTokenParams extends AccessCodeRequestParamsBase {
    'grant_type': 'refresh_token';
    'refresh_token': string;
}

type AccessCodeRequestParams = AccessCodeRequestWithAuthCodeParams | AccessCodeRequestWithRefreshTokenParams;

export class TokenResponse {
    @dataMember({ fieldName: 'access_token' })
    @required()
    public accessToken: string;

    @dataMember({ fieldName: 'token_type' })
    public tokenType: string;

    @dataMember({ fieldName: 'expires_in' })
    public expiresIn: string;

    @dataMember({ fieldName: 'expires_on' })
    public expiresOn: string;

    @dataMember()
    public resource: string;

    @dataMember({ fieldName: 'refresh_token' })
    @required()
    public refreshToken: string;

    @dataMember()
    public scope: string;

    @dataMember({ fieldName: 'id_token' })
    public idToken: string;
}

export class AccessCodeRequest {
    constructor(
        private readonly clientId: string,
        private readonly redirectUri: string,
        private readonly tokenUrl: string
        ) {
    }

    public fetchNew(code: string) {
        const params: AccessCodeRequestParams = { ...this.getDefaultParams(), code, 'grant_type': 'authorization_code' };
        return this.performRequest(params);
    }

    public refresh(refreshToken: string) {
        const params: AccessCodeRequestParams = { ...this.getDefaultParams(), refresh_token: refreshToken, grant_type: 'refresh_token' };
        return this.performRequest(params);
    }

    private performRequest(params: AccessCodeRequestParams) {
        return ajax.post(this.tokenUrl, params).pipe(
            map(x => deserialize(x.response, TokenResponse))
        );
    }

    private getDefaultParams(): AccessCodeRequestParamsBase {
        return {
            'client_id': this.clientId,
            'resource': this.clientId,
            'redirect_uri': this.redirectUri
        };
    }
}
