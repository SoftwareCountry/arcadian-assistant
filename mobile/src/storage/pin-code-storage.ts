/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Storage } from './storage';
import { Nullable } from 'types';

//============================================================================
export class PinCodeStorage extends Storage {
    private pin: string | null = null;

    //----------------------------------------------------------------------------
    public async getPin(): Promise<Nullable<string>> {
        if (this.pin !== null) {
            return this.pin;
        }

        return super.get(Storage.Key.pinCode);
    }

    //----------------------------------------------------------------------------
    public async setPin(pin: Nullable<string>): Promise<null> {

        this.pin = pin;

        return super.set(Storage.Key.pinCode, pin);
    }
}
