import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { EmployeeMap } from '../organization/employees.reducer';

export interface UpdateDepartmentIdsTree {
    type: 'UPDATE-DEPARTMENT-IDS-TREE';
    index: number;
    departmentId: string;
}

export const updateDepartmentIdsTree = (index: number, departmentId: string): UpdateDepartmentIdsTree => ({ type: 'UPDATE-DEPARTMENT-IDS-TREE', index, departmentId });

export type PeopleActions = UpdateDepartmentIdsTree;
