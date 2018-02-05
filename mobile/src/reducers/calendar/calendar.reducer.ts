import { combineReducers } from 'redux';
import { combineEpics, ActionsObservable } from 'redux-observable';
import { DaysCountersModel, VacationDaysCounter, HoursCreditCounter, TodayCounter } from './days-counters.model';
import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';

interface DaysCountersState {
    allVacationDays: VacationDaysCounter;
    hoursCredit: HoursCreditCounter;
    today: TodayCounter;
}

const initState: DaysCountersState = {
    allVacationDays: null,
    hoursCredit: null,
    today: null
};

export const daysCountersReducer: Reducer<DaysCountersState> = (state = initState, action: CalendarActions) => {
    switch (action.type) {
        case 'CALCULATE-DAYS-COUNTERS':
            return {
                ...state,
                allVacationDays: action.daysCounters.allVacationDays,
                hoursCredit: action.daysCounters.hoursCredit,
            };
        case 'CALCULATE-TODAY-COUNTER':
            return {
                ...state,
                today: action.today
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