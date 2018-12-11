import { Reducer } from 'redux';
import { UserActions } from './user.action';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { Map } from 'immutable';
import { Nullable } from 'types';
import { UserPreferences } from './user-preferences.model';

export interface UserInfoState {
    employeeId: Nullable<string>;
    permissions: Map<string, UserEmployeePermissions>;
    preferences: Nullable<UserPreferences>;
}

const initState: UserInfoState = {
    employeeId: null,
    permissions: Map<string, UserEmployeePermissions>(),
    preferences: null,
};

export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return {
                ...state,
                employeeId: action.userEmployeeId
            };
        case 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED': {
            const existingPermissions = state.permissions.get(action.permissions.employeeId);

            if (!existingPermissions || !existingPermissions.equals(action.permissions)) {
                return {
                    ...state,
                    permissions: state.permissions.set(action.permissions.employeeId, action.permissions)
                };
            }

            return state;
        }
        case 'LOAD-USER-PREFERENCES-FINISHED': {
            const existingPreferences = state.preferences;

            if (!existingPreferences || !existingPreferences.equals(action.preferences)) {
                return {
                    ...state,
                    preferences: action.preferences
                };
            }

            return state;
        }
        default:
            return state;
    }
};
