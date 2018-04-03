export interface StartLoginProcess {
    type: 'START-LOGIN-PROCESS';
}

export const startLoginProcess = (): StartLoginProcess => ({ type: 'START-LOGIN-PROCESS'});

export interface StartLogoutProcess {
    type: 'START-LOGOUT-PROCESS';
}

export const startLogoutProcess = (): StartLogoutProcess => ({ type: 'START-LOGOUT-PROCESS' });

export interface UserLoggedIn {
    type: 'USER-LOGGED-IN';
}

export const userLoggedIn = (): UserLoggedIn => ({ type: 'USER-LOGGED-IN' });

export interface UserLoggedOut {
    type: 'USER-LOGGED-OUT';
}

export const userLoggedOut = (): UserLoggedOut => ({ type: 'USER-LOGGED-OUT' });

export type AuthActions = StartLoginProcess | StartLogoutProcess | UserLoggedIn | UserLoggedOut;
