import { RefreshTokenStorage } from './refresh-token-storage';
import { JwtTokenHandler } from './jwt-token-handler';
import { RefreshToken } from './login-request.1';

export class AuthenticationState {
    constructor(
        private readonly refreshTokenStorage: RefreshTokenStorage,
        private readonly jwtTokenHandler: JwtTokenHandler
    ) {}

    public async isAuthenticated() {
        return !!(await this.refreshTokenStorage.getRefreshToken());
    }

    public async forgetUser() {
        this.jwtTokenHandler.clean();
        // maybe, this call should be performed in 'clean' method above,
        // but i didn't really like that idea because there might be more logic involved
        await this.refreshTokenStorage.storeToken(null);
    }

    public async reset(refreshToken: RefreshToken) {
        await this.refreshTokenStorage.storeToken(refreshToken.value);
        this.jwtTokenHandler.refresh();
    }
}