import { CalendarEvent } from '../calendar-event.model';
import { Map } from 'immutable';
import { Action } from 'redux';

export interface LoadPendingRequests extends Action {
    type: 'LOAD-PENDING-REQUESTS';
}

export const loadPendingRequests = (): LoadPendingRequests => ({ type: 'LOAD-PENDING-REQUESTS' });

export interface LoadPendingRequestsFinished extends Action {
    type: 'LOAD-PENDING-REQUESTS-FINISHED';
    requests: Map<string, CalendarEvent[]>;
}

export const loadPendingRequestsFinished = (requests: Map<string, CalendarEvent[]>): LoadPendingRequestsFinished => ({
    type: 'LOAD-PENDING-REQUESTS-FINISHED',
    requests
});

export type PendingRequestsActions = LoadPendingRequestsFinished | LoadPendingRequests;
