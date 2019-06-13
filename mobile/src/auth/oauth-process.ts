import { LoginRequest } from './login-request';
import { JwtTokenHandler } from './jwt-token-handler';

export class OAuthProcess {

    private readonly loginRequest: LoginRequest;

    //----------------------------------------------------------------------------
    constructor(
        clientId: string,
        authorizationUrl: string,
        tokenUrl: string,
        redirectUri: string,
        public readonly jwtTokenHandler: JwtTokenHandler //TODO: public so far
    ) {

        this.loginRequest = new LoginRequest(clientId, redirectUri, authorizationUrl, tokenUrl);
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
        await this.jwtTokenHandler.clean();
    }

    public async getRefreshToken(): Promise<string | null> {
        return await this.jwtTokenHandler.getRefreshToken();
    }

    private async loginOnLoginPage() {
        const refreshToken = await this.loginRequest.getRefreshTokenFromLoginPage(true);
        await this.jwtTokenHandler.reset(refreshToken);
    }

    private async isAuthenticated() {
        try {
            return await this.jwtTokenHandler.isAuthenticated();
        } catch (e) {
            console.log('Error getting initial state, considering user unauthenticated', e);
            return false;
        }
    }
}
