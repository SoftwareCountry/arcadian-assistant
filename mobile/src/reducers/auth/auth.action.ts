/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

//============================================================================
export enum AuthActionType {
    startLoginProcess = 'START-LOGIN-PROCESS',
    startLogoutProcess = 'START-LOGOUT-PROCESS',
    userLoggedIn = 'USER-LOGGED-IN',
    userLoggedOut = 'USER-LOGGED-OUT',
    jwtTokenSet = 'JWT-TOKEN-SET',
}

//============================================================================
// - Actions
//============================================================================

export interface StartLoginProcess {
    type: AuthActionType.startLoginProcess;
}

export interface StartLogoutProcess {
    type: AuthActionType.startLogoutProcess;
    force: boolean;
}

export interface UserLoggedIn {
    type: AuthActionType.userLoggedIn;
}

export interface UserLoggedOut {
    type: AuthActionType.userLoggedOut;
}

export interface JwtTokenSet {
    type: AuthActionType.jwtTokenSet;
    jwtToken: string;
}

export type AuthActions = StartLoginProcess
    | StartLogoutProcess
    | UserLoggedIn
    | UserLoggedOut
    | JwtTokenSet;


//============================================================================
// - Action Creators
//============================================================================

export const startLoginProcess = (): StartLoginProcess => {
    return {
        type: AuthActionType.startLoginProcess,
    };
};

export const startLogoutProcess = (force: boolean = false): StartLogoutProcess => {
    return {
        type: AuthActionType.startLogoutProcess,
        force,
    };
};

export const userLoggedIn = (): UserLoggedIn => {
    return {
        type: AuthActionType.userLoggedIn,
    };
};

export const userLoggedOut = (): UserLoggedOut => {
    return {
        type: AuthActionType.userLoggedOut,
    };
};

export const jwtTokenSet = (jwtToken: string | null): JwtTokenSet => {
    return {
        type: AuthActionType.jwtTokenSet,
        jwtToken,
    };
};
