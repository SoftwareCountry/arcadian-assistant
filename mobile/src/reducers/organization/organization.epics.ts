import { ActionsObservable, ofType, StateObservable } from 'redux-observable';
import {
    LoadDepartments, loadDepartmentsFinished, LoadDepartmentsFinished, loadDepartments,
    loadEmployeesFinished, LoadEmployeesForDepartment, LoadEmployeesForRoom, loadEmployeesForDepartment,
    LoadEmployees, loadEmployees, loadEmployeesForRoom
} from './organization.action';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { AppState, DependenciesContainer } from '../app.reducer';
import { Employee } from './employee.model';
import { loadFailedError } from '../errors/errors.action';
import { handleHttpErrors } from '../errors/errors.epics';
import { catchError, filter, groupBy, map, mergeAll, mergeMap, switchMap } from 'rxjs/operators';
import { forkJoin, of } from 'rxjs';
import { Set } from 'immutable';

export const loadEmployeeEpic$ = (action$: ActionsObservable<LoadEmployees>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD_EMPLOYEES'),
    mergeMap(action => {
        const employeeIds = Set(action.employeeIds);
        const requests = employeeIds.toArray().map(employeeId => {
            return deps.apiClient.getJSON(`/employees/${employeeId}`).pipe(
                map(obj => deserialize(obj, Employee)),
                handleHttpErrors<Employee>(),
            );
        });
        return forkJoin(requests);
    }),
    map(employees => loadEmployeesFinished(employees)),
);

export const loadDepartmentsEpic$ = (action$: ActionsObservable<LoadDepartments>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD-DEPARTMENTS'),
    switchMap(() => deps.apiClient.getJSON(`/departments`).pipe(
        map(obj => deserializeArray(obj as any, Department)),
        map(x => loadDepartmentsFinished(x)),
        handleHttpErrors(),
        ),
    ),
);

export const loadChiefsEpic$ = (action$: ActionsObservable<LoadDepartmentsFinished>) => action$.pipe(
    ofType('LOAD-DEPARTMENTS-FINISHED'),
    map(action => action.departments.filter(department => !!department.chiefId)),
    map(departments => departments.map(department => department.chiefId!)),
    map(chiefIds => loadEmployees(chiefIds))
);

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

export const loadEmployeesForUserDepartmentEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) => action$.pipe(
    ofType('LOAD-USER-EMPLOYEE-FINISHED'),
    map(x => loadEmployeesForDepartment(x.employee.departmentId)),
);

export const loadEmployeesForUserRoomEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) => action$.pipe(
    ofType('LOAD-USER-EMPLOYEE-FINISHED'),
    filter(action => { return action.employee.roomNumber !== null; }),
    map(x => loadEmployeesForRoom(x.employee.roomNumber!)),
);

export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType('LOAD-USER-EMPLOYEE-FINISHED'),
    map(x =>  loadDepartments()),
    catchError((e: Error) => of(loadFailedError(e.message))),
);
