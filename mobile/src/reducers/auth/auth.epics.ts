import { ActionsObservable, ofType, combineEpics } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { DependenciesContainer, AppState } from '../app.reducer';
import { PressLogIn, PressLogOut, pressLogIn, pressLogOut } from '../auth/auth.action';

export const pressLogInEpic$ = (action$: ActionsObservable<PressLogIn>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType('PRESS-LOG-IN')
        .do(x => {
            dep.oauthProcess.login();
        }).ignoreElements();

export const pressLogOutEpic$ = (action$: ActionsObservable<PressLogOut>, state: AppState, dep: DependenciesContainer) =>
    action$.ofType('PRESS-LOG-OUT')
        .do(x => {
            dep.oauthProcess.logout();

        }).ignoreElements();

export const listenerAuthStateEpic$ = (action$: ActionsObservable<any>, state: AppState, dep: DependenciesContainer) =>
    dep.oauthProcess.authenticationState.map(x => {
        if (x.isAuthenticated === true) {
           return pressLogIn();
        } else {
           return pressLogOut();
        } 
    });
