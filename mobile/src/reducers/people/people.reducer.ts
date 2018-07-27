import { Reducer } from 'redux';
import { NavigationAction } from 'react-navigation';
import { LoadDepartmentsFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { SearchActions } from '../search/search.action';
import { SearchType } from '../../navigation/search-view';
import { Department } from '../organization/department.model';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { DepartmentsListStateDescriptor } from '../../people/departments/departments-horizontal-scrollable-list';
import { combineEpics } from 'redux-observable';
import { updateDepartmentsBranchEpic$ } from '../search/search.epics';

export interface PeopleState {
    departments: Department[];
    filteredDepartments: Department[];
    departmentsBranch: Department[];
    currentFocusedDepartmentId: string;
    departmentsLists: DepartmentsListStateDescriptor[];
    filter: string;
}

const initState: PeopleState = {
    departments: [],
    filteredDepartments: [],
    departmentsBranch: [],
    currentFocusedDepartmentId: null,
    departmentsLists: [],
    filter: '',
};

export function departmentsBranchFromDepartmentWithId(departmentId: string, departments: Department[]) {
    const deps: Department[] = [];
    const depsLists: DepartmentsListStateDescriptor[] = [];

    let department = departments.find(d => d.departmentId === departmentId);
    while (department) {
        deps.push(department);   
        const parent = departments.find(d => d.departmentId === department.parentDepartmentId);
        if (parent !== null) {
            depsLists.push({currentPage: departments.filter(d => d.parentDepartmentId === department.parentDepartmentId).indexOf(department)});
        }
        department = parent;
    }

    deps.reverse();
    depsLists.reverse();

    department = departments.find(d => d.parentDepartmentId === departmentId);
    
    while (department) {
        deps.push(department);
        depsLists.push({currentPage: departments.filter(d => d.parentDepartmentId === department.parentDepartmentId).indexOf(department)});
        department = departments.find(d => d.parentDepartmentId === department.departmentId);
    }

    return {departmentsLineup: deps, departmentsLists: depsLists};
}

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | NavigationAction | 
        LoadUserEmployeeFinished | LoadDepartmentsFinished | SearchActions) => {
    switch (action.type) {
        case 'Navigation/NAVIGATE':
            if (action.routeName === 'Company') {
                if (action.params === undefined) {
                    const depsAndMeta = departmentsBranchFromDepartmentWithId(state.currentFocusedDepartmentId, state.departments);
                    return {...state, departmentsBranch: depsAndMeta.departmentsLineup, departmentsLists: depsAndMeta.departmentsLists};
                } else {
                    const { departmentId } = action.params;
                    const depsAndMeta = departmentsBranchFromDepartmentWithId(departmentId, state.departments);
                    return {...state, currentFocusedDepartmentId: departmentId, departmentsBranch: depsAndMeta.departmentsLineup, departmentsLists: depsAndMeta.departmentsLists};
                }
            } else {
                return state;
            }
        case 'LOAD-USER-EMPLOYEE-FINISHED': {
            // Init Departments Branch focused on current user's departments
            if (state.departments.length > 0) {
                const depsAndMeta = departmentsBranchFromDepartmentWithId(action.employee.departmentId, state.departments);
                return {
                    ...state, 
                    currentFocusedDepartmentId: action.employee.departmentId, 
                    departmentsBranch: depsAndMeta.departmentsLineup, 
                    departmentsLists: depsAndMeta.departmentsLists
                };
            } else {
                return {
                    ...state,
                    currentFocusedDepartmentId: action.employee.departmentId, 
                };
            }
        }
        case 'LOAD-DEPARTMENTS-FINISHED':
            if (state.currentFocusedDepartmentId) {
                const depsAndMeta = departmentsBranchFromDepartmentWithId(state.currentFocusedDepartmentId, action.departments);
                return {
                    ...state, 
                    departmentsBranch: depsAndMeta.departmentsLineup, 
                    departmentsLists: depsAndMeta.departmentsLists,
                    departments: action.departments
                };
            } else {
                return {...state, departments: action.departments};   
            }     
        case 'UPDATE-DEPARTMENTS-BRANCH':
            return {
                ...state, 
                departmentsBranch: action.departmentsBranch, 
                departmentsLists: action.departmentLists,
            };
        case 'FILTER-DEPARTMENTS-FINISHED':
            return {
                ...state,
                filteredDepartments: action.filteredDeps,
                departmentsBranch: action.departmentBranch,
                departmentsLists: action.departmentList,
            };
        case 'SEARCH-BY-TEXT-FILTER':
            if (action.searchType === SearchType.People) {
                return {
                    ...state,
                    filter: action.filter,
                };
            }
        default:
            return state;
    }
};

export const peopleEpics = combineEpics(
    updateDepartmentsBranchEpic$ as any,
);