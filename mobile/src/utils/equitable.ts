/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

//============================================================================
interface Equitable {
    equals(obj: any | null | undefined): boolean;
}

//----------------------------------------------------------------------------
export function equals<T extends Equitable>(x: T | null | undefined, y: T | null | undefined) {
    if (x && y) {
        return x.equals(y);
    }

    return  x === y;
}
