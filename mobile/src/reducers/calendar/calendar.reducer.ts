import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadCalendarEventsFinishedEpic$ } from './calendar.epics';
import { calendarEventsReducer } from './calendar-events.reducer';
import { CalendarEvents } from './calendar-events';
import { DaysCountersState, daysCountersReducer } from './days-counters.reducer';

export interface CalendarState {
    daysCounters: DaysCountersState;
    calendarEvents: CalendarEvents[];
}

export const calendarEpics = combineEpics(loadCalendarEventsFinishedEpic$ as any);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer,
    calendarEvents: calendarEventsReducer
});
