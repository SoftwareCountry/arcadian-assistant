import { Dispatch } from 'react-redux';
import { OAuthProcess } from '../auth/oauth-process';

export class DeepLinking {
    constructor(private dispatch: Dispatch<any>, private oauthProcess: OAuthProcess) {
    }

    public openUrl(url: string) {
        if (!url) {
            return;
        }

        const m = url.match(/.*\?(.*)/)[1];
        if (m) {
            this.oauthProcess.handleAuthorizationCodeResponse(url);
        }

        /*
        const uri = new URL(url);
        
        switch (uri.pathname.toLowerCase()) {
            case 'on-login':
                requestAccessCode(uri.searchParams);
            break;
        }*/
    }
}