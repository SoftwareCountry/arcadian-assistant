/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Action } from 'redux';
import { Nullable } from 'types';
import { JwtToken } from '../../auth/jwt-token-handler';

//============================================================================
export enum AuthActionType {
    startLoginProcess = 'START-LOGIN-PROCESS',
    startLogoutProcess = 'START-LOGOUT-PROCESS',
    userLoggedIn = 'USER-LOGGED-IN',
    userLoggedOut = 'USER-LOGGED-OUT',
    jwtTokenSet = 'JWT-TOKEN-SET',
    loadRefreshToken = 'LOAD-REFRESH-TOKEN',
    refreshTokenLoaded = 'REFRESH-TOKEN-LOADED',
    pinLoad = 'PIN-LOAD',
    pinLoaded = 'PIN-LOADED',
    pinStore = 'PIN-STORE',
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
    jwtToken: Nullable<JwtToken>;
}

export interface LoadRefreshToken extends Action {
    type: AuthActionType.loadRefreshToken;
}

export interface RefreshTokenLoaded extends Action {
    type: AuthActionType.refreshTokenLoaded;
    refreshToken: string | null;
}

export interface PinLoad extends Action {
    type: AuthActionType.pinLoad;
}

export interface PinLoaded extends Action {
    type: AuthActionType.pinLoaded;
    pin: string | null;
}

export interface PinStore extends Action {
    type: AuthActionType.pinStore;
    pin: string;
}

export type AuthActions = StartLoginProcess
    | StartLogoutProcess
    | UserLoggedIn
    | UserLoggedOut
    | JwtTokenSet
    | LoadRefreshToken
    | RefreshTokenLoaded
    | PinLoad
    | PinLoaded
    | PinStore;


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

export const jwtTokenSet = (jwtToken: Nullable<JwtToken>): JwtTokenSet => {
    return {
        type: AuthActionType.jwtTokenSet,
        jwtToken,
    };
};

export const loadRefreshToken = (): LoadRefreshToken => {
    return {
        type: AuthActionType.loadRefreshToken,
    };
};

export const refreshTokenLoaded = (refreshToken: string | null): RefreshTokenLoaded => {
    return {
        type: AuthActionType.refreshTokenLoaded,
        refreshToken,
    };
};

export const loadPin = (): PinLoad => {
    return {
        type: AuthActionType.pinLoad,
    };
};

export const pinLoaded = (pin: string | null): PinLoaded => {
    return {
        type: AuthActionType.pinLoaded,
        pin,
    };
};

export const pinStore = (pin: string): PinStore => {
    return {
        type: AuthActionType.pinStore,
        pin,
    };
};
