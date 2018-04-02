import { ActionsObservable, ofType, combineEpics } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { DependenciesContainer, AppState } from '../app.reducer';
import { StartLoginProcess, StartLogoutProcess, startLoginProcess, startLogoutProcess, userLoggedIn, userLoggedOut } from '../auth/auth.action';
import { loadFailedError } from '../errors/errors.action';

export const startLoginProcessEpic$ = (action$: ActionsObservable<StartLoginProcess>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType('START-LOGIN-PROCESS')
        .do(x => dep.oauthProcess.login())
        .ignoreElements();

export const startLogoutProcessEpic$ = (action$: ActionsObservable<StartLogoutProcess>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType('START-LOGOUT-PROCESS')
        .do(x => dep.oauthProcess.logout())
        .ignoreElements();

export const listenerAuthStateEpic$ = (action$: ActionsObservable<any>, state: AppState, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState
        .map(x => {
            if (x.isAuthenticated) {
                return userLoggedIn();
            } else {
                return userLoggedOut();
            }
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));
