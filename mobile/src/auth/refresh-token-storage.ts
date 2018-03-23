import { AsyncStorage } from 'react-native';

export interface RefreshTokenStorage {
    storeToken(refreshToken: string | null) : Promise<void>;
    getRefreshToken(): Promise<string>;
}

export class RefreshTokenFilesystemStorage implements RefreshTokenStorage {
    private readonly keyName = 'refresh-token';
    private refreshToken: string = null;

    public async getRefreshToken() {
        if (this.refreshToken !== null) {
            return this.refreshToken;
        }

        return AsyncStorage.getItem(this.keyName);
    }

    public async storeToken(refreshToken: string | null) {

        if (!refreshToken) {
            this.refreshToken = null;
            await AsyncStorage.removeItem(this.keyName);
        } else {
            this.refreshToken = refreshToken;
            await AsyncStorage.setItem(this.keyName, refreshToken);
        }
    }
}