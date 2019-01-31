import { AuthenticationState } from './authentication-state.1';
import { LoginRequest } from './login-request.1';
import { RefreshTokenStorage } from './refresh-token-storage';
import { JwtTokenHandler } from './jwt-token-handler';

export class OAuthProcess {

    private readonly loginRequest: LoginRequest;
    private readonly authenticationState: AuthenticationState;

    //----------------------------------------------------------------------------
    constructor(
        clientId: string,
        authorizationUrl: string,
        tokenUrl: string,
        redirectUri: string,
        private readonly refreshTokenStorage: RefreshTokenStorage,
        public readonly jwtTokenHandler: JwtTokenHandler //TODO: public so far
        ) {

        this.loginRequest = new LoginRequest(clientId, redirectUri, authorizationUrl, tokenUrl);
        this.authenticationState = new AuthenticationState(refreshTokenStorage, jwtTokenHandler);
    }

    //----------------------------------------------------------------------------
    public async login() {
        const isAuthenticated = await this.isAuthenticated();

        if (isAuthenticated) {
            await this.jwtTokenHandler.refresh();           
        } else {
            await this.loginOnLoginPage();
        }
    }

    public async logout() {
        await this.authenticationState.forgetUser();
    }

    private async loginOnLoginPage() {
        try {
            const refreshToken = await this.loginRequest.getRefreshTokenFromLoginPage(true);
            await this.jwtTokenHandler.reset(refreshToken);
            //this.authenticationState.reset(refreshToken);
        } catch (e) {
            // TODO: handle
        }
    }

    private async isAuthenticated() {
        try {
            return await this.authenticationState.isAuthenticated();
        } catch (e) {
            console.log('Error getting initial state, considering user unauthenticated', e);
            return false;
        } 
    }

    /*
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
    */
}