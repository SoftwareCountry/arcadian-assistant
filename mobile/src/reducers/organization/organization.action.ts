import { Action } from 'redux';
import { Department } from './department.model';
import { Employee } from './employee.model';

export interface LoadDepartments extends Action {
    type: 'LOAD-DEPARTMENTS';
}

export const loadDepartments = (): LoadDepartments => ({ type: 'LOAD-DEPARTMENTS' });

export interface LoadDepartmentsFinished extends Action {
    type: 'LOAD-DEPARTMENTS-FINISHED';
    departments: Department[];
}

export const loadDepartmentsFinished = (departments: Department[]): LoadDepartmentsFinished =>
    ({ type: 'LOAD-DEPARTMENTS-FINISHED', departments });

export interface LoadEmployees extends Action {
    type: 'LOAD_EMPLOYEES';
    employeeIds: string[];
}

export const loadEmployees = (employeeIds: string[]): LoadEmployees => ({ type: 'LOAD_EMPLOYEES', employeeIds });

export interface LoadAllEmployees extends Action {
    type: 'LOAD_ALL_EMPLOYEES';
}

export const loadAllEmployees = (): LoadAllEmployees => ({ type: 'LOAD_ALL_EMPLOYEES' });

export interface LoadEmployeesFinished extends Action {
    type: 'LOAD_EMPLOYEES_FINISHED';
    employees: Employee[];
}

export const loadEmployeesFinished = (employees: Employee[]): LoadEmployeesFinished => ({
    type: 'LOAD_EMPLOYEES_FINISHED',
    employees
});

export interface LoadEmployeesForDepartment extends Action {
    type: 'LOAD_EMPLOYEES_FOR_DEPARTMENT';
    departmentId: string;
}

export const loadEmployeesForDepartment = (departmentId: string): LoadEmployeesForDepartment => ({
    type: 'LOAD_EMPLOYEES_FOR_DEPARTMENT',
    departmentId
});

export interface LoadEmployeesForRoom extends Action {
    type: 'LOAD_EMPLOYEES_FOR_ROOM';
    roomNumber: string;
}

export const loadEmployeesForRoom = (roomNumber: string): LoadEmployeesForRoom => ({
    type: 'LOAD_EMPLOYEES_FOR_ROOM',
    roomNumber
});

export type OrganizationActions =
    LoadDepartments | LoadDepartmentsFinished |
    LoadEmployees | LoadAllEmployees |
    LoadEmployeesForDepartment | LoadEmployeesForRoom | LoadEmployeesFinished;
