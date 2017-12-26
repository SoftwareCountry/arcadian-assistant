import { Reducer } from 'redux';
import { Department } from './department.model';
import { OrganizationActions } from './organization.action';

export const departmentsReducer: Reducer<Department[]> = (state = [], action: OrganizationActions) => {
    switch (action.type) {
        case 'LOAD-DEPARTMENTS-FINISHED':
            return [...action.departments];

        default:
            return state;
    }
};