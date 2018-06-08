import { AsyncStorage } from 'react-native';
import SInfo from 'react-native-sensitive-info';

export interface RefreshTokenStorage {
    storeToken(refreshToken: string | null): Promise<void>;
    getRefreshToken(): Promise<string>;
}

export class RefreshTokenFilesystemStorage implements RefreshTokenStorage {
    private readonly keyName = 'refresh-token';
    private refreshToken: string = null;

    public async getRefreshToken() {
        if (this.refreshToken !== null) {
            return this.refreshToken;
        }
        return SInfo.getItem(this.keyName, {
            keychainService: 'ArcadiaAssistant',
            sharedPreferencesName: 'ArcadiaAssistantPreferences',
        });
    }

    public async storeToken(refreshToken: string | null) {

        if (!refreshToken) {
            this.refreshToken = null;
            await SInfo.deleteItem(this.keyName, {
                keychainService: 'ArcadiaAssistant',
                sharedPreferencesName: 'ArcadiaAssistantPreferences',
            });
        } else {
            this.refreshToken = refreshToken;
            await SInfo.setItem(this.keyName, refreshToken, {
                keychainService: 'ArcadiaAssistant',
                kSecAccessControl: 'kSecAccessControlTouchIDCurrentSet',
                sharedPreferencesName: 'ArcadiaAssistantPreferences',
                touchID: true,
            });
        }
    }
}