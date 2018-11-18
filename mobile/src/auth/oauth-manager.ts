import { createRefreshTokenStorage } from './refresh-token-storage';
import { OAuthProcess } from './oauth-process';

export class OAuthManager {

    public start(clientId: string, tenant: string, redirectUri: string) {
        const endpoint = `https://login.microsoftonline.com/${tenant}`;

        const authorizationUrl = `${endpoint}/oauth2/authorize`;
        const tokenUrl = `${endpoint}/oauth2/token`;

        return new OAuthProcess(clientId, authorizationUrl, tokenUrl, redirectUri, createRefreshTokenStorage());
    }
}
