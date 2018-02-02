import { Department } from './department.model';
import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { OrganizationState } from './organization.reducer';
import { departmentsReducer } from './departments.reducer';
import { employeesReducer, EmployeesStore } from './employees.reducer';
import { feedsReducer } from './feeds.reducer';
import { loadDepartmentsEpic$, loadChiefsEpic$, loadDepartmentsFinishedEpic$, loadEmployeesForDepartmentEpic$, loadFeedsEpic$, loadFeedsFinishedEpic$, loadEmployeeEpic$ } from './organization.epics';
import { Feed } from './feed.model';
import { Employee } from './employee.model';

export interface OrganizationState {
    departments: Department[];
    employees: EmployeesStore;
    feeds: Feed[];
}

export const organizationEpics = combineEpics(
    loadEmployeeEpic$ as any,
    loadDepartmentsEpic$ as any,
    loadChiefsEpic$ as any,
    loadDepartmentsFinishedEpic$ as any,
    loadEmployeesForDepartmentEpic$ as any,
    loadFeedsEpic$ as any,
    loadFeedsFinishedEpic$ as any
);

export const organizationReducer = combineReducers<OrganizationState>({
    departments: departmentsReducer,
    employees: employeesReducer,
    feeds: feedsReducer
});