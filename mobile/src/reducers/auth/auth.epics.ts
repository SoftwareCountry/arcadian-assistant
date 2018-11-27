import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
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
import { handleHttpErrors } from '../errors/errors.epics';
import { distinctUntilChanged, flatMap, map } from 'rxjs/operators';
import { Alert } from 'react-native';
import { AuthenticationState } from '../../auth/authentication-state';
import { Action } from 'redux';


function showAlert(message: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => void, rejectButton: () => void) {
    Alert.alert(
        'Confirmation',
        `${message}`,
        [{ text: okButtonTitle, onPress: () => okButton() }, { text: rejectButtonTitle, onPress: () => rejectButton() }]);
}

export const startLoginProcessEpic$ = (action$: ActionsObservable<StartLoginProcess>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLoginProcess)
        .do(x => dep.oauthProcess.login())
        .ignoreElements();

export const startLogoutProcessEpic$ = (action$: ActionsObservable<StartLogoutProcess>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.startLogoutProcess)
        .map(x => {
            if (x.force) {
                dep.oauthProcess.logout();
                return;
            }
            showAlert('Are you sure you want to logout?', 'Logout', 'Cancel',
                      () => dep.oauthProcess.logout(), () => {});
        })
        .ignoreElements();

export const listenerAuthStateEpic$ = (action$: ActionsObservable<any>, state: AppState, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState
        .pipe(
            handleHttpErrors(),
            distinctUntilChanged((x, y) => x.isAuthenticated === y.isAuthenticated),
            flatMap<AuthenticationState, Action>(x => {
                if (x.isAuthenticated) {
                    return Observable.concat(Observable.of(userLoggedIn()), Observable.of(refresh()));
                } else {
                    return Observable.of(userLoggedOut());
                }
            })
        );


export const jwtTokenEpic$ = (action$: ActionsObservable<any>, state: AppState, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState
        .pipe(
            map(x => x.isAuthenticated ? jwtTokenSet(x.jwtToken) : jwtTokenSet(null))
        );
