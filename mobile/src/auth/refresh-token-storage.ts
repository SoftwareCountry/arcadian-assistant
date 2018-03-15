import { AsyncStorage } from 'react-native';

export interface RefreshTokenStorage {
    storeToken(refreshToken: string | null) : Promise<void>;
    getRefreshToken(): Promise<string>;
}

export class RefreshTokenFilesystemStorage implements RefreshTokenStorage {
    private keyName = 'refresh-token';

    public getRefreshToken() {
        return AsyncStorage.getItem(this.keyName);
    }

    public async storeToken(refreshToken: string | null) {

        if (!refreshToken) {
            await AsyncStorage.removeItem(this.keyName);
        } else {
            await AsyncStorage.setItem(this.keyName, refreshToken);
        }
    }
}