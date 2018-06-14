import { CalendarEvent } from '../calendar-event.model';
import { Map } from 'immutable';

export interface LoadPendingRequests {
    type: 'LOAD-PENDING-REQUESTS';
}

export const loadPendingRequests = (): LoadPendingRequests => ({ type: 'LOAD-PENDING-REQUESTS' });

export interface LoadPendingRequestsFinished {
    type: 'LOAD-PENDING-REQUESTS-FINISHED';
    requests: Map<string, CalendarEvent[]>;
}

export const loadPendingRequestsFinished = (requests: Map<string, CalendarEvent[]>): LoadPendingRequestsFinished => ({ type: 'LOAD-PENDING-REQUESTS-FINISHED', requests });

export type PendingRequestsActions = LoadPendingRequestsFinished | LoadPendingRequests;