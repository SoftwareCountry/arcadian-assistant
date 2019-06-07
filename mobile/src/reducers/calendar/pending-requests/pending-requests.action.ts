/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { CalendarEvent } from '../calendar-event.model';
import { Map } from 'immutable';
import { Action } from 'redux';

//============================================================================
// - Actions
//============================================================================
export interface LoadPendingRequests extends Action {
    type: 'LOAD-PENDING-REQUESTS';
}

export interface LoadPendingRequestsFinished extends Action {
    type: 'LOAD-PENDING-REQUESTS-FINISHED';
    requests: Map<string, CalendarEvent[]>;
}

export interface LoadPendingRequestsFailed extends Action {
    type: 'LOAD-PENDING-REQUESTS-FAILED';
}

//============================================================================
// - Action Creators
//============================================================================
export const loadPendingRequests = (): LoadPendingRequests => ({
    type: 'LOAD-PENDING-REQUESTS'
});

export const loadPendingRequestsFinished = (requests: Map<string, CalendarEvent[]>): LoadPendingRequestsFinished => ({
    type: 'LOAD-PENDING-REQUESTS-FINISHED',
    requests
});

export const loadPendingRequestsFailed = (): LoadPendingRequestsFailed => ({
    type: 'LOAD-PENDING-REQUESTS-FAILED',
});

export type PendingRequestsActions = LoadPendingRequests | LoadPendingRequestsFinished | LoadPendingRequestsFailed;
