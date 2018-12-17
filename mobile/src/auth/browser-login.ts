import { NativeModules, Platform } from 'react-native';

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
            getAuthorizationCode: NativeModules.ArcadiaAuthenticationSession.getSafariData
        };
