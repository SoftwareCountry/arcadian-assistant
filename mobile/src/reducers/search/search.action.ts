import { Action } from 'redux';
import { SearchType } from '../../navigation/search-view';
import { Department } from '../organization/department.model';
import { DepartmentsListStateDescriptor } from '../../people/departments/departments-horizontal-scrollable-list';

export interface SetFilter extends Action {
    type: 'SEARCH-BY-TEXT-FILTER';
    filter: string;
    searchType: SearchType;
}

export const startSearch = (filter: string, searchType: SearchType): SetFilter => {
    return { type: 'SEARCH-BY-TEXT-FILTER', filter, searchType };
};

export const endSearch = (searchType: SearchType): SetFilter => {
    return { type: 'SEARCH-BY-TEXT-FILTER', filter: '', searchType};
};

export interface FilterDepartmentsFinished extends Action {
    type: 'FILTER-DEPARTMENTS-FINISHED';
    departmentBranch: Department[];
    departmentList: DepartmentsListStateDescriptor[];
    filteredDeps: Department[];
}

export const filterDepartmentsFinished = (filteredDeps: Department[], departmentBranch: Department[],
                                          departmentList: DepartmentsListStateDescriptor[]): FilterDepartmentsFinished => {
    return { type: 'FILTER-DEPARTMENTS-FINISHED', departmentBranch, departmentList, filteredDeps };
};

export type SearchActions = SetFilter | FilterDepartmentsFinished;