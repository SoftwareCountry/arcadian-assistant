import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';
import { ActionsObservable } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment, loadEmployeesForRoom, LoadDepartmentsFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { SearchActions } from '../search.action';
import { SearchType } from '../../navigation/search-view';
import { Department } from '../organization/department.model';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { DepartmentsListStateDescriptor } from '../../people/departments/departments-horizontal-scrollable-list';

export interface PeopleState {
    departments: Department[];
    departmentsBranch: Department[];
    currentFocusedDepartmentId: string;
    departmentsLists: DepartmentsListStateDescriptor[];
    filter: string;
}

const initState: PeopleState = {
    departments: [],
    departmentsBranch: [],
    currentFocusedDepartmentId: null,
    departmentsLists: [],
    filter: '',
};

function onlyUnique(value: string, index: number, self: string[]) { 
    return self.indexOf(value) === index;
}

export function updateTopOfBranch(depId: string, departments: Department[]) {
    const deps: Department[] = [];
    const depsLists: DepartmentsListStateDescriptor[] = [];
    const curId = depId;

    // recalculate current pages if number of departments was changed
    let department = departments.find(d => d.departmentId === curId);
    while (department) {
        deps.push(department);
        const parent = departments.find(d => d.departmentId === department.parentDepartmentId);
        if (parent !== null) {
            let children = departments.filter(d => d.parentDepartmentId === department.parentDepartmentId);
            depsLists.push({currentPage: children.indexOf(department)});
        }
        department = parent;
    }
    deps.reverse();
    depsLists.reverse();

    return {departmentsLineup: deps, departmentsLists: depsLists};
}

function departmentsBranchFromDepartmentWithId(departmentId: string, departments: Department[]) {
    const deps: Department[] = [];
    const depsLists: DepartmentsListStateDescriptor[] = [];

    // find from cur to the top
    let department = departments.find(d => d.departmentId === departmentId);
    while (department) {
        deps.push(department);
        
        const parent = departments.find(d => d.departmentId === department.parentDepartmentId);

        if (parent !== null) {
            let children = departments.filter(d => d.parentDepartmentId === department.parentDepartmentId);
            depsLists.push({currentPage: children.indexOf(department)});
        }

        department = parent;
    }

    deps.reverse();
    depsLists.push({currentPage: 0});
    depsLists.reverse();

    // find from cur to the leaves
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
        case 'UPDATE-DEPARTMENTS-BRANCH': {
            return {...state, departmentsBranch: action.departments, departmentsLists: action.departmentLists};
        }
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