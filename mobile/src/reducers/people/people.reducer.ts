import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';
import { ActionsObservable } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment, loadEmployeesForRoom } from '../organization/organization.action';
import { UserActions } from '../user/user.action';
import { PeopleActions, RequestEmployeesForDepartment } from './people.action';
import { EmployeeMap } from '../organization/employees.reducer';

export interface PeopleState {
    departmentId: string;
    departmentIdsBranch: [string];
}

const initState: PeopleState = {
    departmentId: null,
    departmentIdsBranch: null
};

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | NavigationAction) => {
    switch (action.type) {
        case 'REQUEST-EMPLOYEES-FOR-DEPARTMENT':
            return state;
        case 'UPDATE-DEPARTMENT-IDS-TREE':
            var depIds = state.departmentIdsBranch;
            if (depIds === null) {
                depIds = [''];
            }

            if (depIds.length - 1 < action.index) {
                depIds.push(action.departmentId);
            } else {
                depIds[action.index] = action.departmentId;
            }
            return {...state, departmentIdsBranch: depIds};
        default:
            return state;
    }
};

export const requestEmployeesForDepartmentEpic$ = (action$: ActionsObservable<RequestEmployeesForDepartment>) =>
    action$.ofType('REQUEST-EMPLOYEES-FOR-DEPARTMENT')
        .map(x => loadEmployeesForDepartment(x.departmentId));

export const peopleEpics = combineEpics( requestEmployeesForDepartmentEpic$ as any);