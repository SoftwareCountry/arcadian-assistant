/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import SInfo from 'react-native-sensitive-info';

//============================================================================
export interface RefreshTokenStorage {
    storeToken(refreshToken: string | null): Promise<void>;

    getRefreshToken(): Promise<string | null>;
}

//============================================================================
class RefreshTokenBaseStorage {
    private readonly keyName = 'refresh-token';
    private refreshToken: string | null = null;

    public async getRefreshToken(): Promise<string | null> {
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

//============================================================================
// noinspection JSUnusedGlobalSymbols
export class RefreshTokenProtectedStorage extends RefreshTokenBaseStorage implements RefreshTokenStorage {

    public async storeToken(refreshToken: string | null) {
        await super.storeToken(refreshToken, true);
    }
}

//============================================================================
export class RefreshTokenUnprotectedStorage extends RefreshTokenBaseStorage implements RefreshTokenStorage {

    public async storeToken(refreshToken: string | null) {
        await super.storeToken(refreshToken, false);
    }
}

//----------------------------------------------------------------------------
export const createRefreshTokenStorage = (): RefreshTokenStorage => {

    // In current implementation the storage itself is not fingerprint protected.
    // FingerprintScanner is used on the SplashScreen to allow/disallow user to proceed with the
    // token value stored.
    // TODO: get rid of SInfo component in future versions
    return new RefreshTokenUnprotectedStorage();
};
