import { combineEpics } from 'redux-observable';
//import { jwtTokenEpic$, listenerAuthStateEpic$, startLoginProcessEpic$, startLogoutProcessEpic$ } from './auth.epics';
import {startLoginProcessEpic$, startLogoutProcessEpic$, shouldRefreshEpic$, jwtTokenEpic$ } from './auth.epics';
import { AuthActions, AuthActionType } from './auth.action';
import { JwtToken } from '../../auth/jwt-token-handler';

export const authEpics$ = combineEpics(
    //appStartCheckEpic$,
    startLoginProcessEpic$,
    startLogoutProcessEpic$,
    shouldRefreshEpic$,
    //listenerAuthStateEpic$ as any,
    jwtTokenEpic$
);

export interface AuthInfo {
    isAuthenticated: boolean;
}

export interface AuthState {
    authInfo: AuthInfo | null;
    jwtToken: JwtToken | null;
}

const initState: AuthState = {
    authInfo: null,
    jwtToken: null
};

export const authReducer = (state: AuthState = initState, action: AuthActions): AuthState => {
    switch (action.type) {
        case AuthActionType.userLoggedIn:
            return {
                ...state,
                authInfo: {
                    isAuthenticated: true,
                }
            };

        case AuthActionType.userLoggedOut:
            return {
                ...state,
                authInfo: {
                    isAuthenticated: false,
                },
                jwtToken: null
            };

        case AuthActionType.jwtTokenSet:
            return {
                ...state,
                jwtToken: action.jwtToken
            };
        default:
            return state;
    }
};
