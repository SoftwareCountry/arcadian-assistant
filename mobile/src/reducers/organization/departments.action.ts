import { Action } from 'redux';
import { Department } from './department.model';

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

export type DepartmentsActions = LoadDepartments | LoadDepartmentsFinished;