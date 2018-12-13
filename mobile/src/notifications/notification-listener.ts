/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, combineEpics, ofType, StateObservable } from 'redux-observable';
import { AuthActionType, UserLoggedIn } from '../reducers/auth/auth.action';
import { catchError, ignoreElements, switchMap } from 'rxjs/operators';
import Push from 'appcenter-push';
import { EMPTY, from, Subject } from 'rxjs';
import { Action } from 'redux';
import { openProfile } from '../navigation/navigation.actions';
import { AppState, DependenciesContainer } from '../reducers/app.reducer';
import AppCenter from 'appcenter';

//----------------------------------------------------------------------------
const notificationsHandler$ = (action$: ActionsObservable<UserLoggedIn>) =>
    action$.ofType(AuthActionType.userLoggedIn).pipe(
        switchMap(() => {
            const notification$ = new Subject<Action>();

            Push.setListener({
                onPushNotificationReceived: notification => {
                    if (!notification.customProperties) {
                        return;
                    }

                    notification$.next(openProfile());
                },
            });

            return notification$;
        }),
    );

//----------------------------------------------------------------------------
const notificationsRegister$ = (action$: ActionsObservable<UserLoggedIn>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType(AuthActionType.userLoggedIn),
    switchMap(() => {
        return from(AppCenter.getInstallId());
    }),
    switchMap(installId => {
        return deps.apiClient.put(`/push/device/${installId}`);
    }),
    catchError(error => {
        console.warn(error);
        return EMPTY;
    }),
    ignoreElements(),
);

//----------------------------------------------------------------------------
export async function notificationsUnregister(deps: DependenciesContainer) {
    const installId = await AppCenter.getInstallId();
    await deps.apiClient.delete(`/push/device/${installId}`).toPromise();
}

//----------------------------------------------------------------------------
export const notifications$ = combineEpics(
    notificationsHandler$,
    notificationsRegister$,
);
