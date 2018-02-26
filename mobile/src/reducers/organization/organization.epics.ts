import { ActionsObservable, ofType } from 'redux-observable';
import {
    LoadDepartments, loadDepartmentsFinished, LoadDepartmentsFinished, loadDepartments,
    loadEmployeeFinished, LoadEmployeesForDepartment, LoadEmployeesForRoom, loadEmployeesForDepartment,
    LoadEmployee, loadEmployee, LoadEmployeeFinished, loadEmployeesForRoom } from './organization.action';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { AppState } from '../app.reducer';
import { Employee } from './employee.model';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { apiUrl as url } from '../const';

export const loadEmployeeEpic$ = (action$: ActionsObservable<LoadEmployee>) =>
    action$.ofType('LOAD_EMPLOYEE')
        .groupBy(x => x.employeeId)
        .map(x =>
            x.switchMap(y => ajaxGetJSON(`${url}/employees/${y.employeeId}`)).map(obj => deserialize(obj, Employee))
        )
        .mergeAll()
        .map(x => loadEmployeeFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const loadDepartmentsEpic$ = (action$: ActionsObservable<LoadDepartments>) =>
    action$.ofType('LOAD-DEPARTMENTS')
        .switchMap(x => ajaxGetJSON(`${url}/departments`))
        .map(x => deserializeArray(x as any, Department))
        .map(x => loadDepartmentsFinished(x))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const loadChiefsEpic$ = (action$: ActionsObservable<LoadDepartmentsFinished> ) =>
    action$.ofType('LOAD-DEPARTMENTS-FINISHED')
        .flatMap(x => x.departments.map(dep => loadEmployee(dep.chiefId)));

export const loadEmployeesForDepartmentEpic$ = (action$: ActionsObservable<LoadEmployeesForDepartment>, state: AppState) =>
    action$.ofType('LOAD_EMPLOYEES_FOR_DEPARTMENT')
        .groupBy(x => x.departmentId)
        .map(x =>
            x.switchMap(y =>
                ajaxGetJSON(`${url}/employees?departmentId=${x.key}`).map(obj => deserializeArray(obj as any, Employee))))
        .mergeAll()
        .flatMap(x => x.map(loadEmployeeFinished))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const loadEmployeesForRoomEpic$ = (action$: ActionsObservable<LoadEmployeesForRoom>, state: AppState) =>
        action$.ofType('LOAD_EMPLOYEES_FOR_ROOM')
            .groupBy(x => x.roomNumber)
            .map(x =>
                x.switchMap(y =>
                    ajaxGetJSON(`${url}/employees?roomNumber=${x.key}`).map(obj => deserializeArray(obj as any, Employee))))
            .mergeAll()
            .flatMap(x => x.map(loadEmployeeFinished))
            .catch((e: Error) => Observable.of(loadFailedError(e.message)));

export const loadEmployeesForUserDepartmentEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
            action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
                .map(x => loadEmployeesForDepartment(x.employee.departmentId));
        
        
export const loadEmployeesForUserRoomEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
                action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
                    .map(x => loadEmployeesForRoom(x.employee.roomNumber));