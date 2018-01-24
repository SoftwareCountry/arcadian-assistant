import { combineReducers } from 'redux';
import { combineEpics, ActionsObservable } from 'redux-observable';
import { loadDaysCountersEpic, loadDaysFinishedEpic } from './calendar.epics';
import { DaysCountersModel } from './days-counters.model';
import { Reducer } from 'redux';
import { CalendarActions, LoadDaysCountersFinished, LoadDaysCounters } from './calendar.action';

export const daysCountersReducer: Reducer<DaysCountersModel> = (state = { allVacationDays: null, daysOff: null }, action: CalendarActions) => {
    switch (action.type) {
        case 'CALCULATE-DAYS-COUNTERS':
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

export const calendarEpics = combineEpics(loadDaysCountersEpic as any, loadDaysFinishedEpic as any);

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer
});