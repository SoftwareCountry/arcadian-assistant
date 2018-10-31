import { Employee } from '../organization/employee.model';

export interface SelectCompanyDepartment {
    type: 'SELECT-COMPANY-DEPARTMENT';
    departmentId: string;
}

export const selectCompanyDepartment = (departmentId: string): SelectCompanyDepartment => 
    ({ type: 'SELECT-COMPANY-DEPARTMENT', departmentId });

export type PeopleActions = SelectCompanyDepartment;
