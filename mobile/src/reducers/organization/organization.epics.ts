import { ActionsObservable, ofType } from 'redux-observable';
import { LoadDepartments, loadDepartmentsFinished, LoadDepartmentsFinished, loadEmployeeFinished, LoadEmployeesForDepartment, loadEmployeesForDepartment, LoadUser, loadUserFinished, LoadEmployeeFinished, LoadUserFinished } from './organization.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { AppState } from '../app.reducer';
import { Employee } from './employee.model';
import { User } from './user.model';

const url = 'http://localhost:5000/api'; //TODO: fix hardcode

export const loadDepartmentsEpic$ = (action$: ActionsObservable<LoadDepartments>) =>
    action$.ofType('LOAD-DEPARTMENTS')
        .switchMap(x => ajaxGetJSON(`${url}/departments`))
        .map(x => deserializeArray(x as any, Department))
        .map(x => loadDepartmentsFinished(x));

export const loadChiefsEpic$ = (action$: ActionsObservable<LoadDepartmentsFinished> ) => 
    action$.ofType('LOAD-DEPARTMENTS-FINISHED')
        .switchMap(x => 
            x.departments.map(dep =>
                ajaxGetJSON(`${url}/employees/${dep.chiefId}`).map(obj => deserialize(obj, Employee))))
        .mergeAll()
        .map(x => loadEmployeeFinished(x));


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
        .flatMap(x => x.map(loadEmployeeFinished));

// TODO: Handle error
export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>) =>
    action$
        .switchMap(x => ajaxGetJSON(`${url}/user`))
        .map(x => deserialize(x, User))
        .switchMap(x => ajaxGetJSON(`${url}/employees/${x.employeeId}`))
        .map(x => deserialize(x, Employee))
        .map(x => loadUserFinished(x));