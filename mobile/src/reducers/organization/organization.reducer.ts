import { Department } from './department.model';
import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { OrganizationState } from './organization.reducer';
import { departmentsReducer } from './departments.reducer';
import { employeesReducer, EmployeesStore } from './employees.reducer';
import { loadDepartmentsEpic$, loadChiefsEpic$, loadEmployeesForDepartmentEpic$, loadEmployeeEpic$, loadEmployeesForUserDepartmentEpic$, loadEmployeesForUserRoomEpic$, loadEmployeesForRoomEpic$ } from './organization.epics';
import { Employee } from './employee.model';

export interface OrganizationState {
    departments: Department[];
    employees: EmployeesStore;
}

export const organizationEpics = combineEpics(
    loadEmployeeEpic$ as any,
    loadDepartmentsEpic$ as any,
    loadChiefsEpic$ as any,
    loadEmployeesForDepartmentEpic$ as any,
    loadEmployeesForRoomEpic$ as any,
    loadEmployeesForUserDepartmentEpic$ as any, 
    loadEmployeesForUserRoomEpic$ as any
);

export const organizationReducer = combineReducers<OrganizationState>({
    departments: departmentsReducer,
    employees: employeesReducer
});