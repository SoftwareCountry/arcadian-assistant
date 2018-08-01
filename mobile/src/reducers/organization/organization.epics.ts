import { ActionsObservable, ofType, combineEpics } from 'redux-observable';
import {
    loadEmployeeFinished, LoadEmployeesForDepartment, LoadEmployeesForRoom, loadEmployeesForDepartment,
    LoadEmployee, loadEmployee, LoadEmployeeFinished, loadEmployeesForRoom, LoadPhoto, loadPhotoFinished
} from './organization.action';
import { LoadDepartmentsFinished, loadDepartments } from '../people/people.action';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { AppState, AppEpic, DependenciesContainer } from '../app.reducer';
import { Employee, Photo } from './employee.model';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { handleHttpErrors } from '../errors/errors.epics';

export const loadEmployeeEpic$ = (action$: ActionsObservable<LoadEmployee>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD_EMPLOYEE')
        .groupBy(x => x.employeeId)
        .map(x =>
            x.switchMap(y => deps.apiClient.getJSON(`/employees/${y.employeeId}`)).map(obj => deserialize(obj, Employee))
                .pipe(handleHttpErrors())
        )
        .mergeAll()
        .map(x => loadEmployeeFinished(x));

export const loadPhotoEpic$ = (action$: ActionsObservable<LoadPhoto>, deps: DependenciesContainer) => 
        action$.ofType('LOAD_PHOTO')
            .map(x => {
                const photo = deserialize(deps.apiClient.getJSON(`/employees/${x.employeeId}/photo`), Photo);
                return loadPhotoFinished(photo, x.employeeId);
            });

export const loadChiefsEpic$ = (action$: ActionsObservable<LoadDepartmentsFinished>) =>
    action$.ofType('LOAD-DEPARTMENTS-FINISHED')
        .flatMap(x => x.departments.map(dep => loadEmployee(dep.chiefId)));

export const loadEmployeesForDepartmentEpic$ = (action$: ActionsObservable<LoadEmployeesForDepartment>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD_EMPLOYEES_FOR_DEPARTMENT')
        .groupBy(x => x.departmentId)
        .map(x =>
            x.switchMap(y =>
                deps.apiClient.getJSON(`/employees?departmentId=${x.key}`).map(obj => deserializeArray(obj as any, Employee))
                    .pipe(handleHttpErrors())))
        .mergeAll()
        .flatMap(x => x.map(loadEmployeeFinished));

export const loadEmployeesForRoomEpic$ = (action$: ActionsObservable<LoadEmployeesForRoom>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD_EMPLOYEES_FOR_ROOM')
        .groupBy(x => x.roomNumber)
        .map(x =>
            x.switchMap(y =>
                deps.apiClient.getJSON(`/employees?roomNumber=${x.key}`).map(obj => deserializeArray(obj as any, Employee))
                    .pipe(handleHttpErrors())))
        .mergeAll()
        .flatMap(x => x.map(loadEmployeeFinished));

export const loadEmployeesForUserDepartmentEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .map(x => loadEmployeesForDepartment(x.employee.departmentId));


export const loadEmployeesForUserRoomEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .map(x => loadEmployeesForRoom(x.employee.roomNumber));

export const loadUserEmployeeFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        .map(x =>  loadDepartments())
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));