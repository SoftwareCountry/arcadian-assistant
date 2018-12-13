/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable } from 'redux-observable';
import { AuthActionType, UserLoggedIn } from '../reducers/auth/auth.action';
import { switchMap } from 'rxjs/operators';
import Push from 'appcenter-push';
import { Subject } from 'rxjs';
import { Action } from 'redux';
import { openProfile } from '../navigation/navigation.actions';

//----------------------------------------------------------------------------
export const notificationsHandler$ = (action$: ActionsObservable<UserLoggedIn>) =>
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
