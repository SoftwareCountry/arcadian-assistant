import { Reducer } from 'redux';
import { Map } from 'immutable';
import { CalendarActions, CalendarSelectionModeType, CalendarSelectionMode, NextCalendarPage } from './calendar.action';
import { DayModel, WeekModel, IntervalsModel, CalendarSelection, IntervalModel, ExtractedIntervals, ReadOnlyIntervalsModel, CalendarPageModel } from './calendar.model';
import moment, { Moment } from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarEvents } from './calendar-events.model';
import { singleDaySelectionReducer, intervalSelectionReducer } from './calendar-selection.reducer';
import { calendarSelectionModeReducer } from './calendar-selection-mode.reducer';
import { CalendarEvent } from './calendar-event.model';
import { UserActions } from '../user/user.action';
import { UserInfoState } from '../user/user-info.reducer';
import { nextCalendarPageReducer } from './next-calendar-page.reducer';
import { prevCalendarPageReducer } from './prev-calendar-page.reducer';
import { createCalendarPagesInitState } from './calendar-pages-init-state';
import { Optional } from 'types';


export interface IntervalsSubState {
    intervals: Optional<ReadOnlyIntervalsModel>;
}

export interface DisableCalendarDaysBeforeSubState {
    disableCalendarDaysBefore: Optional<DayModel>;
}

export interface SelectionSubState {
    selection: CalendarSelection;
}

export interface EventsMapSubState {
    events: Map<string, CalendarEvent[]>;
    userEmployeeId: Optional<string>;
}

export interface CalendarPagesSubState {
    pages: CalendarPageModel[];
}

export interface CalendarEventsState extends
    IntervalsSubState,
    DisableCalendarDaysBeforeSubState,
    EventsMapSubState,
    SelectionSubState,
    CalendarPagesSubState {
        disableCalendarActionsButtonGroup: boolean;
        selectedIntervalsBySingleDaySelection: ExtractedIntervals;
        disableSelectIntervalsBySingleDaySelection: boolean;
        disableSelection: boolean;
}

const createInitState = (): CalendarEventsState => {
    const date = moment();
    const [
        prevPage,
        currentPage,
        nextPage
     ] = createCalendarPagesInitState(date);

    let todayModel: Optional<DayModel> = null;
    for (let week of currentPage.weeks) {
        todayModel = week.days.find(day => day.today);

        if (todayModel) {
            break;
        }
    }

    const defaultSelection: CalendarSelection = {
        single: {
            day: todayModel
        },
        interval: null
    };

    const defaultExtractedIntervals = new ExtractedIntervals(null);

    return {
        pages: [prevPage, currentPage, nextPage],
        intervals: null,
        events: Map<string, CalendarEvent[]>(),
        userEmployeeId: null,
        disableCalendarDaysBefore: null,
        disableCalendarActionsButtonGroup: true,
        selection: defaultSelection,
        selectedIntervalsBySingleDaySelection: defaultExtractedIntervals,
        disableSelectIntervalsBySingleDaySelection: false,
        disableSelection: false
    };
};

const initState = createInitState();

export const calendarEventsReducer: Reducer<CalendarEventsState> = (state = initState, action: CalendarActions | UserActions) => {
    switch (action.type) {
        case 'LOAD-USER-FINISHED':
            return {...state, userEmployeeId: action.userEmployeeId};
        case 'LOAD-CALENDAR-EVENTS-FINISHED':
            let newState: CalendarEventsState;
            let {events} = state;

            events = events.set(action.employeeId, action.calendarEvents.all);

            newState = {
                ...state,
                events: events
            };

            if (action.employeeId === state.userEmployeeId) {
                const intervals = action.calendarEvents.buildIntervalsModel();

                newState = {
                    ...newState,
                    intervals: intervals,
                    disableCalendarActionsButtonGroup: false,
                };
            }

            return newState;
        case 'SELECT-CALENDAR-DAY':
            const singleDayState = singleDaySelectionReducer(state, action);
            const intervalState = intervalSelectionReducer(state, action);

            return {
                ...state,
                ...singleDayState,
                ...intervalState
            };
        case 'NEXT-CALENDAR-PAGE':
            const nextCalendarPageState = nextCalendarPageReducer(state, action);

            return {
                ...state,
                ...nextCalendarPageState
            };
        case 'PREV-CALENDAR-PAGE':
            const prevCalendarPageState = prevCalendarPageReducer(state, action);

            return {
                ...state,
                ...prevCalendarPageState
            };
        case 'CALENDAR-SELECTION-MODE':
            const selectionState = calendarSelectionModeReducer(state, action);

            return {
                ...state,
                ...selectionState
            };
        case 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION':
            if (state.disableSelectIntervalsBySingleDaySelection) {
                return state;
            }

            const intervalsBySingleDay = state.intervals && state.selection.single.day
                ? state.intervals.get(state.selection.single.day.date)
                : null;

            const extractedIntervals = new ExtractedIntervals(intervalsBySingleDay);

            return {
                ...state,
                selectedIntervalsBySingleDaySelection: extractedIntervals
            };
        case 'DISABLE-CALENDAR-SELECTION':
            return {
                ...state,
                disableSelection: action.disable
            };
        case 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION':
            return {
                ...state,
                disableSelectIntervalsBySingleDaySelection: action.disable
            };
        default:
            return state;
    }
};
