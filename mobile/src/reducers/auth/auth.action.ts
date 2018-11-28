import { Action } from 'redux';

export interface StartLoginProcess extends Action {
    type: 'START-LOGIN-PROCESS';
}

export const startLoginProcess = (): StartLoginProcess => ({ type: 'START-LOGIN-PROCESS'});

export interface StartLogoutProcess extends Action {
    type: 'START-LOGOUT-PROCESS';
    force: boolean;
}

export const startLogoutProcess = (force: boolean = false): StartLogoutProcess => ({ type: 'START-LOGOUT-PROCESS', force });

export interface UserLoggedIn extends Action {
    type: 'USER-LOGGED-IN';
}

export const userLoggedIn = (): UserLoggedIn => ({ type: 'USER-LOGGED-IN' });

export interface UserLoggedOut extends Action {
    type: 'USER-LOGGED-OUT';
}

export const userLoggedOut = (): UserLoggedOut => ({ type: 'USER-LOGGED-OUT' });

export interface JwtTokenSet extends Action {
    type: 'JWT-TOKEN-SET';
    jwtToken: string;
}

export const jwtTokenSet = (jwtToken: string | null): JwtTokenSet => ({ type: 'JWT-TOKEN-SET', jwtToken });

export type AuthActions = StartLoginProcess | StartLogoutProcess | UserLoggedIn | UserLoggedOut | JwtTokenSet;
