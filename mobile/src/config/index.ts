import Config from 'react-native-config';

const config = Object.freeze({

    apiUrl: Config.apiUrl as string,

    oauth: Object.freeze({
        redirectUri: Config.oauthRedirectUri as string,
        clientId: Config.oauthClientId as string,
        tenant: Config.oauthTenant as string,
    })
});

export default config;
