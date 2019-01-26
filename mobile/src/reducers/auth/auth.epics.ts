/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, StateObservable } from 'redux-observable';
import { AppState, DependenciesContainer } from '../app.reducer';
import {
    AuthActionType,
    jwtTokenSet,
    StartLoginProcess,
    StartLogoutProcess,
    userLoggedIn,
    userLoggedOut
} from './auth.action';
import { refresh } from '../refresh/refresh.action';
import { getHttpErrorMessage, handleHttpErrors } from '../../errors/error.operators';
import { distinctUntilChanged, flatMap, ignoreElements, map, tap } from 'rxjs/operators';
import { Alert } from 'react-native';
import { AuthenticationState } from '../../auth/authentication-state';
import { Action } from 'redux';
import { of } from 'rxjs';
import { notificationsUnregister } from '../../notifications/notification.epics';

//----------------------------------------------------------------------------
function showAlert(message: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => void, rejectButton: () => void) {
    Alert.alert(
        'Confirmation',
        `${message}`,
        [
            {
                text: rejectButtonTitle,
                onPress: () => rejectButton(),
                style: 'cancel',
            },
            {
                text: okButtonTitle,
                onPress: () => okButton(),
            }
        ]);
}

//----------------------------------------------------------------------------
function showErrorAlert(error: any) {
    Alert.alert(
        'Error',
        `${getErrorMessage(error)}`,
        [
            {
                text: 'OK', onPress: () => {},
            },
        ]);
}

//----------------------------------------------------------------------------
function getErrorMessage(error: any): string {
    const detailedDescription = error && error.response && error.response.error_description ?
        error.response.error_description : undefined;

    const errorText = getHttpErrorMessage(error);

    return detailedDescription ? detailedDescription : errorText;
}

//----------------------------------------------------------------------------
export const startLoginProcessEpic$ = (action$: ActionsObservable<StartLoginProcess>, _: StateObservable<AppState>, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLoginProcess).pipe(
        tap(x => dep.oauthProcess.login()),
        ignoreElements(),
    );

//----------------------------------------------------------------------------
function logout(dependencies: DependenciesContainer, installId?: string) {
    if (installId) {
        notificationsUnregister(dependencies, installId).catch(console.warn);
    }
    dependencies.oauthProcess.logout();
}

//----------------------------------------------------------------------------
export const startLogoutProcessEpic$ = (action$: ActionsObservable<StartLogoutProcess>, state$: StateObservable<AppState>, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLogoutProcess).pipe(
        tap(x => {
            if (x.force) {
                logout(dep, state$.value.notifications.installId);
                return;
            }
            showAlert(
                'Are you sure you want to logout?',
                'Logout',
                'Cancel',
                () => {
                    logout(dep, state$.value.notifications.installId);
                },
                () => {
                });
        }),
        ignoreElements()
    );

//----------------------------------------------------------------------------
export const listenerAuthStateEpic$ = (action$: ActionsObservable<any>, state$: StateObservable<AppState>, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState
        .pipe(
            handleHttpErrors(),
            distinctUntilChanged<AuthenticationState>((x, y) => (x.isAuthenticated && y.isAuthenticated)),
            flatMap<AuthenticationState, Action>(authState => {
                    if (authState.isAuthenticated) {
                        return of(userLoggedIn(), refresh());
                    } else {
                        if (authState.error) {
                            showErrorAlert(authState.error);
                        }
                        return of(userLoggedOut());
                    }
                }
            ));

//----------------------------------------------------------------------------
export const jwtTokenEpic$ = (action$: ActionsObservable<any>, _: StateObservable<AppState>, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState
        .pipe(
            map((x: AuthenticationState) => x.isAuthenticated ? jwtTokenSet(x.jwtToken) : jwtTokenSet(null))
        );
