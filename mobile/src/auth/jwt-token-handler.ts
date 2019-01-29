import { AccessCodeRequest } from './access-code-request';
import moment from 'moment';
import { map } from 'rxjs/operators';
import { RefreshTokenStorage } from './refresh-token-storage';
import { Observable, from, Subject } from 'rxjs';
import { OauthError } from './oauth-error';

export class NoJwtTokenError extends Error {
    constructor() {
        super('No JWT token is set, user is unauthenticated');
    }
}

export class JwtToken {
    private static readonly clockScrew = moment.duration(1, 'minute');

    constructor(
        public readonly validUntil: moment.Moment,
        public readonly value: string
    ) { }

    public get isExpired() {
        return moment().isSameOrAfter(this.validUntil);
    }

    public get isExpiredOrAboutTo() {
        return moment().subtract(JwtToken.clockScrew).isSameOrAfter(this.validUntil);
    }
}

export class JwtTokenHandler {
    private token: Promise<JwtToken> | null = null;

    constructor(
        private readonly accessCodeRequest: AccessCodeRequest,
        private readonly refreshTokenStorage: RefreshTokenStorage
    ) { }

    // convert to observable?
    public async get(): Promise<JwtToken> {
        if (this.token == null) {
            throw new NoJwtTokenError();
        } else {
            const token = await this.token;
            if (!token.isExpiredOrAboutTo) {
                return token;
            }

            this.refresh();
            return this.get(); //refresh and reiterate
        }
    }

    public get$(): Observable<JwtToken> {
        return from<JwtToken>([]);
    }

    public refresh() {
        this.token = new Promise(resolve => resolve(this.getNewToken()));
    }

    public clean() {
        this.token = null;
    }

    private async getNewToken() {
        const oldRefreshToken = await this.refreshTokenStorage.getRefreshToken();
        if (!oldRefreshToken) {
            throw new OauthError('refresh token is empty, it should be set in storage first');
        }

        const accessCodeResponse = await this.accessCodeRequest.refresh(oldRefreshToken)
            .toPromise();

        await this.refreshTokenStorage.storeToken(accessCodeResponse.refreshToken);

        const expirationDate = moment.unix(+accessCodeResponse.expiresOn);
        const jwtToken = new JwtToken(expirationDate, accessCodeResponse.accessToken);

        return jwtToken;
    }
}