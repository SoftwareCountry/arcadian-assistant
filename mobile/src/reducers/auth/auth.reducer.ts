import { combineEpics } from 'redux-observable';
import { jwtTokenEpic$, loadPin$, loadRefreshToken$, shouldRefreshEpic$, storePin$ } from './auth.epics';
import { AuthActions, AuthActionType } from './auth.action';
import { JwtToken } from '../../auth/jwt-token-handler';
import { startLogoutProcessEpic$ } from './logout.epics';
import { startLoginProcessEpic$ } from './login.epics';

export const authEpics$ = combineEpics(
    startLoginProcessEpic$,
    startLogoutProcessEpic$,
    shouldRefreshEpic$,
    jwtTokenEpic$,
    loadRefreshToken$,
    loadPin$,
    storePin$,
);

export interface AuthInfo {
    isAuthenticated: boolean;
}

export interface AuthState {
    authInfo: AuthInfo | null;
    jwtToken: JwtToken | null;
    hasRefreshToken: boolean | undefined;
    pinCode: string | null | undefined;
}

const initState: AuthState = {
    authInfo: null,
    jwtToken: null,
    hasRefreshToken: undefined,
    pinCode: undefined,
};

export const authReducer = (state: AuthState = initState, action: AuthActions): AuthState => {
    switch (action.type) {
        case AuthActionType.userLoggedIn:
            return {
                ...state,
                authInfo: {
                    isAuthenticated: true,
                },
                hasRefreshToken: true,
            };

        case AuthActionType.userLoggedOut:
            return {
                ...state,
                authInfo: {
                    isAuthenticated: false,
                },
                jwtToken: null,
                hasRefreshToken: false,
                pinCode: null,
            };

        case AuthActionType.jwtTokenSet:
            return {
                ...state,
                jwtToken: action.jwtToken,
            };

        case AuthActionType.pinLoaded:
            return {
                ...state,
                pinCode: action.pin,
            };

        case AuthActionType.pinStore:
            return {
                ...state,
                pinCode: action.pin,
            };

        case AuthActionType.refreshTokenLoaded:
            return {
                ...state,
                hasRefreshToken: !!action.refreshToken,
            };

        default:
            return state;
    }
};
