import { Department } from '../organization/department.model';
import { DepartmentsListStateDescriptor } from '../../people/departments/departments-horizontal-scrollable-list';

export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    departments: Department[];
    departmentLists: DepartmentsListStateDescriptor[];
}

export const updateDepartmentsBranch = (departments: Department[], departmentLists: DepartmentsListStateDescriptor[]): UpdateDepartmentsBranch => 
    ({ type: 'UPDATE-DEPARTMENTS-BRANCH', departments, departmentLists });

export type PeopleActions = UpdateDepartmentsBranch;
