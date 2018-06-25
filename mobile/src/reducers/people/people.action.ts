export interface UpdateDepartmentsBranch {
    type: 'UPDATE-DEPARTMENTS-BRANCH';
    departmentId: string;
    focusOnEmployeesList?: boolean;
}

export const updateDepartmentsBranch = (departmentId: string, focusOnEmployeesList?: boolean): UpdateDepartmentsBranch => ({ type: 'UPDATE-DEPARTMENTS-BRANCH', departmentId, focusOnEmployeesList });

export interface SetPeopleFilter {
    type: 'SEARCH_PEOPLE';
    filter: string;
}

export const setPeopleFilter = (filter: string): SetPeopleFilter => {
    return  { type: 'SEARCH_PEOPLE', filter };
};

export const endPeopleSearch = (): SetPeopleFilter => {
    return  { type: 'SEARCH_PEOPLE', filter: '' };
};

export type PeopleActions = UpdateDepartmentsBranch | SetPeopleFilter;
