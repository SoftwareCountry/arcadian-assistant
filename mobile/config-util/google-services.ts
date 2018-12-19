import { writeFileSync } from 'fs';

export function writeGoogleConfig(config: string, path: string) {
    writeFileSync(path, config);
}