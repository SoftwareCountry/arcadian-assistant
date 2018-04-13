export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    departmentId: string;
}

export const updateDepartmentsBranch = (departmentId: string): UpdateDepartmentsBranch => ({ type: 'UPDATE-DEPARTMENTS-BRANCH', departmentId });

export type PeopleActions = UpdateDepartmentsBranch;
