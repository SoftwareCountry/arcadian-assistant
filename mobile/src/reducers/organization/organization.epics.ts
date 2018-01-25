import { ActionsObservable, ofType } from 'redux-observable';
import { LoadDepartments, loadDepartmentsFinished, LoadDepartmentsFinished, loadEmployeeFinished, LoadEmployeesForDepartment, loadEmployeesForDepartment, LoadUser, loadUserFinished, LoadUserFinished, loadEmployeeForUserFinished, LoadEmployeeForUser, loadUser, loadEmployeeForUser, loadDepartments, LoadEmployeeForUserFinished } from './organization.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { AppState } from '../app.reducer';
import { Employee } from './employee.model';
import { User } from './user.model';

const url = 'http://localhost:5000/api'; //TODO: fix hardcode

// TODO: Handle error, display some big alert blocking app...
export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>) =>
    action$.ofType('LOAD-USER')
        .switchMap(x => ajaxGetJSON(`${url}/user`))
        .map(x => deserialize(x, User))
        .map(x => loadUserFinished(x));

export const loadUserFinishedEpic$ = (action$: ActionsObservable<LoadUserFinished>) =>
    action$.ofType('LOAD-USER-FINISHED')
        .map(x => loadEmployeeForUser(x.user));

// TODO: Handle error, display some big alert blocking app...
export const loadEmployeeForUserEpic$ = (action$: ActionsObservable<LoadEmployeeForUser>) =>
    action$.ofType('LOAD-EMPLOYEE-FOR-USER')
        .switchMap(x => ajaxGetJSON(`${url}/employees/${x.user.employeeId}`))
        .map(x => deserialize(x, Employee))
        .map(x => loadEmployeeForUserFinished(x));

export const loadEmployeeForUserFinishedEpic$ = (action$: ActionsObservable<LoadEmployeeForUserFinished>) =>
    action$.ofType('LOAD-EMPLOYEE-FOR-USER-FINISHED')
        .map(x => loadDepartments());

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