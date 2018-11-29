import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { Reducer } from 'redux';
import { OrganizationActions } from '../organization/organization.action';
import { UserActions } from './user.action';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { Map } from 'immutable';
import { Nullable } from 'types';

export interface UserInfoState {
    employeeId: Nullable<string>;
    permissions: Map<string, UserEmployeePermissions>;
}

const initState: UserInfoState = {
    employeeId: null,
    permissions: Map<string, UserEmployeePermissions>()
};

export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return {
                ...state,
                employeeId: action.userEmployeeId
            };
        case 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED':
            const existingPermissions = state.permissions.get(action.permissions.employeeId);

            if (!existingPermissions || !existingPermissions.equals(action.permissions)) {
                return {
                    ...state,
                    permissions: state.permissions.set(action.permissions.employeeId, action.permissions)
                };
            }

            return state;
        default:
            return state;
    }
};
