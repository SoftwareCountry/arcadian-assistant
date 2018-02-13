import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadCalendarEventsFinishedEpic$ } from './calendar.epics';
import { calendarEventsReducer } from './calendar-events.reducer';
import { CalendarEvents } from './calendar-events';
import { DaysCountersState, daysCountersReducer } from './days-counters.reducer';
import { selectCalendarDayReducer } from './select-calendar-day.reducer';
import { DayModel } from '../../calendar/calendar-page';

export interface CalendarState {
    daysCounters: DaysCountersState;
    calendarEvents: CalendarEvents[];
    selectedCalendarDay: DayModel;
}

export const calendarEpics = combineEpics(loadCalendarEventsFinishedEpic$ as any);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer,
    calendarEvents: calendarEventsReducer,
    selectedCalendarDay: selectCalendarDayReducer
});
