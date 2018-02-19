import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { EmployeeMap } from '../organization/employees.reducer';

export interface NavigatePeopleDepartment {
    type: 'NAVIGATE-PEOPLE-DEPARTMENT';
}

export const navigatePeopleDepartment = (): NavigatePeopleDepartment => ({ type: 'NAVIGATE-PEOPLE-DEPARTMENT' });

export interface NavigatePeopleRoom {
    type: 'NAVIGATE-PEOPLE-ROOM';
}

export const navigatePeopleRoom = (): NavigatePeopleRoom => ({ type: 'NAVIGATE-PEOPLE-ROOM' });

export interface NavigatePeopleCompany {
    type: 'NAVIGATE-PEOPLE-COMPANY';
}

export const navigatePeopleCompany = (): NavigatePeopleCompany => ({ type: 'NAVIGATE-PEOPLE-COMPANY' });

export interface LoadUserDepartmentEmployeesFinished {
    type: 'LOAD-USER-DEPARTMENT-EMPLOYEES-FINISHED';
    employee: Employee;
}

export const loadUserDepartmentEmployeesFinished = (employee: Employee): LoadUserDepartmentEmployeesFinished => ({ type: 'LOAD-USER-DEPARTMENT-EMPLOYEES-FINISHED', employee });

export type PeopleActions = LoadUserDepartmentEmployeesFinished | NavigatePeopleDepartment | NavigatePeopleRoom | NavigatePeopleCompany;
