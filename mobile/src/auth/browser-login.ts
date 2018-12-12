import { NativeModules, Platform } from 'react-native';
import RNSFAuthenticaitonSession from 'react-native-sf-authentication-session';

interface AndroidAuthenticationSession {
    start(url: string): Promise<string>;

    reset(): void;
}

interface BrowserLogin {
    getAuthorizationCode(url: string, redirectUri: string): Promise<string>;
}


export const BrowserLogin: BrowserLogin =
    Platform.OS === 'android'
        ? {
            getAuthorizationCode: (NativeModules.AuthenticationSession as AndroidAuthenticationSession).start
        }
        : {
            getAuthorizationCode: RNSFAuthenticaitonSession.getSafariData
        };
