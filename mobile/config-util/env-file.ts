import { writeFileSync } from 'fs';
import { EOL } from 'os';

export interface EnvConfiguration extends Record<string, string> {
    appCenterSecretId: string;
    apiUrl: string;
    oauthRedirectUri: string;
    oauthClientId: string;
    oauthTenant: string;
}

export function writeEnvConfig(config: EnvConfiguration, path: string) {
    const data = Object.entries(config).map(x => `${x[0]}=${x[1]}`).join(EOL);
    writeFileSync(path, data);
}