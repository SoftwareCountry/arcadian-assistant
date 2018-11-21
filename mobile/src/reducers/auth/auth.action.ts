export interface StartLoginProcess {
    type: 'START-LOGIN-PROCESS';
}

export const startLoginProcess = (): StartLoginProcess => ({ type: 'START-LOGIN-PROCESS'});

export interface StartLogoutProcess {
    type: 'START-LOGOUT-PROCESS';
    force: boolean;
}

export const startLogoutProcess = (force: boolean = false): StartLogoutProcess => ({ type: 'START-LOGOUT-PROCESS', force });

export interface UserLoggedIn {
    type: 'USER-LOGGED-IN';
}

export const userLoggedIn = (): UserLoggedIn => ({ type: 'USER-LOGGED-IN' });

export interface UserLoggedOut {
    type: 'USER-LOGGED-OUT';
}

export const userLoggedOut = (): UserLoggedOut => ({ type: 'USER-LOGGED-OUT' });

export interface JwtTokenSet {
    type: 'JWT-TOKEN-SET';
    jwtToken: string;
}

export const jwtTokenSet = (jwtToken: string | null): JwtTokenSet => ({ type: 'JWT-TOKEN-SET', jwtToken });

export type AuthActions = StartLoginProcess | StartLogoutProcess | UserLoggedIn | UserLoggedOut | JwtTokenSet;
