import { OAuthProcess } from './oauth-process';
import { JwtTokenHandler } from './jwt-token-handler';
import { AccessCodeRequest } from './access-code-request';
import { RefreshTokenStorage } from '../storage/refresh-token-storage';

export class OAuthManager {

    public start(clientId: string, tenant: string, redirectUri: string) {
        const endpoint = `https://login.microsoftonline.com/${tenant}`;

        const authorizationUrl = `${endpoint}/oauth2/authorize`;
        const tokenUrl = `${endpoint}/oauth2/token`;


        const refreshTokenStorage = new RefreshTokenStorage();

        const accessCodeRequest = new AccessCodeRequest(clientId, redirectUri, tokenUrl);
        const jwtTokenHandler = new JwtTokenHandler(accessCodeRequest, refreshTokenStorage);

        return new OAuthProcess(clientId, authorizationUrl, tokenUrl, redirectUri, jwtTokenHandler);
    }
}
