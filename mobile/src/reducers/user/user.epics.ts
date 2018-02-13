import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { deserialize } from 'santee-dcts/src/deserializer';
import { loadEmployee, LoadEmployeeFinished, OrganizationActions, LoadEmployeesForDepartment } from '../organization/organization.action';
import { ActionsObservable } from 'redux-observable';
import { User } from './user.model';
import { LoadUser, loadUserFinished, LoadUserFinished, loadUserEmployeeFinished, LoadUserEmployeeFinished } from './user.action';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { apiUrl as url } from '../const';

// TODO: Handle error, display some big alert blocking app...
export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>) =>
    action$.ofType('LOAD-USER')
        .switchMap(x => ajaxGetJSON(`${url}/user`))
        .map(x => deserialize(x, User))
        .map(x => loadUserFinished(x))
        .catch(x => Observable.of(loadFailedError(x.message)));

export const loadUserFinishedEpic$ = (action$: ActionsObservable<LoadUserFinished | LoadEmployeeFinished>) =>
    Observable.combineLatest<LoadUserFinished, LoadEmployeeFinished>(
        action$.ofType('LOAD-USER-FINISHED'), 
        action$.ofType('LOAD_EMPLOYEE_FINISHED')
    ).filter(([userLoaded, employeeLoaded]) => userLoaded.user.employeeId === employeeLoaded.employee.employeeId)
     .map(([userLoaded, employeeLoaded]) => loadUserEmployeeFinished(employeeLoaded.employee));
