import { TokenResponse, AccessCodeRequest } from './access-code-request';
import { OauthError } from './oauth-error';
import { Observable } from 'rxjs/Observable';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { LoginRequest } from './login-request';
import { RefreshTokenStorage } from './refresh-token-storage';

//https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-protocols-oauth-code

export class OAuthProcess {
    
        public get jwtToken() { return this.jwtTokenSource.asObservable(); }
        
        public refreshIntervalSeconds = 60 * 5; //once in 5 minutes
    
        private readonly authCode: Subject<string> = new Subject<string>();
    
        private readonly refreshTokenSource: Subject<string> = new Subject<string>();
    
        private readonly jwtTokenSource = new ReplaySubject<string>(1);
    
        private readonly accessCodeSubscription: Subscription;
    
        private readonly loginRequest: LoginRequest;
    
        private readonly accessCodeRequest: AccessCodeRequest;
    
        constructor(
            clientId: string,
            authorizationUrl: string,
            tokenUrl: string,
            redirectUri: string,
            private readonly refreshTokenStorage: RefreshTokenStorage) {
    
            this.loginRequest = new LoginRequest(clientId, redirectUri, authorizationUrl);
            this.accessCodeRequest = new AccessCodeRequest(clientId, redirectUri, tokenUrl);
    
            const accessCodeResponse = this.authCode
                .switchMap(code => this.accessCodeRequest.fetchNew(code));
    
            const refreshTokenObtainedAccessCodes = Observable.interval(this.refreshIntervalSeconds)
                .withLatestFrom(this.refreshTokenSource)
                .map(([, token]) => token)
                .filter(x => !!x)
                .switchMap(token => this.accessCodeRequest.refresh(token));
    
            this.accessCodeSubscription = Observable.merge(accessCodeResponse, refreshTokenObtainedAccessCodes)
                .subscribe(this.onNewTokenObtained, this.onTokenError);
        }
    
        public handleAuthorizationCodeResponse(responseUrl: string) {
            try {
                const code = this.loginRequest.getAuthorizationCodeFromResponse(responseUrl);
                this.authCode.next(code);
            }
            catch(e) {
                this.authCode.error(e);
            }
        }
    
        public async login() {
            const value = await this.refreshTokenStorage.getRefreshToken();
            if (!value) {
                //no refresh token is stored
                await this.loginRequest.openLoginPage(false); //TODO: catch exception
            } else {
                //request refresh            
                this.refreshTokenSource.next(value);
            }
        }
    
        public dispose() {
            this.accessCodeSubscription.unsubscribe();
        }
    
        public async forgetUser() {
            await this.refreshTokenStorage.storeToken(null);
        }
    
        private onNewTokenObtained = (tokenResponse: TokenResponse) => {
            this.refreshTokenStorage.storeToken(tokenResponse.refreshToken);
            this.refreshTokenSource.next(tokenResponse.refreshToken);
            this.jwtTokenSource.next(tokenResponse.accessToken);
        }
    
        private onTokenError = (error: any) => {
            if (error && error.status === 0) {
                console.warn('OAuth connectivity error occurred', error)
            } else {
                //ignore, no internet connection
                this.refreshTokenStorage.storeToken(null); // there was an error with /token endpoint so we delete existing token
                this.jwtTokenSource.error(new OauthError(error));
            }
        }
    }