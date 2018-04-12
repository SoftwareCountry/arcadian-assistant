export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    index: number;
    departmentId: string;
}

export const updateDepartmentsBranch = (index: number, departmentId: string): UpdateDepartmentsBranch => ({ type: 'UPDATE-DEPARTMENTS-BRANCH', index, departmentId });

export type PeopleActions = UpdateDepartmentsBranch;
