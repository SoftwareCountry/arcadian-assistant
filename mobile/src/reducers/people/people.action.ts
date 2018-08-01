import { Action } from 'redux';
import { Department } from './department.model';

export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    departmentId: string;
    focusOnEmployeesList?: boolean;
}

export const updateDepartmentsBranch = (departmentId: string, focusOnEmployeesList?: boolean): UpdateDepartmentsBranch => ({ type: 'UPDATE-DEPARTMENTS-BRANCH', departmentId, focusOnEmployeesList });

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

export type PeopleActions = UpdateDepartmentsBranch | LoadDepartments | LoadDepartmentsFinished;
