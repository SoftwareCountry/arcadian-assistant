import { EnvConfiguration, writeEnvConfig } from './env-file';
import { writeGoogleConfig } from './google-services';

interface Configuration {
    env: EnvConfiguration;
    google: string | null;
}

function createConfiguration(): Configuration {

    let google = process.env['AA_GOOGLE_SERVICES'] || null;
    if (google) {
        google = google.replace(/\\/g, '');
    }

    return {
        env: {
            appCenterSecretId: process.env['AA_API_APPCENTER'] || 'WRITE_ME',
            baseUrl: process.env['AA_BASE_URL'] || 'https://arcadia-assistant-dev.arcadialab.ru',
            oauthRedirectUri: process.env['AA_OAUTH_REDIRECT_URI'] || 'arcadia-assistant://on-login',
            oauthClientId: process.env['AA_OAUTH_CLIENT_ID'] || 'a2ccb221-60e2-47b8-b28c-bf88a59f7f4a',
            oauthTenant: process.env['AA_OAUTH_TENANT'] || 'fa4e9c1f-6222-443d-a083-28f80c1ffefc',
        },
        google,
    };
}

export function run() {
    const config = createConfiguration();
    console.log('writing app config', config);

    writeEnvConfig(config.env, '../.env');
    if (config.google) {
        writeGoogleConfig(config.google, '../android/app/google-services.json');
    }
}
