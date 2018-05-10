import { ActionsObservable, ofType, combineEpics } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { DependenciesContainer, AppState } from '../app.reducer';
import { StartLoginProcess, StartLogoutProcess, startLoginProcess, startLogoutProcess, userLoggedIn, userLoggedOut } from '../auth/auth.action';
import { loadFailedError } from '../errors/errors.action';
import { loadUser } from '../user/user.action';
import { loadDepartments } from '../organization/organization.action';

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
        .distinctUntilChanged((x, y) => x.isAuthenticated === y.isAuthenticated)
        .flatMap(x => {
            if (x.isAuthenticated) {
                return Observable.concat(Observable.of(userLoggedIn()), Observable.of(loadUser()), Observable.of(loadDepartments()));
            } else {
                return Observable.of(userLoggedOut());
            }
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));
