import { CalendarEventsState, CalendarPagesSubState } from './calendar-events.reducer';
import { PrevCalendarPage, ResetCalendarPages } from './calendar.action';
import moment from 'moment';
import { createCalendarPagesInitState } from './calendar-pages-init-state';

export const resetCalendarPagesReducer = (state: CalendarEventsState, action: ResetCalendarPages): CalendarPagesSubState => {
    return { pages: createCalendarPagesInitState(moment()) };
};
