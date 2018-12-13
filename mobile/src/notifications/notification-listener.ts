/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, combineEpics, ofType, StateObservable } from 'redux-observable';
import { AuthActionType, UserLoggedIn } from '../reducers/auth/auth.action';
import { catchError, map, switchMap } from 'rxjs/operators';
import Push from 'appcenter-push';
import { EMPTY, from, of, Subject } from 'rxjs';
import { Action } from 'redux';
import { openProfile } from '../navigation/navigation.actions';
import { AppState, DependenciesContainer } from '../reducers/app.reducer';
import AppCenter from 'appcenter';
import { registeredForNotifications } from './notification.actions';

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
        return deps.apiClient.put(`/push/device/${installId}`).pipe(
            map(() => installId),
            catchError(error => {
                console.warn(error);
                return of(installId);
            })
        );
    }),
    map(installId => registeredForNotifications(installId)),
);

//----------------------------------------------------------------------------
export async function notificationsUnregister(dependencies: DependenciesContainer, installId: string) {
    await dependencies.apiClient.delete(`/push/device/${installId}`).toPromise();
}

//----------------------------------------------------------------------------
export const notifications$ = combineEpics(
    notificationsHandler$,
    notificationsRegister$,
);
