import { Department } from '../organization/department.model';
import { DepartmentsListStateDescriptor } from '../../reducers/people/people.reducer';
import { departmentsBranchFromDepartmentWithId } from './people.reducer';

export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    departmentsBranch: Department[];
    departmentLists: DepartmentsListStateDescriptor[];
}

export const updateDepartmentsBranch = (departments: Department[], depId: string): UpdateDepartmentsBranch => {
    const res = departmentsBranchFromDepartmentWithId(depId, departments);
    return ({ type: 'UPDATE-DEPARTMENTS-BRANCH', departmentsBranch: res.departmentsLineup, departmentLists: res.departmentsLists });
};

export type PeopleActions = UpdateDepartmentsBranch;
