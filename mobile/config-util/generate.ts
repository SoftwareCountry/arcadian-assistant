import {Configuration as ApplicationConfiguration} from '../src/config/configuration';
import { writeFileSync } from 'fs';
import { EOL } from 'os';

//appCenterSecretId=WRITE_ME
//apiUrl=https://arcadia-assistant-dev.arcadialab.ru/api
//oauthRedirectUri=arcadia-assistant://on-login
//oauthClientId=a2ccb221-60e2-47b8-b28c-bf88a59f7f4a
//oauthTenant=fa4e9c1f-6222-443d-a083-28f80c1ffefc

interface EnvConfiguration extends Record<string, string> {
    appCenterSecretId: string;
    apiUrl: string;
    oauthRedirectUri: string;
    oauthClientId: string;
    oauthTenant: string;
}

interface Configuration {
    env: EnvConfiguration;
}

function createConfiguration(): Configuration {
    return {
        env: {
            appCenterSecretId: process.env['AA_API_APPCENTER'] || '<writeme>',
            apiUrl: process.env['AA_API_URL'] || 'https://arcadia-assistant-dev.arcadialab.ru/api',
            oauthRedirectUri: process.env['AA_OAUTH_REDIRECT_URI'] || 'arcadia-assistant://on-login',
            oauthClientId: process.env['AA_OAUTH_CLIENT_ID'] || 'a2ccb221-60e2-47b8-b28c-bf88a59f7f4a',
            oauthTenant: process.env['AA_OAUTH_TENANT'] || 'fa4e9c1f-6222-443d-a083-28f80c1ffefc'
        }
    };
}

function writeEnvConfig(config: EnvConfiguration) {
    const data = Object.entries(config).map(x => `${x[0]}=${x[1]}`).join(EOL);
    writeFileSync('../.env', data);
}

export function run() {    
    writeEnvConfig(createConfiguration().env);
}