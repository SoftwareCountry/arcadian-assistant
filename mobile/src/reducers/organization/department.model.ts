/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { dataMember, required } from 'santee-dcts';
import { Nullable, Optional } from 'types';
import { Hasher } from '../../utils/hasher';
import { ValueObject } from 'immutable';

//============================================================================
export class Department implements ValueObject {

    @dataMember()
    @required()
    public departmentId: string = '';

    @dataMember()
    @required()
    public abbreviation: string = '';

    @dataMember()
    @required()
    public name: string = '';

    @dataMember()
    @required({ nullable: true })
    public parentDepartmentId: Nullable<string> = null;

    @dataMember()
    @required({ nullable: true })
    public chiefId: Nullable<string> = null;

    @dataMember()
    @required({ nullable: true })
    public isHeadDepartment: Nullable<boolean> = null;

    //----------------------------------------------------------------------------
    public equals(obj: Optional<Department>): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return obj.departmentId === this.departmentId
            && obj.abbreviation === this.abbreviation
            && obj.name === this.name
            && obj.parentDepartmentId === this.parentDepartmentId
            && obj.chiefId === this.chiefId
            && obj.isHeadDepartment === this.isHeadDepartment;
    }

    //----------------------------------------------------------------------------
    public hashCode(): number {
        const hasher = new Hasher(this.departmentId);
        hasher.combine(this.abbreviation);
        hasher.combine(this.name);
        hasher.combine(this.parentDepartmentId);
        hasher.combine(this.chiefId);
        hasher.combine(this.isHeadDepartment);
        return hasher.value;
    }
}
