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
import { DepartmentsListStateDescriptor } from '../../people/departments/departments-horizontal-scrollable-list';

export interface PeopleState {
    departments: Department[];
    departmentsBranch: Department[];
    currentFocusedDepartmentId: string;
    departmentsLists: DepartmentsListStateDescriptor[];
}

const initState: PeopleState = {
    departments: [],
    departmentsBranch: [],
    currentFocusedDepartmentId: null,
    departmentsLists: []
};

function onlyUnique(value: string, index: number, self: string[]) { 
    return self.indexOf(value) === index;
}

function departmentsBranchFromDepartmentWithId(departmentId: string, departments: Department[], focusOnEmployeesList?: boolean) {
    const deps: Department[] = [];
    const depsLists: DepartmentsListStateDescriptor[] = [];

    let department = departments.find(d => d.departmentId === departmentId);

    while (department) {
        deps.push(department);
        
        const parent = departments.find(d => d.departmentId === department.parentDepartmentId) != null ?
            departments.find(d => d.departmentId === department.parentDepartmentId) : null;

        if (parent !== null) {
            depsLists.push({currentPage: departments.filter(d => d.parentDepartmentId === department.parentDepartmentId).indexOf(department)});
        }

        department = parent;
    }

    deps.reverse();
    // Toppest Head
    depsLists.push({currentPage: 0});
    depsLists.reverse();

    if (focusOnEmployeesList) {
        return {departmentsLineup: deps, departmentsLists: depsLists};
    }

    department = departments.find(d => d.parentDepartmentId === departmentId);
    
    while (department) {
        deps.push(department);
        depsLists.push({currentPage: departments.filter(d => d.parentDepartmentId === department.parentDepartmentId).indexOf(department)});

        const child = departments.find(d => d.parentDepartmentId === department.departmentId) != null ?
            departments.find(d => d.parentDepartmentId === department.departmentId) : null;

        department = child;
    }

    return {departmentsLineup: deps, departmentsLists: depsLists};
}

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | NavigationAction | LoadUserEmployeeFinished | LoadDepartmentsFinished) => {
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
            } else if (action.routeName === 'CurrentProfile') {
                /*
                    {
                        "calendarEventId": "a8cc5361-9b63-46d0-bee1-83d16a8d7f62",
                        "type": "Vacation",
                        "dates": {
                            "startDate": "2018-04-17T21:00:00Z",
                            "endDate": "2018-04-27T21:00:00Z",
                            "startWorkingHour": 0,
                            "finishWorkingHour": 8
                        },
                        "status": "Requested"
                    }
                */
                return state;
            } else {
                return state;
            }
        case 'LOAD-USER-EMPLOYEE-FINISHED': {
            // Init Departments Branch focused on current user's departments
            const depsAndMeta = departmentsBranchFromDepartmentWithId(action.employee.departmentId, state.departments);
            return {...state, currentFocusedDepartmentId: action.employee.departmentId, departmentsBranch: depsAndMeta.departmentsLineup, departmentsLists: depsAndMeta.departmentsLists};
        }
        case 'LOAD-DEPARTMENTS-FINISHED':
            return {...state, departments: action.departments};        
        case 'UPDATE-DEPARTMENTS-BRANCH': {
            const depsAndMeta = departmentsBranchFromDepartmentWithId(action.departmentId, state.departments, action.focusOnEmployeesList);
            return {...state, departmentsBranch: depsAndMeta.departmentsLineup, departmentsLists: depsAndMeta.departmentsLists};
        }
        default:
            return state;
    }
};