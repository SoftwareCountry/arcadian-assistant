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
    employeesDepartmentSubsetFilterCallback: any;
    employeesRoomSubsetFilterCallback: any;
    employeesSubsetFilterCallback: any;
}

const initState: PeopleState = {
    employees: Map(),
    employeesDepartmentSubsetFilterCallback: function(employee: Employee) {
        return employee.employeeId === '';
    },
    employeesRoomSubsetFilterCallback: function(employee: Employee) {
        return employee.employeeId === '';
    },
    employeesSubsetFilterCallback: function(employee: Employee) {
        return employee.employeeId === '';
    }
};

export const peopleEpics = combineEpics(
    loadUserDepartmentEmployeesEpic$ as any);

export const peopleReducer: Reducer<PeopleState> = (state = initState, action: PeopleActions) => {
    switch (action.type) {
        case 'NAVIGATE-PEOPLE-DEPARTMENT':
            return {...state, employeesDepartmentSubsetFilterCallback: function(employee: Employee) {
                return employee.departmentId === action.departmentId;
            }};
        case 'NAVIGATE-PEOPLE-ROOM':
            return {...state, employeesRoomSubsetFilterCallback: function(employee: Employee) {
                return employee.roomNumber === action.roomNumber;
            }};
        case 'NAVIGATE-PEOPLE-COMPANY':
            return state;
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