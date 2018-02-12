import { Reducer } from 'redux';
import { CalendarEvents } from './calendar-events';
import { UserActions } from '../user/user.action';
import { CalendarActions } from './calendar.action';

export const calendarEventsReducer: Reducer<CalendarEvents[]> = (state = [], action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-CALENDAR-EVENTS-FINISHED':
            return action.calendarEvents;
        default:
            return state;
    }
};