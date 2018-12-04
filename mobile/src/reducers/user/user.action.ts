import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { Action } from 'redux';

export interface LoadUser extends Action {
    type: 'LOAD-USER';
}

export const loadUser = (): LoadUser => ({ type: 'LOAD-USER' });

export interface LoadUserFinished extends Action {
    type: 'LOAD-USER-FINISHED';
    userEmployeeId: string;
}

export const loadUserFinished = (userEmployeeId: string): LoadUserFinished => ({ type: 'LOAD-USER-FINISHED', userEmployeeId });

export interface LoadUserEmployeeFinished extends Action {
    type: 'LOAD-USER-EMPLOYEE-FINISHED';
    employee: Employee;
}

export const loadUserEmployeeFinished = (employee: Employee): LoadUserEmployeeFinished => ({ type: 'LOAD-USER-EMPLOYEE-FINISHED', employee });

export interface LoadUserEmployeePermissions extends Action {
    type: 'LOAD-USER-EMPLOYEE-PERMISSIONS';
    employeeId: string;
}

export const loadUserEmployeePermissions = (employeeId: string): LoadUserEmployeePermissions => ({ type: 'LOAD-USER-EMPLOYEE-PERMISSIONS', employeeId });

export interface LoadUserEmployeePermissionsFininshed extends Action {
    type: 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED';
    permissions: UserEmployeePermissions;
}

export const loadUserEmployeePermissionsFinished = (permissions: UserEmployeePermissions): LoadUserEmployeePermissionsFininshed =>
    ({ type: 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED', permissions });

export type UserActions = LoadUser | LoadUserFinished | LoadUserEmployeeFinished
    | LoadUserEmployeePermissions | LoadUserEmployeePermissionsFininshed;
