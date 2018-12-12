/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { dataMember, required } from 'santee-dcts';
import { CalendarEventId } from './calendar-event.model';
import { EmployeeId } from '../organization/employee.model';
import { hash, ValueObject } from 'immutable';
import { Hasher } from '../../utils/hasher';

//============================================================================
export class Approval implements ValueObject {
    @dataMember()
    @required()
    public approverId: EmployeeId;

    public eventId: CalendarEventId;

    //----------------------------------------------------------------------------
    constructor(approverId: EmployeeId = '', eventId: CalendarEventId = '') {
        this.approverId = approverId;
        this.eventId = eventId;
    }

    //----------------------------------------------------------------------------
    public equals(obj: Approval | null): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return this.approverId === obj.approverId &&
            this.eventId === obj.eventId;
    }

    //----------------------------------------------------------------------------
    public hashCode(): number {
        const hasher = new Hasher(this.approverId);
        hasher.combine(this.eventId);
        return hasher.value;
    }
}
