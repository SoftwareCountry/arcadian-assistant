/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import SInfo from 'react-native-sensitive-info';
import Storage from './storage';

//============================================================================
export interface RefreshTokenStorage {
    storeToken(refreshToken: string | null): Promise<void>;

    getRefreshToken(): Promise<string | null>;
}

//============================================================================
class RefreshTokenBaseStorage {
    private static options = {
        keychainService: Storage.keychain,
        sharedPreferencesName: Storage.preferences,
    };

    private refreshToken: string | null = null;

    public async getRefreshToken(): Promise<string | null> {
        if (this.refreshToken !== null) {
            return this.refreshToken;
        }
        return SInfo.getItem(Storage.Key.refreshToken, RefreshTokenBaseStorage.options)
            .then(
                (value) => {
                    if (!value) {
                        return null;
                    }

                    return value;
                },
                (_) => {
                    return null;
                });
    }

    protected async storeToken(refreshToken: string | null, useTouchId: boolean) {

        if (!refreshToken) {
            this.refreshToken = null;
            await SInfo.deleteItem(Storage.Key.refreshToken, RefreshTokenBaseStorage.options);
        } else {
            this.refreshToken = refreshToken;
            await SInfo.setItem(Storage.Key.refreshToken, refreshToken, {
                ...RefreshTokenBaseStorage.options,
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
