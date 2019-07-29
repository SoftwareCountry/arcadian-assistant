/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Map, Set } from 'immutable';
import { Optional } from 'types';
import { dataMember, required } from 'santee-dcts';

//============================================================================
export const enum DepartmentFeature {
    dayoffs = 'DepartmentFeature.dayoffs',
}

//----------------------------------------------------------------------------
const mapping = Map<DepartmentFeature>({
    'dayoffs': DepartmentFeature.dayoffs,
});

//----------------------------------------------------------------------------
function toFeature(value: string): Optional<DepartmentFeature> {
    return mapping.get(value, undefined);
}

//============================================================================
export class DepartmentFeatures {

    @dataMember({
        customDeserializer: (features: string[]) => Set(
            features
                .map(name => toFeature(name))
                .filter(value => !!value)
        )
    })
    @required()
    public features: Set<DepartmentFeature> = Set<DepartmentFeature>();

    //----------------------------------------------------------------------------
    public has(feature: DepartmentFeature): boolean {
        return this.features.has(feature);
    }

    //----------------------------------------------------------------------------
    public equals(obj: DepartmentFeatures | null | undefined): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return this.features.equals(obj.features);
    }

    //----------------------------------------------------------------------------
    public get isDayoffsSupported(): boolean {
        return this.has(DepartmentFeature.dayoffs);
    }
}
