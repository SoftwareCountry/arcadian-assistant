import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { Reducer } from 'redux';
import { OrganizationActions } from '../organization/organization.action';
import { UserActions } from './user.action';
import { UserEmployeePermissions } from './user-permissions.model';

export interface UserInfoState {
    employeeId: string;
    permissions: UserEmployeePermissions;
}

const initState: UserInfoState = {
    employeeId: null,
    permissions: null
};

export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return {
                ...state,
                employeeId: action.userEmployeeId
            };
        case 'LOAD-USER-EMPLOYEE-PERMISSIONS-FINISHED':
            return {
                ...state,
                permissions: action.permissions
            };
        default:
            return state;
    }
};