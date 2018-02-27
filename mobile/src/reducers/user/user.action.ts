import { User } from './user.model';
import { Employee } from '../organization/employee.model';

export interface LoadUser {
    type: 'LOAD-USER';
}

export const loadUser = (): LoadUser => ({ type: 'LOAD-USER' });

export interface LoadUserFinished {
    type: 'LOAD-USER-FINISHED';
    user: User;
}

export const loadUserFinished = (user: User): LoadUserFinished => ({ type: 'LOAD-USER-FINISHED', user });

export interface LoadUserEmployeeFinished {
    type: 'LOAD-USER-EMPLOYEE-FINISHED';
    employee: Employee;
}

export const loadUserEmployeeFinished = (employee: Employee): LoadUserEmployeeFinished => ({ type: 'LOAD-USER-EMPLOYEE-FINISHED', employee });

export type UserActions = LoadUser | LoadUserFinished | LoadUserEmployeeFinished;