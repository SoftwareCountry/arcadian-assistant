import { Action } from 'redux';
import { Department } from './department.model';
import { Employee, Photo } from './employee.model';

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

export interface LoadEmployee extends Action {
    type: 'LOAD_EMPLOYEE';
    employeeId: string;
}

export const loadEmployee = (employeeId: string): LoadEmployee => ({ type: 'LOAD_EMPLOYEE', employeeId });

export interface LoadEmployeeFinished extends Action {
    type: 'LOAD_EMPLOYEE_FINISHED';
    employee: Employee;
}

export const loadEmployeeFinished = (employee: Employee): LoadEmployeeFinished => ({ type: 'LOAD_EMPLOYEE_FINISHED', employee });

export interface LoadEmployeesForDepartment extends Action {
    type: 'LOAD_EMPLOYEES_FOR_DEPARTMENT';
    departmentId: string;
}

export const loadEmployeesForDepartment = (departmentId: string): LoadEmployeesForDepartment => ({ type: 'LOAD_EMPLOYEES_FOR_DEPARTMENT', departmentId });

export interface LoadEmployeesForRoom extends Action {
    type: 'LOAD_EMPLOYEES_FOR_ROOM';
    roomNumber: string;
}

export const loadEmployeesForRoom = (roomNumber: string): LoadEmployeesForRoom => ({ type: 'LOAD_EMPLOYEES_FOR_ROOM', roomNumber });

export interface LoadPhoto extends Action {
    type: 'LOAD_PHOTO';
    employeeId: string;
}

export const loadPhoto = (employeeId: string): LoadPhoto => ({ type: 'LOAD_PHOTO', employeeId });

export interface LoadPhotoFinished extends Action {
    type: 'LOAD_PHOTO_FINISHED';
    id: string, 
    photo: Photo;
}

export const loadPhotoFinished = (photo: Photo, id: string): LoadPhotoFinished => ({ type: 'LOAD_PHOTO_FINISHED', photo, id });

export type OrganizationActions =
    LoadDepartments | LoadDepartmentsFinished |
    LoadEmployee | LoadEmployeeFinished | LoadEmployeesForDepartment | LoadEmployeesForRoom |
    LoadPhoto | LoadPhotoFinished;