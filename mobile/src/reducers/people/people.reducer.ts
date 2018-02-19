import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment } from '../organization/organization.action';
import { UserActions } from '../user/user.action';
import { PeopleActions } from './people.action';
import { loadUserDepartmentEmployeesEpic$ } from './people.epics';
import { EmployeeMap } from '../organization/employees.reducer';

export interface PeopleState {
    employees: EmployeeMap;
    employeesSubsetFilter: Function;
}

const initState: PeopleState = {
    employees: Map(),
    employeesSubsetFilter: () => {}
};

export const peopleEpics = combineEpics(
    loadUserDepartmentEmployeesEpic$ as any);

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions) => {
    console.log(action.type);
    switch (action.type) {
        case 'NAVIGATE-PEOPLE-DEPARTMENT':
            return {...state, employeesSubsetFilter: () => { console.log('department filter'); }};
            // break;
        case 'NAVIGATE-PEOPLE-ROOM':
            return {...state, employeesSubsetFilter: () => { console.log('room filter'); }};
            // break;
        case 'NAVIGATE-PEOPLE-COMPANY':
            return {...state, employeesSubsetFilter: () => { console.log('company filter'); }};
            // break;
        case 'LOAD-USER-DEPARTMENT-EMPLOYEES-FINISHED':
            let { employees } = state;

            return {
                ...state,
                employees: employees.set(action.employee.employeeId, action.employee)
            };
        default:
            return state;
    }
};