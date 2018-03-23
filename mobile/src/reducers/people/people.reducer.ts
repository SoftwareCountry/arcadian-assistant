import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';
import { ActionsObservable } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment, loadEmployeesForRoom } from '../organization/organization.action';
import { UserActions } from '../user/user.action';
import { PeopleActions } from './people.action';
import { EmployeeMap } from '../organization/employees.reducer';

export interface PeopleState {
    departmentId: string;
    departmentIdsBranch: string[];
}

const initState: PeopleState = {
    departmentId: null,
    departmentIdsBranch: null
};

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions | NavigationAction) => {
    switch (action.type) {
        case 'UPDATE-DEPARTMENT-IDS-TREE':
            var depIds = state.departmentIdsBranch;
            if (depIds === null) {
                depIds = [''];
            }

            if (depIds.length - 1 < action.index) {
                depIds.push(action.departmentId);
            } else {
                depIds[action.index] = action.departmentId;
                depIds = depIds.slice(0, action.index + 1);
            }
            return {...state, departmentIdsBranch: depIds};
        default:
            return state;
    }
};