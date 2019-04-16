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

    //----------------------------------------------------------------------------
    public async get(key: string): Promise<Nullable<string>> {
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
    public async set(key: string, value: Nullable<string>): Promise<null> {
        if (!value) {
            return await SInfo.deleteItem(key, Storage.options);
        } else {
            return await SInfo.setItem(key, value, Storage.options);
        }
    }
}
