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
import { Linking, Platform } from 'react-native';
import { loadCalendarEvents } from '../reducers/calendar/calendar.action';
import { logError } from '../utils/analytics';
import { LoadUserFinished, UserActionType } from '../reducers/user/user.action';
import { NotificationType } from './notifications';
import { loadPendingRequests } from '../reducers/calendar/pending-requests/pending-requests.action';
import config from '../config';

//----------------------------------------------------------------------------
const notificationsHandler$ = (action$: ActionsObservable<LoadUserFinished>, state$: StateObservable<AppState>) =>
    action$.ofType(UserActionType.loadUserFinished).pipe(
        switchMap(() => {
            return new Observable<Action>(observer => {
                // noinspection JSIgnoredPromiseFromCall
                Push.setListener({
                    onPushNotificationReceived: notification => {

                        if (Platform.OS === 'android' && !!notification.message) {
                            // Android messages received in the background don't include a message. On Android, that fact can be used to
                            // check if the message was received in the background or foreground. For iOS the message is always present.
                            return;
                        }

                        if (!state$.value.userInfo || !state$.value.userInfo.employeeId ||
                            !notification.customProperties || !notification.customProperties.type) {
                            return;
                        }

                        const currentEmployeeId = state$.value.userInfo.employeeId;
                        const { type, employeeId, approverId } = notification.customProperties;

                        switch (type) {
                            case NotificationType.UpdateAvailable:
                                Linking.openURL(config.downloadNewVersionLink).catch(err => console.error(err));
                                break;
                            case NotificationType.EventAssignedToApprover:
                            case NotificationType.EventStatusChanged:
                            case NotificationType.EventUserGrantedApproval:
                            case NotificationType.SickLeaveCancelledManager:
                            case NotificationType.SickLeaveCreatedManager:
                            case NotificationType.SickLeaveProlongedManager:
                                observer.next(loadPendingRequests());
                                if (employeeId) {
                                    observer.next(loadCalendarEvents(employeeId));
                                    if (currentEmployeeId === employeeId) {
                                        observer.next(openProfile());
                                    } else {
                                        observer.next(openEmployeeDetails(employeeId));
                                    }
                                } else if (approverId) {
                                    if (currentEmployeeId === approverId) {
                                        observer.next(openProfile());
                                    }
                                }
                                break;
                            default:
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
