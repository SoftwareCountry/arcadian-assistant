import SInfo from 'react-native-sensitive-info';
import { Platform } from 'react-native';

export interface RefreshTokenStorage {
    storeToken(refreshToken: string | null): Promise<void>;

    getRefreshToken(): Promise<string>;
}

class RefreshTokenBaseStorage {
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

    protected async storeToken(refreshToken: string | null, useTouchId: boolean) {

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
                sharedPreferencesName: 'ArcadiaAssistantPreferences',
                touchID: useTouchId,
            });
        }
    }
}

export class RefreshTokenProtectedStorage extends RefreshTokenBaseStorage implements RefreshTokenStorage {

    public async storeToken(refreshToken: string | null) {
        await super.storeToken(refreshToken, true);
    }
}

export class RefreshTokenUnprotectedStorage extends RefreshTokenBaseStorage implements RefreshTokenStorage {

    public async storeToken(refreshToken: string | null) {
        await super.storeToken(refreshToken, false);
    }
}

export const createRefreshTokenStorage = (): RefreshTokenStorage => {

    const isAndroid = Platform.OS === 'android';
    const isSensorAvailable = !isAndroid && SInfo.isSensorAvailable();
    console.debug(`Fingerprint sensor is ${isSensorAvailable ? ` available` : `NOT available`}`);

    return isSensorAvailable ?
        new RefreshTokenProtectedStorage() :
        new RefreshTokenUnprotectedStorage();
};
