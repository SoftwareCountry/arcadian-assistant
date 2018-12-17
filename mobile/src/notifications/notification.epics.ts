/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, combineEpics, ofType, StateObservable } from 'redux-observable';
import { AuthActionType, UserLoggedIn } from '../reducers/auth/auth.action';
import { ignoreElements, map, switchMap } from 'rxjs/operators';
import Push from 'appcenter-push';
import { from, Subject } from 'rxjs';
import { Action } from 'redux';
import { openProfile } from '../navigation/navigation.actions';
import { AppState, DependenciesContainer } from '../reducers/app.reducer';
import AppCenter from 'appcenter';
import { installIdReceived, NotificationAction, NotificationActionType } from './notification.actions';
import { handleHttpErrors } from '../reducers/errors/errors.epics';

//----------------------------------------------------------------------------
const notificationsHandler$ = (action$: ActionsObservable<UserLoggedIn>, state$: StateObservable<AppState>) =>
    action$.ofType(AuthActionType.userLoggedIn).pipe(
        switchMap(() => {
            const notification$ = new Subject<Action>();

            Push.setListener({
                onPushNotificationReceived: notification => {
                    if (!notification.customProperties || !notification.customProperties.employeeId) {
                        return;
                    }

                    const userInfo = state$.value.userInfo;
                    if (!userInfo || notification.customProperties.employeeId !== userInfo.employeeId) {
                        return;
                    }

                    notification$.next(openProfile());
                },
            });

            return notification$;
        }),
    );

//----------------------------------------------------------------------------
const getInstallId$ = (action$: ActionsObservable<UserLoggedIn>) => action$.pipe(
    ofType(AuthActionType.userLoggedIn),
    switchMap(() => {
        return from(AppCenter.getInstallId());
    }),
    map(installId => installIdReceived(installId)),
);

//----------------------------------------------------------------------------
const notificationsRegister$ = (action$: ActionsObservable<NotificationAction>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType(NotificationActionType.installIdReceived),
    switchMap(action => {
        return deps.apiClient.post(`/push/device`, {
            devicePushToken: action.installId,
        }).pipe(
            handleHttpErrors(true, 'Push Notifications will not be received. Please contact administrator'),
        );
    }),
    ignoreElements(),
);

//----------------------------------------------------------------------------
export async function notificationsUnregister(dependencies: DependenciesContainer, installId: string) {
    await dependencies.apiClient.delete(`/push/device/${installId}`).toPromise();
}

//----------------------------------------------------------------------------
export const notifications$ = combineEpics(
    notificationsHandler$,
    getInstallId$,
    notificationsRegister$,
);
