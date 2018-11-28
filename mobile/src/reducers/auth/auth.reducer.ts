import { combineEpics } from 'redux-observable';
import { jwtTokenEpic$, listenerAuthStateEpic$, startLoginProcessEpic$, startLogoutProcessEpic$ } from './auth.epics';
import { AuthActions, AuthActionType } from './auth.action';

export const authEpics$ = combineEpics(
    startLoginProcessEpic$ as any,
    startLogoutProcessEpic$ as any,
    listenerAuthStateEpic$ as any,
    jwtTokenEpic$ as any
);

export interface AuthInfo {
    isAuthenticated: boolean;
    jwtToken: string | null;
}

export interface AuthState {
    authInfo: AuthInfo | null;
}

const initState: AuthState = {
    authInfo: null,
};

export const authReducer = (state: AuthState = initState, action: AuthActions): AuthState => {
    switch (action.type) {
        case AuthActionType.userLoggedIn:
            return {
                ...state,
                authInfo: {
                    ...state.authInfo,
                    isAuthenticated: true,
                }
            };
        case AuthActionType.userLoggedOut:
            return {
                ...state,
                authInfo: {
                    isAuthenticated: false,
                    jwtToken: null
                }
            };

        case AuthActionType.jwtTokenSet:
            return {
                ...state,
                authInfo: {
                    ...state.authInfo,
                    jwtToken: action.jwtToken
                }
            };
        default:
            return state;
   }
};
