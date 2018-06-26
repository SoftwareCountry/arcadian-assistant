export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    departmentId: string;
    focusOnEmployeesList?: boolean;
}

export const updateDepartmentsBranch = (departmentId: string, focusOnEmployeesList?: boolean): UpdateDepartmentsBranch => ({ type: 'UPDATE-DEPARTMENTS-BRANCH', departmentId, focusOnEmployeesList });

export type PeopleActions = UpdateDepartmentsBranch;
