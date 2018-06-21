import { required, dataMember, typedArray } from 'santee-dcts';

export class UserPermissions {

    @dataMember()
    @typedArray(String)
    @required()
    public userEmployeePermissions: string[];

    public get canApproveCalendarEvents(): boolean {
        return this.hasPermission('approveCalendarEvents');
    }

    public get canRejectCalendarEvents(): boolean {
        return this.hasPermission('rejectCalendarEvents');
    }

    private hasPermission(permission: string): boolean {
        return this.userEmployeePermissions.some(x => x === permission);
    }
}