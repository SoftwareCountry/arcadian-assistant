/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Action } from 'redux';
import { Nullable } from 'types';

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

export interface StartLoginProcess extends Action {
    type: AuthActionType.startLoginProcess;
}

export interface StartLogoutProcess extends Action {
    type: AuthActionType.startLogoutProcess;
    force: boolean;
}

export interface UserLoggedIn extends Action {
    type: AuthActionType.userLoggedIn;
}

export interface UserLoggedOut extends Action {
    type: AuthActionType.userLoggedOut;
}

export interface JwtTokenSet extends Action {
    type: AuthActionType.jwtTokenSet;
    jwtToken: Nullable<string>;
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

export const jwtTokenSet = (jwtToken: Nullable<string>): JwtTokenSet => {
    return {
        type: AuthActionType.jwtTokenSet,
        jwtToken,
    };
};
