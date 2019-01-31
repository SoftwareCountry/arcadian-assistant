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
        await this.jwtTokenHandler.clean();
    }

    public async reset(refreshToken: RefreshToken) {
        await this.refreshTokenStorage.storeToken(refreshToken.value);
        await this.jwtTokenHandler.refresh();
    }
}