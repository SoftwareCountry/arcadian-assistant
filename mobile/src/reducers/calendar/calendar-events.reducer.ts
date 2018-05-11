import { Reducer } from 'redux';
import { Map } from 'immutable';
import { CalendarActions, CalendarSelectionModeType, CalendarSelectionMode } from './calendar.action';
import { DayModel, WeekModel, IntervalsModel, CalendarSelection, IntervalModel, ExtractedIntervals, ReadOnlyIntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarEvents } from './calendar-events.model';
import { singleDaySelectionReducer, intervalSelectionReducer } from './calendar-selection.reducer';
import { calendarSelectionModeReducer } from './calendar-selection-mode.reducer';
import { CalendarEvent } from './calendar-event.model';
import { UserActions } from '../user/user.action';

export interface IntervalsSubState {
    intervals: ReadOnlyIntervalsModel;
}

export interface DisableCalendarDaysBeforeSubState {
    disableCalendarDaysBefore: DayModel;
}

export interface SelectionSubState {
    selection: CalendarSelection;
}

export interface EventsMapSubState {
    events: Map<string, CalendarEvent[]>;
    userEmployeeId: string;
    eventsPredicate: (event: CalendarEvent) => boolean;
}

export interface CalendarEventsState extends
    IntervalsSubState,
    DisableCalendarDaysBeforeSubState,
    EventsMapSubState,
    SelectionSubState {
        weeks: WeekModel[];
        disableCalendarActionsButtonGroup: boolean;
        selectedIntervalsBySingleDaySelection: ExtractedIntervals;
        disableSelectIntervalsBySingleDaySelection: boolean;
        disableSelection: boolean;
}

const createInitState = (): CalendarEventsState => {
    const builder = new CalendarWeeksBuilder();
    const today = moment();
    const weeks = builder.buildWeeks(today.month(), today.year());

    let todayModel: DayModel = null;
    for (let week of weeks) {
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
        weeks: weeks,
        intervals: null,
        events: Map<string, CalendarEvent[]>(),
        userEmployeeId: null,
        eventsPredicate: (event: CalendarEvent) => {
            const now = moment();
            return event.dates.endDate.isSameOrAfter(now, 'date');
        },
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
        case 'LOAD-USER-EMPLOYEE-FINISHED':
            return {...state, userEmployeeId: action.employee.employeeId};
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
        case 'SELECT-CALENDAR-MONTH':
            const builder = new CalendarWeeksBuilder();
            const weeks = builder.buildWeeks(action.month, action.year);

            return {
                ...state,
                weeks: weeks
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

            const intervalsBySingleDay = state.intervals
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