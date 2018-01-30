import { Action } from 'redux';
import { Department } from './department.model';
import { Employee } from './employee.model';
import { Feed } from './feed.model';

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
//----------
export interface LoadFeeds extends Action {
    type: 'LOAD_FEEDS';
}

export const loadFeeds = (): LoadFeeds => ({ type: 'LOAD_FEEDS' });

export interface LoadFeedsFinished extends Action {
    type: 'LOAD_FEEDS_FINISHED';
    feeds: Feed[];
}

export const loadFeedsFinished = (feeds: Feed[]): LoadFeedsFinished => {
    if (feeds && feeds.length === 1) {
        //generate multiple sample feeds
        const feed = feeds[0];
        feeds = Array.apply(0, Array(10)).map(function (f: void, i: number) { return Object.assign({}, feed, { messageId: i, title: `${feed.title} ${i}` }); });
    }
    return { type: 'LOAD_FEEDS_FINISHED', feeds };
};
//----------
export type OrganizationActions =
    LoadDepartments | LoadDepartmentsFinished |
    LoadEmployee | LoadEmployeesForDepartment | LoadEmployeeFinished |
    LoadFeeds | LoadFeedsFinished;