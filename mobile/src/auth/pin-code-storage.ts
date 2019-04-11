/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import SInfo from 'react-native-sensitive-info';
import Storage from './storage';

//============================================================================
export class PinCodeStorage {
    private static options = {
        keychainService: Storage.keychain,
        sharedPreferencesName: Storage.preferences,
    };

    private pin: string | null = null;

    //----------------------------------------------------------------------------
    public async getPin(): Promise<string | null> {
        if (this.pin !== null) {
            return this.pin;
        }

        return SInfo.getItem(Storage.Key.pinCode, PinCodeStorage.options)
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
    public async setPin(pin: string | null) {

        this.pin = pin;

        if (!pin) {
            await SInfo.deleteItem(Storage.Key.pinCode, PinCodeStorage.options);
        } else {
            await SInfo.setItem(Storage.Key.pinCode, pin, PinCodeStorage.options);
        }
    }
}
