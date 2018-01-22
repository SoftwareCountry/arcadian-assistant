import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadDaysCountersEpic$ } from './calendar.epics';
import { DaysCounterItem, DaysCountersModel } from './days-counters.model';
import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';

export const daysCountersReducer: Reducer<DaysCountersModel> = (state = { vacation: null, off: null, sick: null }, action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-DAYS-COUNTERS-FINISHED':
            return {
                vacation: action.daysCounters.vacation,
                off: action.daysCounters.off,
                sick: action.daysCounters.sick
            };

        default:
            return state;
    }
};

export interface CalendarState {
    daysCounters: DaysCountersModel;
}

export const calendarEpics = combineEpics(loadDaysCountersEpic$);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer
});