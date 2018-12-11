import { Employee } from '../organization/employee.model';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { Action } from 'redux';
import { UserPreferences } from './user-preferences.model';

export interface LoadUser extends Action {
    type: 'LOAD-USER';
}

export const loadUser = (): LoadUser => ({ type: 'LOAD-USER' });

export interface LoadUserFinished extends Action {
    type: 'LOAD-USER-FINISHED';
    userEmployeeId: string;
}

export const loadUserFinished = (userEmployeeId: string): LoadUserFinished => ({
    type: 'LOAD-USER-FINISHED',
    userEmployeeId
});

export interface LoadUserEmployeeFinished extends Action {
    type: 'LOAD-USER-EMPLOYEE-FINISHED';
    employee: Employee;
}

export const loadUserEmployeeFinished = (employee: Employee): LoadUserEmployeeFinished => ({
    type: 'LOAD-USER-EMPLOYEE-FINISHED',
    employee
});

export interface LoadUserEmployeePermissions extends Action {
    type: 'LOAD-USER-EMPLOYEE-PERMISSIONS';
    employeeId: string;
}

export const loadUserEmployeePermissions = (employeeId: string): LoadUserEmployeePermissions => ({
    type: 'LOAD-USER-EMPLOYEE-PERMISSIONS',
    employeeId
});

export interface LoadUserEmployeePermissionsFinished extends Action {
    type: 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED';
    permissions: UserEmployeePermissions;
}

export const loadUserEmployeePermissionsFinished = (permissions: UserEmployeePermissions): LoadUserEmployeePermissionsFinished =>
    ({ type: 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED', permissions });

export interface LoadUserPreferences extends Action {
    type: 'LOAD-USER-PREFERENCES';
    userId: string;
}

export const loadUserPreferences = (userId: string): LoadUserPreferences => ({ type: 'LOAD-USER-PREFERENCES', userId });

export interface UpdateUserPreferences extends Action {
    type: 'UPDATE-USER-PREFERENCES';
    userId: string;
    previousPreferences: UserPreferences;
    preferences: UserPreferences;
}

export const updateUserPreferences = (userId: string, previousPreferences: UserPreferences, newPreferences: UserPreferences): UpdateUserPreferences => ({
    type: 'UPDATE-USER-PREFERENCES',
    userId: userId,
    previousPreferences: previousPreferences,
    preferences: newPreferences,
});

export interface LoadUserPreferencesFinished extends Action {
    type: 'LOAD-USER-PREFERENCES-FINISHED';
    preferences: UserPreferences;
}

export const loadUserPreferencesFinished = (preferences: UserPreferences): LoadUserPreferencesFinished =>
    ({ type: 'LOAD-USER-PREFERENCES-FINISHED', preferences });


export type UserActions = LoadUser | LoadUserFinished | LoadUserEmployeeFinished
    | LoadUserEmployeePermissions | LoadUserEmployeePermissionsFinished
    | LoadUserPreferences | UpdateUserPreferences | LoadUserPreferencesFinished;
