import { deserialize } from 'santee-dcts/src/deserializer';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { User } from './user.model';
import {
    LoadUser,
    loadUserEmployeeFinished,
    LoadUserEmployeePermissions,
    loadUserEmployeePermissionsFinished,
    LoadUserFinished,
    loadUserFinished,
    LoadUserPreferences,
    loadUserPreferencesFinished,
    UpdateUserPreferences
} from './user.action';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';
import { Employee } from '../organization/employee.model';
import { handleHttpErrors } from '../errors/errors.epics';
import { startLogoutProcess } from '../auth/auth.action';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { UserPreferences } from './user-preferences.model';

export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER').pipe(
        switchMap(x => deps.apiClient.getJSON(`/user`).pipe(handleHttpErrors(false))),
        map(x => deserialize(x, User)),
        map(x => loadUserFinished(x.employeeId)),
        catchError(e => of(startLogoutProcess())));

export const loadUserFinishedEpic$ = (action$: ActionsObservable<LoadUserFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-FINISHED').pipe(
        switchMap(x => deps.apiClient.getJSON(`/employees/${x.userEmployeeId}`).pipe(handleHttpErrors(false))),
        map(obj => deserialize(obj, Employee)),
        map(z => loadUserEmployeeFinished(z)),
        catchError(e => of(startLogoutProcess())));

export const loadUserEmployeePermissionsEpic$ = (action$: ActionsObservable<LoadUserEmployeePermissions>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-EMPLOYEE-PERMISSIONS').pipe(
        switchMap(x => deps.apiClient.getJSON(`/user/permissions/${x.employeeId}`).pipe(handleHttpErrors(false))),
        map(obj => deserialize(obj, UserEmployeePermissions)),
        map(x => loadUserEmployeePermissionsFinished(x)));

export const loadUserPreferencesEpic$ = (action$: ActionsObservable<LoadUserPreferences>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-USER-PREFERENCES').pipe(
        switchMap(() => deps.apiClient.getJSON(`/user-preferences/`)
            .pipe(handleHttpErrors(false))),
        map(obj => deserialize(obj, UserPreferences)),
        map(response => loadUserPreferencesFinished(response)));

export const updateUserPreferencesEpic$ = (action$: ActionsObservable<UpdateUserPreferences>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType('UPDATE-USER-PREFERENCES').pipe(
        switchMap(action => {
            const requestBody = { ...action.preferences };

            return deps.apiClient.put(`/user-preferences/`, requestBody, { 'Content-Type': 'application/json' }).pipe(
                handleHttpErrors(false),
                map(_ => loadUserPreferencesFinished(action.preferences))
            );
        }));
