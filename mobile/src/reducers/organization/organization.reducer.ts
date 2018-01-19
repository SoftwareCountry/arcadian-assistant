import { Department } from './department.model';
import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { OrganizationState } from './organization.reducer';
import { departmentsReducer } from './departments.reducer';
import { employeesReducer, EmployeesStore } from './employees.reducer';
import { loadDepartmentsEpic$, loadChiefsEpic$, loadDepartmentsFinishedEpic$, loadEmployeesForDepartmentEpic$ } from './organization.epics';
import { errorLoadFailedEpic$ } from '../errors/errors.epics';

export interface OrganizationState {
    departments: Department[];
    employees: EmployeesStore;
}

export const organizationEpics = combineEpics(
    loadDepartmentsEpic$ as any,
    loadChiefsEpic$ as any,
    loadDepartmentsFinishedEpic$ as any,
    loadEmployeesForDepartmentEpic$ as any,
    errorLoadFailedEpic$ as any);

export const organizationReducer = combineReducers<OrganizationState>({
    departments: departmentsReducer,
    employees: employeesReducer
});