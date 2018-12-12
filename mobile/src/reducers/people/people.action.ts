import { Action } from 'redux';

export interface SelectCompanyDepartment extends Action {
    type: 'SELECT-COMPANY-DEPARTMENT';
    departmentId: string;
}

export const selectCompanyDepartment = (departmentId: string): SelectCompanyDepartment =>
    ({ type: 'SELECT-COMPANY-DEPARTMENT', departmentId });

export type PeopleActions = SelectCompanyDepartment;
