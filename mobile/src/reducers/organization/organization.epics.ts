import { ActionsObservable, ofType } from 'redux-observable';
import {
    LoadDepartments, loadDepartmentsFinished, LoadDepartmentsFinished, loadDepartments,
    loadEmployeeFinished, LoadEmployeesForDepartment, loadEmployeesForDepartment,
    LoadEmployee, loadEmployee, LoadEmployeeFinished, LoadFeeds, loadFeedsFinished } from './organization.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { AppState } from '../app.reducer';
import { Employee } from './employee.model';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { Feed } from './feed.model';

export const url = 'http://localhost:5000/api'; //TODO: fix hardcode

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

//TODO: this thing loads all employees for all departments. It needs to be changed to load only requested ones
export const loadDepartmentsFinishedEpic$ = (action$: ActionsObservable<LoadDepartmentsFinished> ) =>
    action$.ofType('LOAD-DEPARTMENTS-FINISHED')
        .flatMap(x =>
            x.departments.map(dep =>
                loadEmployeesForDepartment(dep.departmentId)));

export const loadEmployeesForDepartmentEpic$ = (action$: ActionsObservable<LoadEmployeesForDepartment>, state: AppState) =>
    action$.ofType('LOAD_EMPLOYEES_FOR_DEPARTMENT')
        .groupBy(x => x.departmentId)
        .map(x =>
            x.switchMap(y =>
                ajaxGetJSON(`${url}/employees?departmentId=${x.key}`).map(obj => deserializeArray(obj as any, Employee))))
        .mergeAll()
        .flatMap(x => x.map(loadEmployeeFinished))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));
export const loadFeedsEpic$ = (action$: ActionsObservable<LoadFeeds>) =>
    action$.ofType('LOAD_FEEDS')
        .switchMap(x => ajaxGetJSON(`${url}/feeds/messages`))
        .map(x => deserializeArray(x as any, Feed))
        .map(x => loadFeedsFinished(x))
        .catch(e => Observable.of(loadFailedError(e.message)));