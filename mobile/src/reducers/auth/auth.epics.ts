/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable } from 'redux-observable';
import { DependenciesContainer } from '../app.reducer';
import {
    AuthActionType,
    jwtTokenSet, PinLoad, LoadRefreshToken, pinLoaded, refreshTokenLoaded,
    UserLoggedIn, PinStore,
} from './auth.action';
import { refresh } from '../refresh/refresh.action';
import { ignoreElements, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { fromPromise } from 'rxjs/internal-compatibility';
import { switchMap } from 'rxjs/internal/operators/switchMap';
import { tap } from 'rxjs/internal/operators/tap';

export const shouldRefreshEpic$ = (action$: ActionsObservable<UserLoggedIn>) =>
    action$.ofType(AuthActionType.userLoggedIn).pipe(
        map(() => refresh())
    );

export const jwtTokenEpic$ = (_actions: unknown, _state: unknown, dep: DependenciesContainer) =>
    dep.oauthProcess.jwtTokenHandler.get$().pipe(
        map(jwtTokenSet)
    );

export const loadRefreshToken$ = (action$: ActionsObservable<LoadRefreshToken>, _: unknown, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.loadRefreshToken).pipe(
        switchMap(_ => {
            return fromPromise(dep.oauthProcess.getRefreshToken());
        }),
        map(refreshToken => {
            return refreshTokenLoaded(refreshToken);
        }),
    );

export const loadPin$ = (action$: ActionsObservable<PinLoad>, _: unknown, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.pinLoad).pipe(
        switchMap(_ => {
            return fromPromise(dep.storage.getPin());
        }),
        map(pin => {
            return pinLoaded(pin);
        }),
    );

export const storePin$ = (action$: ActionsObservable<PinStore>, _: unknown, dep: DependenciesContainer) =>
    action$.ofType(AuthActionType.pinStore).pipe(
        tap(action => {
            return fromPromise(dep.storage.setPin(action.pin));
        }),
        ignoreElements(),
    );
