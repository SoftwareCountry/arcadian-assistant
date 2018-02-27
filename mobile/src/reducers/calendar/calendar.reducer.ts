import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadCalendarEventsFinishedEpic$ } from './calendar.epics';
import { DaysCountersState, daysCountersReducer } from './days-counters.reducer';
import { calendarEventsReducer, CalendarEventsState } from './calendar-events.reducer';

export interface CalendarState {
    daysCounters: DaysCountersState;
    calendarEvents: CalendarEventsState;
}

export const calendarEpics = combineEpics(loadCalendarEventsFinishedEpic$ as any);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer,
    calendarEvents: calendarEventsReducer
});
