import { AccessCodeRequest } from './access-code-request';
import moment from 'moment';
import { map } from 'rxjs/operators';
import { RefreshTokenStorage } from './refresh-token-storage';
import { Observable, from, Subject, BehaviorSubject } from 'rxjs';
import { OauthError } from './oauth-error';
import { RefreshToken } from './login-request';

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
    private tokenSource = new BehaviorSubject<JwtToken | null>(null);

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

            await this.refresh();
            return this.get(); 
        }
    }

    public get$(): Observable<JwtToken | null> {
        return this.tokenSource.asObservable();
    }

    public async refresh() {
       return this.reset(await this.getExistingToken());
    }

    public reset(refreshToken: RefreshToken) {
        this.token = (async () => {
            await this.refreshTokenStorage.storeToken(refreshToken.value);
            return this.getNewToken();
        })();

        this.notifyAboutNewToken(this.token);

        return this.token;
    }

    public async clean() {
        await this.refreshTokenStorage.storeToken(null);
        this.token = null;
        this.tokenSource.next(null);
    }

    public async isAuthenticated() {
        return !!(await this.refreshTokenStorage.getRefreshToken());
    }

    private notifyAboutNewToken(token: Promise<JwtToken>) {
        token
            .then((x) => this.tokenSource.next(x))
            .catch(x => { console.warn(x); this.tokenSource.next(null); });
    } 

    private async getExistingToken(): Promise<RefreshToken> {
        const oldRefreshToken = await this.refreshTokenStorage.getRefreshToken();
        if (!oldRefreshToken) {
            throw new OauthError('refresh token is empty, it should be set in storage first');
        }

        return { value: oldRefreshToken };
    }

    private async getNewToken() {
        console.log('updating jwt token');
        const oldRefreshToken = await this.getExistingToken();

        const accessCodeResponse = await this.accessCodeRequest.refresh(oldRefreshToken.value)
            .toPromise();

        await this.refreshTokenStorage.storeToken(accessCodeResponse.refreshToken);

        const expirationDate = moment.unix(+accessCodeResponse.expiresOn);
        const jwtToken = new JwtToken(expirationDate, accessCodeResponse.accessToken);

        return jwtToken;
    }
}