import { Department } from '../organization/department.model';
import { DepartmentsListStateDescriptor } from '../../people/departments/departments-horizontal-scrollable-list';

export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    departments: Department[];
    departmentLists: DepartmentsListStateDescriptor[];
    filteredDepartments: Department[];
}

export const updateDepartmentsBranch = (departments: Department[], departmentLists: DepartmentsListStateDescriptor[], 
                                        filteredDepartments: Department[]): UpdateDepartmentsBranch => 
    ({ type: 'UPDATE-DEPARTMENTS-BRANCH', departments, departmentLists, filteredDepartments });

export type PeopleActions = UpdateDepartmentsBranch;
