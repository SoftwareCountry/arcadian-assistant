import { Reducer } from 'redux';
import { OrganizationActions } from './organization.action';
import { User } from './user.model';
import { Employee } from './employee.model';

export interface UserState {
    user: User;
    employee: Employee;
}

const initState: UserState = {
    user: null,
    employee: null
};

export const userReducer: Reducer<UserState> = (state = initState, action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return {
                user: action.user,
                employee: state.employee
            };
        case 'LOAD_EMPLOYEE_FINISHED':
            if (state.user && state.user.employeeId === action.employee.employeeId) {
                return {
                    user: state.user,
                    employee: action.employee,
                };
            }
            return state;
        default:
            return state;
    }
};