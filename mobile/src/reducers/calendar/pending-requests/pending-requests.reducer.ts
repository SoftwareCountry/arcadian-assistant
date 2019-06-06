/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { CalendarEvent } from '../calendar-event.model';
import { Map } from 'immutable';
import { IntervalTypeConverter } from '../interval-type-converter';
import { PendingRequestsActions } from './pending-requests.action';
import { Nullable } from 'types';
import { ApprovalAction, ApprovalActionType } from '../approval.action';

//============================================================================
export interface PendingRequestsState {
    requestsAreLoading: boolean;
    requests: Map<string, CalendarEvent[]>;
    hoursToIntervalTitle: (startWorkingHour: number, finishWorkingHour: number) => Nullable<string>;
}

//----------------------------------------------------------------------------
const initState: PendingRequestsState = {
    requestsAreLoading: true,
    requests: Map(),
    hoursToIntervalTitle: IntervalTypeConverter.hoursToIntervalTitle,
};

//----------------------------------------------------------------------------
export const pendingRequestsReducer = (state: PendingRequestsState = initState, action: PendingRequestsActions | ApprovalAction): PendingRequestsState => {
    switch (action.type) {
        case 'LOAD-PENDING-REQUESTS':
            return {
                ...state,
                requestsAreLoading: true,
            };

        case 'LOAD-PENDING-REQUESTS-FINISHED':
            return {
                ...state,
                requests: action.requests,
                requestsAreLoading: false,
            };

        case 'LOAD-PENDING-REQUESTS-FAILED':
            return {
                ...state,
                requestsAreLoading: false,
            };

        case ApprovalActionType.approveFinished:

            const requests = state.requests
                .map(events => events.filter(event => event.calendarEventId !== action.approval.eventId))
                .filter(events => events.length > 0);

            return {
                ...state,
                requests: requests,
            };

        default:
            return state;
    }
};
