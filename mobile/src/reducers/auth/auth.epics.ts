/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable } from 'redux-observable';
import { DependenciesContainer } from '../app.reducer';
import {
    AuthActionType,
    jwtTokenSet,
    UserLoggedIn,
} from './auth.action';
import { refresh } from '../refresh/refresh.action';
import { map } from 'rxjs/operators';

export const shouldRefreshEpic$ = (action$: ActionsObservable<UserLoggedIn>) =>
    action$.ofType(AuthActionType.userLoggedIn).pipe(
        map(() => refresh())
    );

export const jwtTokenEpic$ = (_actions: unknown, _state: unknown, dep: DependenciesContainer) =>
    dep.oauthProcess.jwtTokenHandler.get$().pipe(
        map(jwtTokenSet)
    );