import { Department } from './department.model';
import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { OrganizationState } from './organization.reducer';
import { loadDepartmentsEpic$, departmentsReducer } from './departments.reducer';

export interface OrganizationState {
    departments: Department[];
}

export const organizationEpics = combineEpics( loadDepartmentsEpic$ );

export const organizationReducer = combineReducers<OrganizationState>({
    departments: departmentsReducer,
});