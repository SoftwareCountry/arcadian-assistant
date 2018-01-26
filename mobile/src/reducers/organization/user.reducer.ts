import { Reducer } from 'redux';
import { OrganizationActions } from './organization.action';
import { User } from './user.model';
import { Employee } from './employee.model';

export const userReducer: Reducer<User> = (state = null, action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return action.user;

        default:
            return state;
    }
};