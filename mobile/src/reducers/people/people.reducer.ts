import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { combineEpics } from 'redux-observable';
import { NavigationAction } from 'react-navigation';
import { ActionsObservable } from 'redux-observable';

import { User } from '../user/user.model';
import { Employee } from '../organization/employee.model';
import { OrganizationActions, loadEmployeesForDepartment, loadEmployeesForRoom } from '../organization/organization.action';
import { UserActions, LoadUserEmployeeFinished } from '../user/user.action';
import { EmployeeMap } from '../organization/employees.reducer';

export const loadEmployeesForDepartmentEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .map(x => loadEmployeesForDepartment(x.employee.departmentId));


export const loadEmployeesForRoomEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
        action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
            .map(x => loadEmployeesForRoom(x.employee.roomNumber));
    

export const peopleEpics = combineEpics( loadEmployeesForDepartmentEpic$ as any, loadEmployeesForRoomEpic$ as any);