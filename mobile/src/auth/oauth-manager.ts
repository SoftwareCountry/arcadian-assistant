import { createRefreshTokenStorage } from './refresh-token-storage';
import { OAuthProcess } from './oauth-process-2';
import { JwtTokenHandler } from './jwt-token-handler';
import { AccessCodeRequest } from './access-code-request';

export class OAuthManager {

    public start(clientId: string, tenant: string, redirectUri: string) {
        const endpoint = `https://login.microsoftonline.com/${tenant}`;

        const authorizationUrl = `${endpoint}/oauth2/authorize`;
        const tokenUrl = `${endpoint}/oauth2/token`;
        

        const refreshTokenStorage = createRefreshTokenStorage();

        const accessCodeRequest = new AccessCodeRequest(clientId, redirectUri, tokenUrl);
        const jwtTokenHandler = new JwtTokenHandler(accessCodeRequest, refreshTokenStorage);

        return new OAuthProcess(clientId, authorizationUrl, tokenUrl, redirectUri, jwtTokenHandler);
    }
}
