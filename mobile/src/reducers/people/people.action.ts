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

export interface LoadUserDepartmentEmployessFinished {
    type: 'LOAD-USER-DEPARTMENT-EMPOYEES-FINISHED';
    employee: Employee;
}

export const loadUserDepartmentEmployessFinished = (employee: Employee): LoadUserDepartmentEmployessFinished => ({ type: 'LOAD-USER-DEPARTMENT-EMPOYEES-FINISHED', employee });

export type PeopleActions = LoadUserDepartmentEmployessFinished | NavigatePeopleDepartment | NavigatePeopleRoom | NavigatePeopleCompany;
