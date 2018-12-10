/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { dataMember, required } from 'santee-dcts';
import { Map, Set } from 'immutable';
import { Optional } from 'types';

//============================================================================
export const enum Permission {
    readEmployeeInfo = 'Permission.readEmployeeInfo',
    readEmployeeVacationsCounter = 'Permission.readEmployeeVacationsCounter',
    readEmployeeDayoffsCounter = 'Permission.readEmployeeDayoffsCounter',
    readEmployeePhone = 'Permission.readEmployeePhone',
    readEmployeeCalendarEvents = 'Permission.readEmployeeCalendarEvents',
    createCalendarEvents = 'Permission.createCalendarEvents',
    approveCalendarEvents = 'Permission.approveCalendarEvents',
    rejectCalendarEvents = 'Permission.rejectCalendarEvents',
    completeSickLeave = 'Permission.completeSickLeave',
    prolongSickLeave = 'Permission.prolongSickLeave',
    cancelPendingCalendarEvents = 'Permission.cancelPendingCalendarEvents',
    editPendingCalendarEvents = 'Permission.editPendingCalendarEvents',
    cancelApprovedCalendarEvents = 'Permission.cancelApprovedCalendarEvents',
}

const mapping = Map<Permission>({
    'readEmployeeInfo': Permission.readEmployeeInfo,
    'readEmployeeVacationsCounter': Permission.readEmployeeVacationsCounter,
    'readEmployeeDayoffsCounter': Permission.readEmployeeDayoffsCounter,
    'readEmployeePhone': Permission.readEmployeePhone,
    'readEmployeeCalendarEvents': Permission.readEmployeeCalendarEvents,
    'createCalendarEvents': Permission.createCalendarEvents,
    'approveCalendarEvents': Permission.approveCalendarEvents,
    'rejectCalendarEvents': Permission.rejectCalendarEvents,
    'completeSickLeave': Permission.completeSickLeave,
    'prolongSickLeave': Permission.prolongSickLeave,
    'cancelPendingCalendarEvents': Permission.cancelPendingCalendarEvents,
    'editPendingCalendarEvents': Permission.editPendingCalendarEvents,
    'cancelApprovedCalendarEvents': Permission.cancelApprovedCalendarEvents,
});

//----------------------------------------------------------------------------
function toPermission(value: string): Optional<Permission> {
    return mapping.get(value, undefined);
}

//============================================================================
export class UserEmployeePermissions {

    @dataMember()
    @required()
    public employeeId: string = '';

    @dataMember({
        customDeserializer: (names: string[]) => Set(
            names
                .map(name => toPermission(name))
                .filter(value => !!value)
        )
    })
    @required()
    public permissionsNames: Set<Permission> = Set<Permission>();

    //----------------------------------------------------------------------------
    public get canApproveCalendarEvents(): boolean {
        return this.permissionsNames.has(Permission.approveCalendarEvents);
    }

    //----------------------------------------------------------------------------
    public get canRejectCalendarEvents(): boolean {
        return this.permissionsNames.has(Permission.rejectCalendarEvents);
    }

    //----------------------------------------------------------------------------
    public equals(obj: UserEmployeePermissions | null): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return this.employeeId === obj.employeeId && this.permissionsNames.equals(obj.permissionsNames);
    }
}
