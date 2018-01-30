import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { Reducer } from 'redux';
import { OrganizationActions } from '../organization/organization.action';
import { UserActions } from './user.action';

export interface UserInfoState {
    user: User;
    employee: Employee;
}

const initState: UserInfoState = {
    user: null,
    employee: null
};

export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions | OrganizationActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return {
                ...state,
                user: action.user
            };
        case 'LOAD_EMPLOYEE_FINISHED':
            if (state.user && state.user.employeeId === action.employee.employeeId) {
                return {
                    ...state,
                    employee: action.employee,
                };
            }
            return state;
        default:
            return state;
    }
};