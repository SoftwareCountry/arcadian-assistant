import { ActionsObservable, ofType, combineEpics } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { DependenciesContainer, AppState } from '../app.reducer';
import { StartLoginProcess, StartLogoutProcess, startLoginProcess, startLogoutProcess, userLoggedIn, userLoggedOut, jwtTokenSet } from '../auth/auth.action';
import { refresh } from '../refresh/refresh.action';
import { handleHttpErrors } from '../errors/errors.epics';
import { flatMap, distinctUntilChanged, map, filter } from 'rxjs/operators';
import { Alert } from 'react-native';
import { AuthenticationState, AuthenticatedState } from '../../auth/authentication-state';
import { Action } from 'redux';


function showAlert(message: string, okButtonTitle: string, rejectButtonTitle: string, okButton: () => void, rejectButton: () => void) {
    Alert.alert(
        'Confirmation',
        `${message}`,
        [{ text: okButtonTitle, onPress: () => okButton() }, { text: rejectButtonTitle, onPress: () => rejectButton() }]);
}

export const startLoginProcessEpic$ = (action$: ActionsObservable<StartLoginProcess>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType('START-LOGIN-PROCESS')
        .do(x => dep.oauthProcess.login())
        .ignoreElements();

export const startLogoutProcessEpic$ = (action$: ActionsObservable<StartLogoutProcess>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType('START-LOGOUT-PROCESS')
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
