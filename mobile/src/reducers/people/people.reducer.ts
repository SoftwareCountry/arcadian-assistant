import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions } from '../organization/organization.action';
import { UserActions } from '../user/user.action';
import { PeopleActions } from './people.action';
import { loadUserDepartmentEmployeesEpic$ } from './people.epics';
import { EmployeeMap } from '../organization/employees.reducer';

export interface PeopleState {
    employees: EmployeeMap;
}

const initState: PeopleState = {
    employees: Map()
};

export const peopleEpics = combineEpics(
    loadUserDepartmentEmployeesEpic$ as any);

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions) => {
    switch (action.type) {
        case 'LOAD-USER-DEPARTMENT-EMPOYEES-FINISHED':
            let { employees } = state;

            return {
                ...state,
                employees: employees.set(action.employee.employeeId, action.employee)
            };
        default:
            return state;
    }
};