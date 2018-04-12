import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';
import { ActionsObservable } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment, loadEmployeesForRoom, LoadDepartmentsFinished, LoadEmployeeFinished } from '../organization/organization.action';
import { PeopleActions } from './people.action';
import { Department } from '../organization/department.model';

export interface PeopleState {
    headDepartment: Department;
    departments: Department[];
    departmentsHeads: Employee[];
    employees: Employee[];
    departmentsHeadsIds: string[];
    departmentsBranch: Department[];
}

const initState: PeopleState = {
    headDepartment: null,
    departments: [],
    departmentsHeads: [],
    employees: [],
    departmentsHeadsIds: [],
    departmentsBranch: []
};

function onlyUnique(value: string, index: number, self: string[]) { 
    return self.indexOf(value) === index;
}

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | LoadEmployeeFinished | LoadDepartmentsFinished) => {
    switch (action.type) {
        case 'LOAD-DEPARTMENTS-FINISHED':
            const headsIds = action.departments.map(department => department.chiefId).filter(onlyUnique);
            const headDepartment = action.departments.find((department) => department.isHeadDepartment === true);
            return {...state, departments: action.departments, headDepartment: headDepartment, departmentsHeadsIds: headsIds};        
        case 'UPDATE-DEPARTMENTS-BRANCH':
            let deps: Department[] = [];
            let currentDepartment = state.departments.find(department => department.departmentId === action.departmentId);

            while (currentDepartment) {
                deps.push(currentDepartment);
                const parent = state.departments.find(department => department.departmentId === currentDepartment.parentDepartmentId) != null ? state.departments.find(department => department.departmentId === currentDepartment.parentDepartmentId) : null;
                currentDepartment = parent;
            }

            deps.reverse();

            currentDepartment = state.departments.find(department => department.parentDepartmentId === action.departmentId);

            while (currentDepartment) {
                deps.push(currentDepartment);
                const child = state.departments.find(department => department.parentDepartmentId === currentDepartment.departmentId) != null ? state.departments.find(department => department.parentDepartmentId === currentDepartment.departmentId) : null;
                currentDepartment = child;
            }

            return {...state, departmentsBranch: deps};
        default:
            return state;
    }
};