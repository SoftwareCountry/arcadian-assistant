import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { EmployeeMap } from '../organization/employees.reducer';

export interface LoadUserDepartmentEmployessFinished {
    type: 'LOAD-USER-DEPARTMENT-EMPOYEES-FINISHED';
    employee: Employee;
}

export const loadUserDepartmentEmployessFinished = (employee: Employee): LoadUserDepartmentEmployessFinished => ({ type: 'LOAD-USER-DEPARTMENT-EMPOYEES-FINISHED', employee });

export type PeopleActions = LoadUserDepartmentEmployessFinished;
