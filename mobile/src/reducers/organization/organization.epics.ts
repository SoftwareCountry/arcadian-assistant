import { ActionsObservable } from 'redux-observable';
import * as oAction from './organization.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { AppState } from '../app.reducer';
import { Employee } from './employee.model';
import { Feed } from './feed.model';

const url = 'http://localhost:5000/api'; //TODO: fix hardcode

export const loadDepartmentsEpic$ = (action$: ActionsObservable<oAction.LoadDepartments>) =>
    action$.ofType('LOAD-DEPARTMENTS')
        .switchMap(x => ajaxGetJSON(`${url}/departments`))
        .map(x => {
            let data = deserializeArray(x as any, Department);
            console.log('departments ', data);
            return data;
        })
        .map(x => oAction.loadDepartmentsFinished(x));

export const loadChiefsEpic$ = (action$: ActionsObservable<oAction.LoadDepartmentsFinished> ) =>
    action$.ofType('LOAD-DEPARTMENTS-FINISHED')
        .switchMap(x =>
            x.departments.map(dep =>
                ajaxGetJSON(`${url}/employees/${dep.chiefId}`).map(obj => deserialize(obj, Employee))))
        .mergeAll()
        .map(x => oAction.loadEmployeeFinished(x));


//TODO: this thing loads all employees for all departments. It needs to be changed to load only requested ones
export const loadDepartmentsFinishedEpic$ = (action$: ActionsObservable<oAction.LoadDepartmentsFinished> ) =>
    action$.ofType('LOAD-DEPARTMENTS-FINISHED')
        .flatMap(x =>
            x.departments.map(dep =>
                oAction.loadEmployeesForDepartment(dep.departmentId)));

export const loadEmployeesForDepartmentEpic$ = (action$: ActionsObservable<oAction.LoadEmployeesForDepartment>, state: AppState) =>
    action$.ofType('LOAD_EMPLOYEES_FOR_DEPARTMENT')
        .groupBy(x => x.departmentId)
        .map(x =>
            x.switchMap(y =>
                ajaxGetJSON(`${url}/employees?departmentId=${x.key}`).map(obj => deserializeArray(obj as any, Employee))))
        .mergeAll()
        .flatMap(x => x.map(oAction.loadEmployeeFinished));

export const loadFeedsEpic$ = (action$: ActionsObservable<oAction.LoadFeeds>) =>
    action$.ofType('LOAD_FEEDS')
        .switchMap(x => ajaxGetJSON(`${url}/feeds/messages`))
        .map(x => {
            let data = deserializeArray(x as any, Feed);
            //console.log('feeds ', data);
            return data;
        })
        .map(x => oAction.loadFeedsFinished(x));