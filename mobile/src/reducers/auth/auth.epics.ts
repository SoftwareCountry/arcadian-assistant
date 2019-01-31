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
    userLoggedOut,
    startLoginProcess,
    UserLoggedIn,
} from './auth.action';
import { refresh } from '../refresh/refresh.action';
import { handleHttpErrors } from '../../errors/error.operators';
import { distinctUntilChanged, flatMap, ignoreElements, map, tap, filter } from 'rxjs/operators';
import { Alert } from 'react-native';
import { AuthenticationState } from '../../auth/authentication-state';
import { Action } from 'redux';
import { of, concat, Observable, from, merge, empty } from 'rxjs';
import { notificationsUnregister } from '../../notifications/notification.epics';

//----------------------------------------------------------------------------
function showAlert<TOk, TCancel>(message: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => Promise<TOk>, rejectButton: () => Promise<TCancel>) {

    return new Promise<TOk | TCancel>((resolve) => {
        Alert.alert(
            'Confirmation',
            `${message}`,
            [
                {
                    text: rejectButtonTitle,
                    onPress: () => resolve(rejectButton()),
                    style: 'cancel',
                },
                {
                    text: okButtonTitle,
                    onPress: () => resolve(okButton()),
                }
            ]);
    });
}

//----------------------------------------------------------------------------
function showErrorAlert(error: any) {
    Alert.alert(
        'Error occurred',
        `${getErrorMessage(error)}`,
        [
            {
                text: 'OK', onPress: () => { },
            },
        ]);
}

//----------------------------------------------------------------------------
function getErrorMessage(error: any): string {
    const detailedDescription = error && error.response && error.response.error_description ?
        error.response.error_description : undefined;

    const errorText =
        error
            ? error.message
                ? error.message.toString()
                : error.toString()
            : 'unknown error';

    return detailedDescription ? detailedDescription : errorText;
}

//----------------------------------------------------------------------------
export const startLoginProcessEpic$ = (action$: ActionsObservable<StartLoginProcess>, _: unknown, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLoginProcess).pipe(
        flatMap(x => {
            console.log('test');
            return dep.oauthProcess.login().then(
                () => userLoggedIn() //successful login
            ).catch(
                () => startLoginProcess() // error occurred
            );
        }), //TODO: redirect back
    );

export const shouldRefreshEpic$ = (action$: ActionsObservable<UserLoggedIn>) =>
    action$.ofType(AuthActionType.userLoggedIn).pipe(
        map(() => refresh())
    );

export const jwtTokenEpic$ = (actions: unknown, state: unknown, dep: DependenciesContainer) =>
    dep.oauthProcess.jwtTokenHandler.get$().pipe(
        map(jwtTokenSet)
    );

//----------------------------------------------------------------------------
async function logout(dependencies: DependenciesContainer, installId?: string) {
    if (installId) {
        try {
            await notificationsUnregister(dependencies, installId);
        } catch (e) {
            console.warn(e);
        }
    }
    try {
        await dependencies.oauthProcess.logout();
    } catch (e) {
        console.warn('Error during logout', e);
    }
}

//----------------------------------------------------------------------------
export const startLogoutProcessEpic$ = (action$: ActionsObservable<StartLogoutProcess>, state$: StateObservable<AppState>, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLogoutProcess).pipe(
        map(x => {
            const logoutCallback = async () => {
                await logout(dep, state$.value.notifications.installId);
                return true;
            };

            if (x.force) {
                return logoutCallback();
            }
            return showAlert(
                'Are you sure you want to logout?',
                'Logout',
                'Cancel',
                logoutCallback,
                async () => false
            );
        }),
        flatMap(x => from(x)),
        flatMap(x => {
            if (x) {
                return of(userLoggedOut());
            } else {
                return empty();
            }
        })
    );

/*
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
*/