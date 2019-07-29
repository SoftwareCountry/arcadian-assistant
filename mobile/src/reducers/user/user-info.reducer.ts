/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Reducer } from 'redux';
import { UserActions, UserActionType } from './user.action';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { Map } from 'immutable';
import { Nullable, Optional } from 'types';
import { UserPreferences } from './user-preferences.model';
import { DepartmentFeatures } from './department-features.model';

//============================================================================
export interface UserInfoState {
    employeeId: Nullable<string>;
    permissions: Map<string, UserEmployeePermissions>;
    preferences: Nullable<UserPreferences>;
    userDepartmentFeatures: Optional<DepartmentFeatures>;
}

const initState: UserInfoState = {
    employeeId: null,
    permissions: Map<string, UserEmployeePermissions>(),
    preferences: null,
    userDepartmentFeatures: undefined,
};

//----------------------------------------------------------------------------
export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions) => {
    switch (action.type) {

        case UserActionType.loadUserFinished:
            return {
                ...state,
                employeeId: action.userEmployeeId
            };

        case UserActionType.loadUserEmployeePermissionsFinished:
            const existingPermissions = state.permissions.get(action.permissions.employeeId);

            if (!existingPermissions || !existingPermissions.equals(action.permissions)) {
                return {
                    ...state,
                    permissions: state.permissions.set(action.permissions.employeeId, action.permissions)
                };
            }

            return state;

        case UserActionType.updateUserPreferences:
        case UserActionType.loadUserPreferencesFinished:
            const existingPreferences = state.preferences;

            if (!existingPreferences || !existingPreferences.equals(action.preferences)) {
                return {
                    ...state,
                    preferences: action.preferences
                };
            }

            return state;

        case UserActionType.loadUserDepartmentFeaturesFinished:
            const existingDepartmentFeatures = state.userDepartmentFeatures;

            if (!existingDepartmentFeatures || !existingDepartmentFeatures.equals(action.features)) {
                return {
                    ...state,
                    userDepartmentFeatures: action.features,
                };
            }

            return state;

        default:
            return state;
    }
};
