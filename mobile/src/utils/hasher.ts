/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { hash } from 'immutable';

//============================================================================
export class Hasher {
    private _value: number;

    //----------------------------------------------------------------------------
    public get value(): number {
        return this._value;
    }

    //----------------------------------------------------------------------------
    public constructor(entity: any | null | undefined) {
        this._value = Hasher.hash(entity);
    }

    //----------------------------------------------------------------------------
    public combine(entity: any | null | undefined) {
        this._value = this._value * 31 + Hasher.hash(entity);
    }

    //----------------------------------------------------------------------------
    private static hash(value: any | null | undefined): number {
        if (!value) {
            return 0;
        }

        return hash(value.valueOf());
    }
}
