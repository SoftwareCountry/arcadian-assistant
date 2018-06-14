import { CalendarEvent } from '../calendar-event.model';
import { Map } from 'immutable';
import { IntervalTypeConverter } from '../interval-type-converter';
import { PendingRequestsActions } from './pending-requests.action';

export interface PendingRequestsState {
    requests: Map<string, CalendarEvent[]>;
    hoursToIntervalTitle: (startWorkingHour: number, finishWorkingHour: number) => string;
}

const initState: PendingRequestsState = {
    requests: Map<string, CalendarEvent[]>(),
    hoursToIntervalTitle: IntervalTypeConverter.hoursToIntervalTitle,
};

export const pendingRequestsReducer = (state: PendingRequestsState = initState, action: PendingRequestsActions): PendingRequestsState => {
    switch (action.type) {
        case 'LOAD-PENDING-REQUESTS-FINISHED':
            return {
                ...state,
                requests: action.requests
            };
        default:
            return state;
    }
};