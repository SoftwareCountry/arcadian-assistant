import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { EmployeeMap } from '../organization/employees.reducer';

export interface RequestEmployeesForDepartment {
    type: 'REQUEST-EMPLOYEES-FOR-DEPARTMENT';
    departmentId: string;
}

export const requestEmployeesForDepartment = (departmentId: string): RequestEmployeesForDepartment => ({ type: 'REQUEST-EMPLOYEES-FOR-DEPARTMENT', departmentId });

export type PeopleActions = RequestEmployeesForDepartment;
