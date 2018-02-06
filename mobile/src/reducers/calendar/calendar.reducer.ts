import { combineReducers } from 'redux';
import { combineEpics, ActionsObservable } from 'redux-observable';
import { DaysCountersModel, VacationDaysCounter, HoursCreditCounter } from './days-counters.model';
import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';

interface DaysCountersState {
    allVacationDays: VacationDaysCounter;
    hoursCredit: HoursCreditCounter;
}

const initState: DaysCountersState = {
    allVacationDays: null,
    hoursCredit: null
};

export const daysCountersReducer: Reducer<DaysCountersState> = (state = initState, action: CalendarActions) => {
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
    daysCounters: DaysCountersState;
}

export const calendarReducer = combineReducers<CalendarState>({
    daysCounters: daysCountersReducer
});