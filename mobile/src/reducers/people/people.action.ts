import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { EmployeeMap } from '../organization/employees.reducer';
import { DepartmentsTreeNode } from '../../people/departments/departments-tree-node';

export interface UpdateDepartmentIdsTree {
    type: 'UPDATE-DEPARTMENT-IDS-TREE';
    index: number;
    departmentId: DepartmentsTreeNode;
}

export const updateDepartmentIdsTree = (index: number, departmentId: DepartmentsTreeNode): UpdateDepartmentIdsTree => ({ type: 'UPDATE-DEPARTMENT-IDS-TREE', index, departmentId });

export type PeopleActions = UpdateDepartmentIdsTree;
