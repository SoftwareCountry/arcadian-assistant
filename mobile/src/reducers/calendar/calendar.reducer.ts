import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadDaysCountersEpic$ } from './calendar.epics';
import { DaysCounterItem, DaysCountersModel } from './days-counters.model';
import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';

export const daysCountersReducer: Reducer<DaysCountersModel> = (state = { allVacationDays: null, daysOff: null }, action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-DAYS-COUNTERS-FINISHED':
            return {
                allVacationDays: action.daysCounters.allVacationDays,
                daysOff: action.daysCounters.daysOff,
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