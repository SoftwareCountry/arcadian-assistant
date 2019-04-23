/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, combineEpics, ofType, StateObservable } from 'redux-observable';
import { AuthActionType, UserLoggedIn } from '../reducers/auth/auth.action';
import { ignoreElements, map, switchMap } from 'rxjs/operators';
import Push from 'appcenter-push';
import { from, Observable } from 'rxjs';
import { Action } from 'redux';
import { openEmployeeDetails, openProfile } from '../navigation/navigation.actions';
import { AppState, DependenciesContainer } from '../reducers/app.reducer';
import AppCenter from 'appcenter';
import { installIdReceived, NotificationAction, NotificationActionType } from './notification.actions';
import { handleHttpErrors, retryDelayed } from '../errors/error.operators';
import { Platform } from 'react-native';
import { loadPendingRequests } from '../reducers/calendar/pending-requests/pending-requests.action';
import { loadCalendarEvents } from '../reducers/calendar/calendar.action';
import { logError } from '../utils/analytics';
import { LoadUserFinished } from '../reducers/user/user.action';

//----------------------------------------------------------------------------
const notificationsHandler$ = (action$: ActionsObservable<LoadUserFinished>, state$: StateObservable<AppState>) =>
    action$.ofType('LOAD-USER-FINISHED').pipe(
        switchMap(() => {
            return new Observable<Action>(observer => {
                // noinspection JSIgnoredPromiseFromCall
                Push.setListener({
                    onPushNotificationReceived: notification => {

                        console.log(`Notification: ${JSON.stringify(notification)}`);

                        const userInfo = state$.value.userInfo;
                        if (!userInfo || !userInfo.employeeId) {
                            console.log(`Notification: no user info`);
                            return;
                        }

                        const isAndroid = Platform.OS === 'android';
                        if (isAndroid && !!notification.message) {
                            // Android messages received in the background don't include a message. On Android, that fact can be used to
                            // check if the message was received in the background or foreground. For iOS the message is always present.
                            console.log(`Notification: message received in foreground on Android`);
                            return;
                        }

                        if (!notification.customProperties) {
                            console.log(`Notification: no custom payload`);
                            return;
                        }

                        const employeeId = notification.customProperties.employeeId;
                        const approverId = notification.customProperties.approverId;

                        observer.next(loadPendingRequests());

                        if (employeeId) {
                            observer.next(loadCalendarEvents(employeeId));
                            if (userInfo.employeeId === employeeId) {
                                observer.next(openProfile());
                            } else {
                                observer.next(openEmployeeDetails(employeeId));
                            }
                        } else if (approverId) {
                            if (userInfo.employeeId === approverId) {
                                observer.next(openProfile());
                            }
                        } else {
                            logError('Unexpected notification payload', notification.customProperties);
                        }
                    },
                });

                return () => {
                    // noinspection JSIgnoredPromiseFromCall
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
        console.log(`installId = ${action.installId}`);
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
            retryDelayed(),
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
