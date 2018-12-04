import { ActionsObservable, ofType, combineEpics, StateObservable } from 'redux-observable';
import { DependenciesContainer, AppState } from '../app.reducer';
import { StartLoginProcess, StartLogoutProcess, startLoginProcess, startLogoutProcess, userLoggedIn, userLoggedOut, jwtTokenSet } from '../auth/auth.action';
import { refresh } from '../refresh/refresh.action';
import { handleHttpErrors } from '../errors/errors.epics';
import {flatMap, distinctUntilChanged, map, filter, ignoreElements, tap} from 'rxjs/operators';
import { Alert } from 'react-native';
import {AuthenticationState, AuthenticatedState, NotAuthenticatedState} from '../../auth/authentication-state';
import { Action } from 'redux';
import { concat, of } from 'rxjs';


function showAlert(message: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => void, rejectButton: () => void) {
    Alert.alert(
        'Confirmation',
        `${message}`,
        [{ text: okButtonTitle, onPress: () => okButton() }, { text: rejectButtonTitle, onPress: () => rejectButton() }]);
}

export const startLoginProcessEpic$ = (action$: ActionsObservable<StartLoginProcess>, _: StateObservable<AppState>, dep: DependenciesContainer) =>
    action$.ofType('START-LOGIN-PROCESS').pipe(
        tap(x => dep.oauthProcess.login()),
        ignoreElements(),
    );

export const startLogoutProcessEpic$ = (action$: ActionsObservable<StartLogoutProcess>, _: StateObservable<AppState>, dep: DependenciesContainer) =>
    action$.ofType('START-LOGOUT-PROCESS').pipe(
        tap(x => {
            if (x.force) {
                dep.oauthProcess.logout();
                return;
            }
            showAlert(
                'Are you sure you want to logout?',
                'Logout',
                'Cancel',
                () => dep.oauthProcess.logout(),
                () => {});
        }),
        ignoreElements()
    );

export const listenerAuthStateEpic$ = (action$: ActionsObservable<any>, _: StateObservable<AppState>, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState
        .pipe(
            handleHttpErrors(),
            distinctUntilChanged<AuthenticationState>((x, y) => x.isAuthenticated === y.isAuthenticated),
            flatMap<AuthenticationState, Action>(x => {
                if (x.isAuthenticated) {
                    return concat(of(userLoggedIn()), of(refresh()));
                } else {
                    return of(userLoggedOut());
                }
            })
        );


export const jwtTokenEpic$ = (action$: ActionsObservable<any>, _: StateObservable<AppState>, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState
        .pipe(
            map((x: AuthenticationState) => x.isAuthenticated ? jwtTokenSet(x.jwtToken) : jwtTokenSet(null))
        );
