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
    public constructor(entity: any) {
        this._value = hash(entity);
    }

    //----------------------------------------------------------------------------
    public combine(entity: any) {
        this._value = this._value * 31 + hash(entity);
    }
}
