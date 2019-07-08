import Config from 'react-native-config';
import { Platform } from 'react-native';

const config = Object.freeze({

    baseUrl: Config.baseUrl as string,
    apiUrl: `${Config.baseUrl}/api`,
    downloadLink: Platform.OS === 'android' ? `${Config.baseUrl}/get/android` :
        `itms-services://?action=download-manifest&url=${Config.baseUrl}/download/ios-manifest`,

    oauth: Object.freeze({
        redirectUri: Config.oauthRedirectUri as string,
        clientId: Config.oauthClientId as string,
        tenant: Config.oauthTenant as string,
    })
});

//----------------------------------------------------------------------------

export default config;
