import { required, dataMember, typedArray } from 'santee-dcts';

export class UserEmployeePermissions {

    @dataMember()
    @required()
    public employeeId: string;

    @dataMember()
    @typedArray(String)
    @required()
    public permissionsNames: string[];

    public get canApproveCalendarEvents(): boolean {
        return this.hasPermission('approveCalendarEvents');
    }

    public get canRejectCalendarEvents(): boolean {
        return this.hasPermission('rejectCalendarEvents');
    }

    private hasPermission(permission: string): boolean {
        return this.permissionsNames.some(x => x === permission);
    }
}