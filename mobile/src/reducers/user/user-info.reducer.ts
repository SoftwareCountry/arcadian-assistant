import { User } from './user.model';
import { Employee } from '../organization/employee.model';
import { Reducer } from 'redux';
import { OrganizationActions } from '../organization/organization.action';
import { UserActions } from './user.action';

export interface UserInfoState {
    employeeId: string;
}

const initState: UserInfoState = {
    employeeId: null
};

export const userInfoReducer: Reducer<UserInfoState> = (state = initState, action: UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return {
                employeeId: action.userEmployeeId
            };
        default:
            return state;
    }
};