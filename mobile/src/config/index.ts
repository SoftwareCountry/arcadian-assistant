import Config from 'react-native-config';

const config = Object.freeze({

    apiUrl: Config.apiUrl,

    oauth: Object.freeze({
        redirectUri: Config.oauthRedirectUri,
        clientId: Config.oauthClientId,
        tenant: Config.oauthTenant,
    })
});

export default config;
