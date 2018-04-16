import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';
import { ActionsObservable } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment, loadEmployeesForRoom, LoadDepartmentsFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { Department } from '../organization/department.model';
import { LoadUserEmployeeFinished } from '../user/user.action';

export interface PeopleState {
    departments: Department[];
    departmentsBranch: Department[];
    currentFocusedDepartmentId: string;
}

const initState: PeopleState = {
    departments: [],
    departmentsBranch: [],
    currentFocusedDepartmentId: null
};

function onlyUnique(value: string, index: number, self: string[]) { 
    return self.indexOf(value) === index;
}

function departmentsBranchFromDepartmentWithId(departmentId: string, departments: Department[]) {
    const deps: Department[] = [];
    let department = departments.find(d => d.departmentId === departmentId);

    while (department) {
        deps.push(department);
        const parent = departments.find(d => d.departmentId === department.parentDepartmentId) != null ?
            departments.find(d => d.departmentId === department.parentDepartmentId) : null;
        department = parent;
    }

    deps.reverse();

    department = departments.find(d => d.parentDepartmentId === departmentId);

    while (department) {
        deps.push(department);
        const child = departments.find(d => d.parentDepartmentId === department.departmentId) != null ?
            departments.find(d => d.parentDepartmentId === department.departmentId) : null;
        department = child;
    }

    return deps;
}

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | NavigationAction | LoadUserEmployeeFinished | LoadDepartmentsFinished) => {
    switch (action.type) {
        case 'Navigation/NAVIGATE':
            if (action.routeName === 'Company') {
                const { departmentId } = action.params;
                return {...state, currentFocusedDepartmentId: departmentId, departmentsBranch: departmentsBranchFromDepartmentWithId(departmentId, state.departments)};
            } else {
                return state;
            }
        case 'LOAD-USER-EMPLOYEE-FINISHED': {
            // Init Departments Branch focused on current user's departments
            return {...state, currentFocusedDepartmentId: action.employee.departmentId, departmentsBranch: departmentsBranchFromDepartmentWithId(action.employee.departmentId, state.departments)};
        }
        case 'LOAD-DEPARTMENTS-FINISHED':
            return {...state, departments: action.departments};        
        case 'UPDATE-DEPARTMENTS-BRANCH': {
            return {...state, departmentsBranch: departmentsBranchFromDepartmentWithId(action.departmentId, state.departments)};
        }
        default:
            return state;
    }
};