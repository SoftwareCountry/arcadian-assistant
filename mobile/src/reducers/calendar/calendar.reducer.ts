import { combineReducers } from 'redux';
import { combineEpics } from 'redux-observable';
import { loadDaysEpic$ } from './calendar.epics';
import { DaysItem, Days } from './days.model';
import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';

export const daysReducer: Reducer<Days> = (state = { vacation: null, off: null, sick: null }, action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-DAYS-FINISHED':
            return {
                vacation: action.days.vacation,
                off: action.days.off,
                sick: action.days.sick
            };

        default:
            return state;
    }
};

export interface CalendarState {
    days: Days;
}

export const calendarEpics = combineEpics(loadDaysEpic$);

export const calendarReducer = combineReducers<CalendarState>({
    days: daysReducer
});