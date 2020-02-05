/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { deserialize } from 'santee-dcts/src/deserializer';
import { ActionsObservable, StateObservable } from 'redux-observable';
import { User } from './user.model';
import {
    LoadUser, LoadUserDepartmentFeatures, loadUserDepartmentFeatures, loadUserDepartmentFeaturesFinished,
    loadUserEmployeeFinished,
    LoadUserEmployeePermissions,
    loadUserEmployeePermissionsFinished,
    LoadUserFinished,
    loadUserFinished,
    loadUserPreferences,
    LoadUserPreferences,
    loadUserPreferencesFinished,
    UpdateUserPreferences,
    UserActionType
} from './user.action';
import { AppState } from 'react-native';
import { DependenciesContainer } from '../app.reducer';
import { Employee } from '../organization/employee.model';
import { handleHttpErrors } from '../../errors/error.operators';
import { startLogoutProcess } from '../auth/auth.action';
import { UserEmployeePermissions } from './user-employee-permissions.model';
import { catchError, map, mergeMap, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { UserPreferences } from './user-preferences.model';
import { DepartmentFeatures } from './department-features.model';

//----------------------------------------------------------------------------
export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType(UserActionType.loadUser).pipe(
        switchMap(x => deps.apiClient.getJSON<Object>(`/user`).pipe(handleHttpErrors(false))),
        map(x => deserialize(x, User)),
        mergeMap(x => of(loadUserFinished(x.employeeId), loadUserPreferences(x.employeeId), loadUserDepartmentFeatures())),
        catchError(e => of(startLogoutProcess())));

//----------------------------------------------------------------------------
export const loadUserFinishedEpic$ = (action$: ActionsObservable<LoadUserFinished>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType(UserActionType.loadUserFinished).pipe(
        switchMap(x => deps.apiClient.getJSON<Object>(`/employees/${x.userEmployeeId}`).pipe(handleHttpErrors(false))),
        map(obj => deserialize(obj, Employee)),
        map(z => loadUserEmployeeFinished(z)),
        catchError(e => of(startLogoutProcess())));

//----------------------------------------------------------------------------
export const loadUserEmployeePermissionsEpic$ = (action$: ActionsObservable<LoadUserEmployeePermissions>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType(UserActionType.loadUserEmployeePermissions).pipe(
        switchMap(x => deps.apiClient.getJSON<Object>(`/user/permissions/${x.employeeId}`).pipe(handleHttpErrors(false))),
        map(obj => deserialize(obj, UserEmployeePermissions)),
        map(x => loadUserEmployeePermissionsFinished(x)),
        catchError(e => of(startLogoutProcess())));

//----------------------------------------------------------------------------
export const loadUserPreferencesEpic$ = (action$: ActionsObservable<LoadUserPreferences>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType(UserActionType.loadUserPreferences).pipe(
        switchMap(() => deps.apiClient.getJSON<Object>(`/user-preferences/`)
            .pipe(handleHttpErrors(false))),
        map(obj => deserialize(obj, UserPreferences)),
        map(response => loadUserPreferencesFinished(response)),
        catchError(e => of(startLogoutProcess())));

//----------------------------------------------------------------------------
export const updateUserPreferencesEpic$ = (action$: ActionsObservable<UpdateUserPreferences>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType(UserActionType.updateUserPreferences).pipe(
        switchMap(action => {
            const requestBody = { ...action.preferences };

            return deps.apiClient.put(`/user-preferences/`, requestBody, {}).pipe(
                handleHttpErrors(false),
                map(_ => loadUserPreferencesFinished(action.preferences)),
                catchError(e => of(startLogoutProcess()))
            );
        }));

//----------------------------------------------------------------------------
export const loadUserDepartmentFeaturesEpic$ = (action$: ActionsObservable<LoadUserDepartmentFeatures>, appState: AppState, deps: DependenciesContainer) =>
    action$.ofType(UserActionType.loadUserDepartmentFeatures).pipe(
        switchMap(() => deps.apiClient.getJSON<Object>(`/user-department-features/`).pipe(handleHttpErrors(false))),
        map(obj => deserialize(obj, DepartmentFeatures)),
        map(features => loadUserDepartmentFeaturesFinished(features)),
        catchError(e => of(startLogoutProcess())));
