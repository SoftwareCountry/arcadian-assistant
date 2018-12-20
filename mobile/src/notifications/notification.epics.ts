/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, combineEpics, ofType, StateObservable } from 'redux-observable';
import { AuthActionType, UserLoggedIn } from '../reducers/auth/auth.action';
import { ignoreElements, map, switchMap } from 'rxjs/operators';
import Push from 'appcenter-push';
import { from, Observable } from 'rxjs';
import { Action } from 'redux';
import { openProfile } from '../navigation/navigation.actions';
import { AppState, DependenciesContainer } from '../reducers/app.reducer';
import AppCenter from 'appcenter';
import { installIdReceived, NotificationAction, NotificationActionType } from './notification.actions';
import { handleHttpErrors } from '../reducers/errors/errors.epics';
import { Platform } from 'react-native';

//----------------------------------------------------------------------------
const notificationsHandler$ = (action$: ActionsObservable<UserLoggedIn>, state$: StateObservable<AppState>) =>
    action$.ofType(AuthActionType.userLoggedIn).pipe(
        switchMap(() => {
            return new Observable<Action>(observer => {
                Push.setListener({
                    onPushNotificationReceived: notification => {
                        if (!notification.customProperties || !notification.customProperties.employeeId) {
                            return;
                        }

                        const userInfo = state$.value.userInfo;
                        if (!userInfo || notification.customProperties.employeeId !== userInfo.employeeId) {
                            return;
                        }

                        observer.next(openProfile());
                    },
                });

                return () => {
                    Push.setListener(undefined);
                };
            });
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
        const deviceType = Platform.select({
            ios: 'Ios',
            android: 'Android',
        });
        return deps.apiClient.post(`/push/device`,
                                   {
                                       devicePushToken: action.installId,
                                       deviceType,
                                   },
                                   { 'Content-Type': 'application/json' }).pipe(
            handleHttpErrors(true, 'Push Notifications will not be received. Please contact administrator'),
        );
    }),
    ignoreElements(),
);

//----------------------------------------------------------------------------
export async function notificationsUnregister(dependencies: DependenciesContainer, installId: string) {
    Push.setListener(undefined);
    await dependencies.apiClient.delete(`/push/device/${installId}`).toPromise();
}

//----------------------------------------------------------------------------
export const notifications$ = combineEpics(
    notificationsHandler$,
    getInstallId$,
    notificationsRegister$,
);
