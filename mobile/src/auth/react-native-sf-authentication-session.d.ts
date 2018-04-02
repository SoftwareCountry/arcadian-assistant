declare module 'react-native-sf-authentication-session' {
    export function getSafariData(address: string, callbackURL: string): Promise<string>;
}