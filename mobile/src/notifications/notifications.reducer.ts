import { Reducer } from 'redux';
import { NotificationAction, NotificationActionType } from './notification.actions';

/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

//============================================================================
export interface NotificationState {
    installId?: string;
}

//============================================================================
export const notificationsReducer: Reducer<NotificationState, NotificationAction> = (state = {}, action) => {
    switch (action.type) {
        case NotificationActionType.registered:
            return {
                ...state,
                installId: action.installId,
            };

        default:
            return state;
    }
};

