/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Storage } from './storage';
import { Nullable } from 'types';

//============================================================================
export class RefreshTokenStorage extends Storage {
    private refreshToken: string | null = null;

    //----------------------------------------------------------------------------
    public async getRefreshToken(): Promise<Nullable<string>> {
        if (this.refreshToken !== null) {
            return this.refreshToken;
        }

        return super.get(Storage.Key.refreshToken);
    }

    //----------------------------------------------------------------------------
    public async setRefreshToken(refreshToken: Nullable<string>): Promise<null> {
        this.refreshToken = refreshToken;

        return super.set(Storage.Key.refreshToken, refreshToken);
    }
}
