import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { UserPermissions } from './user-permissions.model';

export interface LoadUser {
    type: 'LOAD-USER';
}

export const loadUser = (): LoadUser => ({ type: 'LOAD-USER' });

export interface LoadUserFinished {
    type: 'LOAD-USER-FINISHED';
    userEmployeeId: string;
}

export const loadUserFinished = (userEmployeeId: string): LoadUserFinished => ({ type: 'LOAD-USER-FINISHED', userEmployeeId });

export interface LoadUserEmployeeFinished {
    type: 'LOAD-USER-EMPLOYEE-FINISHED';
    employee: Employee;
}

export const loadUserEmployeeFinished = (employee: Employee): LoadUserEmployeeFinished => ({ type: 'LOAD-USER-EMPLOYEE-FINISHED', employee });

export interface LoadUserEmployeePermissions {
    type: 'LOAD-USER-EMPLOYEE-PERMISSIONS';
}

export const loadUserEmployeePermissions = (): LoadUserEmployeePermissions => ({ type: 'LOAD-USER-EMPLOYEE-PERMISSIONS' });

export interface LoadUserEmployeePermissionsFininshed {
    type: 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED';
    userPermissions: UserPermissions;
}

export const loadUserEmployeePermissionsFinished = (userPermissions: UserPermissions): LoadUserEmployeePermissionsFininshed => 
    ({ type: 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED', userPermissions });

export type UserActions = LoadUser | LoadUserFinished | LoadUserEmployeeFinished 
    | LoadUserEmployeePermissions | LoadUserEmployeePermissionsFininshed;
