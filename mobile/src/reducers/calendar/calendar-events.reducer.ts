import { Reducer } from 'redux';
import { CalendarActions, CalendarSelectionModeType, CalendarSelectionMode } from './calendar.action';
import { DayModel, WeekModel, IntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { singleDaySelectionReducer, intervalSelectionReducer } from './calendar-selection.reducer';
import { calendarSelectionModeReducer } from './calendar-selection-mode.reducer';

export interface IntervalsSubState {
    intervals: IntervalsModel;
}

export interface DisableDaysCalendarDaysBeforeSubState {
    disableCalendarDaysBefore: DayModel;
}

export interface SelectionState {
    mode: CalendarSelectionModeType;
    startDay: DayModel;
    endDay: DayModel;
    color: string;
}

export interface SelectionSubState {
    selection: SelectionState;
}

export interface CalendarEventsState extends
    IntervalsSubState,
    DisableDaysCalendarDaysBeforeSubState,
    SelectionSubState {
        weeks: WeekModel[];
        selectedCalendarDay: DayModel;
        disableCalendarActionsButtonGroup: boolean;
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

    const defaultSelection: SelectionState = {
        mode: CalendarSelectionModeType.SingleDay,
        startDay: null,
        endDay: null,
        color: null
    };

    return {
        weeks: weeks,
        selectedCalendarDay: todayModel,
        intervals: null,
        disableCalendarDaysBefore: null,
        disableCalendarActionsButtonGroup: true,
        selection: defaultSelection
    };
};

const initState = createInitState();

export const calendarEventsReducer: Reducer<CalendarEventsState> = (state = initState, action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-CALENDAR-EVENTS-FINISHED':
            const builderIntervals = new CalendarIntervalsBuilder();
            const intervals = builderIntervals.buildIntervals(action.calendarEvents);

            return {
                ...state,
                intervals: intervals,
                disableCalendarActionsButtonGroup: false
            };
        case 'CALENDAR-EVENT-CREATED':
            const builderCalendarIntervals = new CalendarIntervalsBuilder();

            const intervalsWithNewEvent = state.intervals
                ? state.intervals.copy()
                : new IntervalsModel();

            builderCalendarIntervals.appendCalendarEvents(intervalsWithNewEvent, [action.calendarEvent]);

            return {
                ...state,
                intervals: intervalsWithNewEvent
            };
        case 'SELECT-CALENDAR-DAY':
            const singleDayState = singleDaySelectionReducer(state, action);
            const intervalState = intervalSelectionReducer(state, action);

            return {
                ...state,
                ...singleDayState,
                ...intervalState,
                selectedCalendarDay: action.day
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
        default:
            return state;
    }
};