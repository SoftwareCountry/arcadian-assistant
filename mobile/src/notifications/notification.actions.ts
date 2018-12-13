/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Action } from 'redux';

//============================================================================
export enum NotificationActionType {
    installIdReceived = 'NotificationActionType.installIdReceived',
}


//============================================================================
// - Actions
//============================================================================

export interface InstallIdReceived extends Action {
    type: NotificationActionType.installIdReceived;
    installId: string;
}

export type NotificationAction = InstallIdReceived;


//============================================================================
// - Action Creators
//============================================================================

export const installIdReceived = (installId: string): InstallIdReceived => {
    return {
        type: NotificationActionType.installIdReceived,
        installId,
    };
};
