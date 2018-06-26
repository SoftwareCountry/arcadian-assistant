import { deserialize } from 'santee-dcts/src/deserializer';
import { loadEmployee, LoadEmployeeFinished } from '../organization/organization.action';
import { ActionsObservable } from 'redux-observable';
import { User } from './user.model';
import { LoadUser, loadUserFinished, LoadUserFinished, loadUserEmployeeFinished, LoadUserEmployeeFinished, LoadUserEmployeePermissions, loadUserEmployeePermissionsFinished } from './user.action';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';
import { Employee } from '../organization/employee.model';
import { handleHttpErrors } from '../errors/errors.epics';
import { startLogoutProcess } from '../auth/auth.action';
import { UserEmployeePermissions } from './user-permissions.model';

export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER')
        .switchMap(x => deps.apiClient.getJSON(`/user`)
            .pipe(handleHttpErrors(false)))
        .map(x => deserialize(x, User))
        .map(x => loadUserFinished(x.employeeId))
        .catch(e => Observable.of(startLogoutProcess()));

export const loadUserFinishedEpic$ = (action$: ActionsObservable<LoadUserFinished>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-FINISHED')
        .switchMap(x => deps.apiClient.getJSON(`/employees/${x.userEmployeeId}`)
            .pipe(handleHttpErrors(false)))
        .map(obj => deserialize(obj, Employee))
        .map(z => loadUserEmployeeFinished(z))
        .catch(e => Observable.of(startLogoutProcess()));

export const loadUserEmployeePermissionsEpic$ = (action$: ActionsObservable<LoadUserEmployeePermissions>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-PERMISSIONS')
        .switchMap(x => deps.apiClient.getJSON(`/user/${x.employeeId}/permissions`)
            .pipe(handleHttpErrors(false)))
        .map(obj => deserialize(obj, UserEmployeePermissions))
        .map(x => loadUserEmployeePermissionsFinished(x));
