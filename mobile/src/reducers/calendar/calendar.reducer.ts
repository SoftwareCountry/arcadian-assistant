import { combineReducers } from 'redux';
import { combineEpics, ActionsObservable } from 'redux-observable';
import { DaysCountersModel } from './days-counters.model';
import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';

export const daysCountersReducer: Reducer<DaysCountersModel> = (state = { allVacationDays: null, hoursCredit: null }, action: CalendarActions) => {
    switch (action.type) {
        case 'CALCULATE-DAYS-COUNTERS':
            return {
                allVacationDays: action.daysCounters.allVacationDays,
                hoursCredit: action.daysCounters.hoursCredit,
            };

        default:
            return state;
    }
};

export interface CalendarState {
    daysCounters: DaysCountersModel;
}

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer
});