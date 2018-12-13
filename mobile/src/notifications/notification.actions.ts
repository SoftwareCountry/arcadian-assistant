/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Action } from 'redux';

//============================================================================
export enum NotificationActionType {
    registered = 'NotificationActionType.registered',
}


//============================================================================
// - Actions
//============================================================================

export interface RegisteredForNotifications extends Action {
    type: NotificationActionType.registered;
    installId: string;
}

export type NotificationAction = RegisteredForNotifications;


//============================================================================
// - Action Creators
//============================================================================

export const registeredForNotifications = (installId: string): RegisteredForNotifications => {
    return {
        type: NotificationActionType.registered,
        installId,
    };
};
