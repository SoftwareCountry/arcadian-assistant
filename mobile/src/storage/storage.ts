/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import SInfo from 'react-native-sensitive-info';
import { Nullable } from 'types';

//============================================================================
export class Storage {
    public static keychain = 'ArcadiaAssistant';
    public static preferences = 'ArcadiaAssistantPreferences';

    public static Key = {
        pinCode: 'pin-code',
        refreshToken: 'refresh-token',
    };

    private static options = {
        keychainService: Storage.keychain,
        sharedPreferencesName: Storage.preferences,
    };

    private pin: Nullable<string> = null;
    private refreshToken: Nullable<string> = null;

    //----------------------------------------------------------------------------
    public async getPin(): Promise<Nullable<string>> {
        return this.pin ? this.pin : this.get(Storage.Key.pinCode);
    }

    //----------------------------------------------------------------------------
    public async setPin(pin: Nullable<string>): Promise<null> {
        this.pin = pin;
        return this.set(Storage.Key.pinCode, pin);
    }

    //----------------------------------------------------------------------------
    public async getRefreshToken(): Promise<Nullable<string>> {
        return this.refreshToken ? this.refreshToken : this.get(Storage.Key.refreshToken);
    }

    //----------------------------------------------------------------------------
    public async setRefreshToken(refreshToken: Nullable<string>): Promise<null> {
        this.refreshToken = refreshToken;
        return this.set(Storage.Key.refreshToken, refreshToken);
    }

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private async get(key: string): Promise<Nullable<string>> {
        return SInfo.getItem(key, Storage.options)
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

    //----------------------------------------------------------------------------
    // noinspection JSMethodCanBeStatic
    private async set(key: string, value: Nullable<string>): Promise<null> {
        return value ?
            await SInfo.setItem(key, value, Storage.options) :
            await SInfo.deleteItem(key, Storage.options);
    }
}
