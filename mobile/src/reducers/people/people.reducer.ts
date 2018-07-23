import { Reducer } from 'redux';
import { NavigationAction } from 'react-navigation';
import { LoadDepartmentsFinished } from '../organization/organization.action';
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

    return { departmentsLineup: deps, departmentsLists: depsLists };
}

export function updateLeaves(depsBranch: Department[], depsLists: DepartmentsListStateDescriptor[], 
                             treeLevel: number, depId: string, departments: Department[]) {
    for (let i = depsBranch.length - 1; i >= treeLevel; i--) {
        depsBranch.pop();
        depsLists.pop();
    }

    let department = departments.find(d => d.departmentId === depId);
    while (department) {
        depsBranch.push(department);
        depsLists.push({currentPage: departments.filter(d => d.parentDepartmentId === department.parentDepartmentId).indexOf(department)});

        department = departments.find(d => d.parentDepartmentId === department.departmentId);
    }
    return { departmentsLineup: depsBranch, departmentsLists: depsLists };
}

function departmentsBranchFromDepartmentWithId(departmentId: string, departments: Department[]) {
    const res = updateTopOfBranch(departmentId, departments);
    const leaves = updateLeaves(res.departmentsLineup, res.departmentsLists, res.departmentsLineup.length - 1,
                                departmentId, departments);
    return { departmentsLineup: leaves.departmentsLineup, departmentsLists: leaves.departmentsLists };
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