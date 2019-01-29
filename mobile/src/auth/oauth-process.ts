/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { AccessCodeRequest, TokenResponse } from './access-code-request';
import { LoginRequest } from './login-request';
import { RefreshTokenStorage } from './refresh-token-storage';
import { AuthenticationState, NotAuthenticatedState } from './authentication-state';
import moment from 'moment';
import { catchError, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { concat, EMPTY, interval, merge, Observable, of, ReplaySubject, Subject, Subscription } from 'rxjs';
import { Nullable, Optional } from 'types';

//https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-protocols-oauth-code

//============================================================================
const notAuthenticatedInstance: NotAuthenticatedState = { isAuthenticated: false }; //save a reference, so distinct works

//============================================================================
const cancellationErrorCode = '1';
const networkErrorStatus = 0;

//============================================================================
export class OAuthProcess {
    //----------------------------------------------------------------------------
    public get authenticationState() {
        return this.authenticationStateSource.asObservable().pipe(distinctUntilChanged());
    }

    private readonly refreshIntervalSeconds = 30;

    private readonly tokenValidDuration = moment.duration(5, 'm');

    private readonly authorizationCode: Subject<string> = new Subject<string>();

    private readonly refreshTokenSource: Subject<Nullable<RefreshTokenRequest>> = new Subject<Nullable<RefreshTokenRequest>>();

    private readonly authenticationStateSource = new ReplaySubject<AuthenticationState>(1);

    private readonly accessCodeSubscription: Subscription;

    private readonly loginRequest: LoginRequest;

    private readonly accessCodeRequest: AccessCodeRequest;

    //----------------------------------------------------------------------------
    constructor(
        clientId: string,
        authorizationUrl: string,
        tokenUrl: string,
        redirectUri: string,
        private readonly refreshTokenStorage: RefreshTokenStorage) {

        this.loginRequest = new LoginRequest(clientId, redirectUri, authorizationUrl);
        this.accessCodeRequest = new AccessCodeRequest(clientId, redirectUri, tokenUrl);

        const accessCodeResponse = this.authorizationCode.pipe(
            switchMap((code: string) => {
                return this.accessCodeRequest.fetchNew(code).pipe(
                    catchError((error) => {
                        this.handleError(error);
                        return EMPTY;
                    }),
                );
            }));

        const refreshTokenObtainedAccessCodes = this.refreshTokenSource.pipe(
            switchMap(request => this.getPeriodicalRefreshTokens(request)),
            switchMap(token => {
                if (!token) {
                    return of<Nullable<TokenResponse>>(null);
                }

                return this.accessCodeRequest.refresh(token).pipe(
                    catchError((error) => {

                        if (!this.isNetworkError(error)) {
                            this.handleError(error);
                            return EMPTY;
                        }

                        return of(new TokenResponse(token));
                    }),
                );
            }));

        this.accessCodeSubscription = merge(accessCodeResponse, refreshTokenObtainedAccessCodes)
            .subscribe(x => this.onNewTokenObtained(x));
    }

    //----------------------------------------------------------------------------
    public handleAuthorizationCodeResponse(responseUrl: string) {
        try {
            const code = this.loginRequest.getAuthorizationCodeFromResponse(responseUrl);
            this.authorizationCode.next(code);
        } catch (error) {
            this.handleError(error);
        }
    }

    //----------------------------------------------------------------------------
    public async login() {
        let value: Nullable<string> = null;
        try {
            value = await this.refreshTokenStorage.getRefreshToken();
        } catch (e) {
            console.warn('Authentication fail with error', e);
        } finally {
            if (!value) {
                console.debug('Refresh token is not found in storage, opening login page...');
                //no refresh token is stored
                try {
                    const authorizationCodeResponseUrl = await this.loginRequest.openLoginPage(true);
                    this.handleAuthorizationCodeResponse(authorizationCodeResponseUrl);
                } catch (error) {
                    this.handleError(error);
                }
            } else {
                console.debug('Using refresh token from the application storage');
                //request refresh
                this.refreshTokenSource.next({ immediateRefresh: true, tokenValue: value });
            }
        }
    }

    //----------------------------------------------------------------------------
    public async logout() {
        return this.forgetUser();
    }

    //----------------------------------------------------------------------------
    public dispose() {
        this.accessCodeSubscription.unsubscribe();
    }

    //----------------------------------------------------------------------------
    private async forgetUser() {
        await this.storeRefreshToken(null);
        this.refreshTokenSource.next(null);
    }

    //----------------------------------------------------------------------------
    private onNewTokenObtained(tokenResponse: Nullable<TokenResponse>) {
        this.storeRefreshToken(tokenResponse ? tokenResponse.refreshToken : null);

        if (!tokenResponse) {
            this.authenticationStateSource.next(notAuthenticatedInstance);
        } else {
            this.refreshTokenSource.next({ tokenValue: tokenResponse.refreshToken, immediateRefresh: false });
            this.authenticationStateSource.next({
                isAuthenticated: true,
                jwtToken: tokenResponse.accessToken,
                refreshToken: tokenResponse.refreshToken,
                validUntil: moment().add(this.tokenValidDuration),
            });
        }
    }

    //----------------------------------------------------------------------------
    private async storeRefreshToken(token: string | null) {
        try {
            await this.refreshTokenStorage.storeToken(token);
        } catch (e) {
            console.warn('Couldn\'t change refresh token in the storage', e);
        }
    }

    //----------------------------------------------------------------------------
    private getPeriodicalRefreshTokens(request: Nullable<RefreshTokenRequest>): Observable<Nullable<string>> {
        if (!request) {
            return of(null);
        }

        const scheduledEmition = interval(this.refreshIntervalSeconds * 1000).pipe(map(() => request.tokenValue));
        if (request.immediateRefresh) {
            return concat(of(request.tokenValue), scheduledEmition);
        } else {
            return scheduledEmition;
        }
    }

    //----------------------------------------------------------------------------
    private handleError(error: any) {

        let errorInstance: NotAuthenticatedState = notAuthenticatedInstance;


        if (!this.isCancellationError(error)) {
            errorInstance = {
                ...errorInstance,
                error,
            };
        }

        this.refreshTokenSource.next(null);
        this.authenticationStateSource.next(errorInstance);
    }

    //----------------------------------------------------------------------------
    private isCancellationError(error: any): boolean {
        const errorCode = error.code;

        return errorCode && errorCode === cancellationErrorCode;
    }

    //----------------------------------------------------------------------------
    private isNetworkError(error: any): boolean {
        const errorStatus = error.status;

        return (errorStatus !== undefined && errorStatus === networkErrorStatus);
    }
}


//============================================================================
interface RefreshTokenRequest {
    tokenValue: string;
    immediateRefresh: boolean;
}
