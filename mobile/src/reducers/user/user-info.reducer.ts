import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { Reducer } from 'redux';
import { OrganizationActions } from '../organization/organization.action';
import { UserActions } from './user.action';

export interface UserInfoState {
    employee: Employee;
}

const initState: UserInfoState = {
    employee: null
};

export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-EMPLOYEE-FINISHED':
            return {
                employee: action.employee
            };
        default:
            return state;
    }
};