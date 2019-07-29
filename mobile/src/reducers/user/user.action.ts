/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Employee } from '../organization/employee.model';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { Action } from 'redux';
import { UserPreferences } from './user-preferences.model';
import { DepartmentFeatures } from './department-features.model';

//============================================================================
export enum UserActionType {
    loadUser = 'LOAD-USER',
    loadUserFinished = 'LOAD-USER-FINISHED',
    loadUserEmployeeFinished = 'LOAD-USER-EMPLOYEE-FINISHED',
    loadUserEmployeePermissions = 'LOAD-USER-EMPLOYEE-PERMISSIONS',
    loadUserEmployeePermissionsFinished = 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED',
    loadUserPreferences = 'LOAD-USER-PREFERENCES',
    updateUserPreferences = 'UPDATE-USER-PREFERENCES',
    loadUserPreferencesFinished = 'LOAD-USER-PREFERENCES-FINISHED',
    loadUserDepartmentFeatures = 'LOAD-USER-DEPARTMENT-FEATURES',
    loadUserDepartmentFeaturesFinished = 'LOAD-USER-DEPARTMENT-FEATURES-FINISHED',
}

//----------------------------------------------------------------------------
// - Actions
//----------------------------------------------------------------------------
export interface LoadUser extends Action<UserActionType.loadUser> {
}

export interface LoadUserFinished extends Action<UserActionType.loadUserFinished> {
    userEmployeeId: string;
}

export interface LoadUserEmployeeFinished extends Action<UserActionType.loadUserEmployeeFinished> {
    employee: Employee;
}

export interface LoadUserEmployeePermissions extends Action<UserActionType.loadUserEmployeePermissions> {
    employeeId: string;
}

export interface LoadUserEmployeePermissionsFinished extends Action<UserActionType.loadUserEmployeePermissionsFinished> {
    permissions: UserEmployeePermissions;
}

export interface LoadUserPreferences extends Action<UserActionType.loadUserPreferences> {
    userId: string;
}

export interface UpdateUserPreferences extends Action<UserActionType.updateUserPreferences> {
    userId: string;
    previousPreferences: UserPreferences;
    preferences: UserPreferences;
}

export interface LoadUserPreferencesFinished extends Action<UserActionType.loadUserPreferencesFinished> {
    preferences: UserPreferences;
}

export interface LoadUserDepartmentFeatures extends Action<UserActionType.loadUserDepartmentFeatures> {
}

export interface LoadUserDepartmentFeaturesFinished extends Action<UserActionType.loadUserDepartmentFeaturesFinished> {
    features: DepartmentFeatures;
}

//----------------------------------------------------------------------------
// - Action creators
//----------------------------------------------------------------------------

export const loadUser = (): LoadUser => {
    return {
        type: UserActionType.loadUser,
    };
};

export const loadUserFinished = (userEmployeeId: string): LoadUserFinished => {
    return {
        type: UserActionType.loadUserFinished,
        userEmployeeId,
    };
};

export const loadUserEmployeeFinished = (employee: Employee): LoadUserEmployeeFinished => {
    return {
        type: UserActionType.loadUserEmployeeFinished,
        employee,
    };
};

export const loadUserEmployeePermissions = (employeeId: string): LoadUserEmployeePermissions => {
    return {
        type: UserActionType.loadUserEmployeePermissions,
        employeeId,
    };
};

export const loadUserEmployeePermissionsFinished = (permissions: UserEmployeePermissions): LoadUserEmployeePermissionsFinished => {
    return {
        type: UserActionType.loadUserEmployeePermissionsFinished,
        permissions,
    };
};

export const loadUserPreferences = (userId: string): LoadUserPreferences => {
    return {
        type: UserActionType.loadUserPreferences,
        userId,
    };
};

export const updateUserPreferences = (userId: string, previousPreferences: UserPreferences, newPreferences: UserPreferences): UpdateUserPreferences => {
    return {
        type: UserActionType.updateUserPreferences,
        userId: userId,
        previousPreferences: previousPreferences,
        preferences: newPreferences,
    };
};

export const loadUserPreferencesFinished = (preferences: UserPreferences): LoadUserPreferencesFinished => {
    return {
        type: UserActionType.loadUserPreferencesFinished,
        preferences,
    };
};

export const loadUserDepartmentFeatures = (): LoadUserDepartmentFeatures => {
    return {
        type: UserActionType.loadUserDepartmentFeatures,
    };
};

export const loadUserDepartmentFeaturesFinished = (features: DepartmentFeatures): LoadUserDepartmentFeaturesFinished => {
    return {
        type: UserActionType.loadUserDepartmentFeaturesFinished,
        features,
    };
};

export type UserActions =
    LoadUser | LoadUserFinished | LoadUserEmployeeFinished
    | LoadUserEmployeePermissions | LoadUserEmployeePermissionsFinished
    | LoadUserPreferences | UpdateUserPreferences | LoadUserPreferencesFinished
    | LoadUserDepartmentFeatures | LoadUserDepartmentFeaturesFinished;
