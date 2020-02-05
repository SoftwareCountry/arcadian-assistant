/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, ofType, StateObservable } from 'redux-observable';
import {
    LoadAllEmployees,
    LoadDepartments,
    loadDepartments,
    loadDepartmentsFinished,
    LoadDepartmentsFinished,
    LoadEmployees,
    loadEmployees,
    loadEmployeesFinished,
    LoadEmployeesForDepartment,
    loadEmployeesForDepartment,
    LoadEmployeesForRoom,
    loadEmployeesForRoom
} from './organization.action';
import { LoadUserEmployeeFinished, UserActionType } from '../user/user.action';
import { deserialize, deserializeArray } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { AppState, DependenciesContainer } from '../app.reducer';
import { Employee } from './employee.model';
import { handleHttpErrors } from '../../errors/error.operators';
import { filter, groupBy, map, mergeAll, mergeMap, switchMap } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { Set } from 'immutable';
import { NavigationActionType, OpenDepartmentAction } from '../../navigation/navigation.actions';

//----------------------------------------------------------------------------
export const loadEmployeeEpic$ = (action$: ActionsObservable<LoadEmployees>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD_EMPLOYEES'),
    mergeMap(action => {
        const employeeIds = Set(action.employeeIds);
        const requests = employeeIds.toArray().map(employeeId => {
            return deps.apiClient.getJSON<Object>(`/employees/${employeeId}`).pipe(
                map(obj => deserialize(obj, Employee)),
                handleHttpErrors<Employee>(),
            );
        });
        return forkJoin(requests);
    }),
    map(employees => loadEmployeesFinished(employees)),
);

//----------------------------------------------------------------------------
export const loadAllEmployeeEpic$ = (action$: ActionsObservable<LoadAllEmployees>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD_ALL_EMPLOYEES'),
    switchMap(action => {
        return deps.apiClient.getJSON(`/employees/`).pipe(
            map(obj => deserializeArray(obj as any, Employee) as Employee[]),
            map(employees => loadEmployeesFinished(employees)),
            handleHttpErrors(),
        );
    }),
);

//----------------------------------------------------------------------------
export const loadDepartmentsEpic$ = (action$: ActionsObservable<LoadDepartments>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD-DEPARTMENTS'),
    switchMap(() => deps.apiClient.getJSON(`/departments`).pipe(
        map(obj => deserializeArray(obj as any, Department)),
        map(x => loadDepartmentsFinished(x)),
        handleHttpErrors(),
        ),
    ),
);

//----------------------------------------------------------------------------
export const loadChiefsEpic$ = (action$: ActionsObservable<LoadDepartmentsFinished>) => action$.pipe(
    ofType('LOAD-DEPARTMENTS-FINISHED'),
    map(action => action.departments.filter(department => !!department.chiefId)),
    map(departments => departments.map(department => department.chiefId!)),
    map(chiefIds => loadEmployees(chiefIds))
);

//----------------------------------------------------------------------------
export const loadEmployeesForDepartmentEpic$ = (action$: ActionsObservable<LoadEmployeesForDepartment>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD_EMPLOYEES_FOR_DEPARTMENT'),
    groupBy(action => action.departmentId),
    map(x =>
        x.pipe(
            switchMap(() => deps.apiClient.getJSON(`/employees?departmentId=${x.key}`).pipe(
                map(obj => deserializeArray(obj as any, Employee)),
                handleHttpErrors<Employee[]>())),
        )),
    mergeAll<Employee[]>(),
    map(loadEmployeesFinished)
);

//----------------------------------------------------------------------------
export const loadEmployeesForRoomEpic$ = (action$: ActionsObservable<LoadEmployeesForRoom>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD_EMPLOYEES_FOR_ROOM'),
    groupBy(action => action.roomNumber),
    map(x => x.pipe(
        switchMap(() => deps.apiClient.getJSON(`/employees?roomNumber=${x.key}`).pipe(
            map(obj => deserializeArray(obj as any, Employee)),
            handleHttpErrors()
        )),
    )),
    mergeAll(),
    map(loadEmployeesFinished)
);

//----------------------------------------------------------------------------
export const loadEmployeesForUserDepartmentEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) => action$.pipe(
    ofType(UserActionType.loadUserEmployeeFinished),
    map(x => loadEmployeesForDepartment(x.employee.departmentId)),
);

//----------------------------------------------------------------------------
export const loadEmployeesForUserRoomEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) => action$.pipe(
    ofType(UserActionType.loadUserEmployeeFinished),
    filter(action => {
        return action.employee.roomNumber !== null;
    }),
    map(x => loadEmployeesForRoom(x.employee.roomNumber!)),
);

//----------------------------------------------------------------------------
export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType(UserActionType.loadUserEmployeeFinished),
    map(x => loadDepartments()),
);

//----------------------------------------------------------------------------
export const handleDepartmentNavigation$ = (action$: ActionsObservable<OpenDepartmentAction>) => action$.pipe(
    ofType(NavigationActionType.openDepartment),
    map(x => loadEmployeesForDepartment(x.params.departmentId)),
);
