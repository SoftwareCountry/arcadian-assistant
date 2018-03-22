import { Reducer } from 'redux';
import { CalendarActions, CalendarSelectionModeType, CalendarSelectionMode } from './calendar.action';
import { DayModel, WeekModel, IntervalsModel, CalendarSelection, IntervalModel, ExtractedIntervals, ReadOnlyIntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarEvents } from './calendar-events.model';
import { singleDaySelectionReducer, intervalSelectionReducer } from './calendar-selection.reducer';
import { calendarSelectionModeReducer } from './calendar-selection-mode.reducer';

export interface IntervalsSubState {
    intervals: ReadOnlyIntervalsModel;
}

export interface DisableDaysCalendarDaysBeforeSubState {
    disableCalendarDaysBefore: DayModel;
}

export interface SelectionSubState {
    selection: CalendarSelection;
}

export interface CalendarEventsState extends
    IntervalsSubState,
    DisableDaysCalendarDaysBeforeSubState,
    SelectionSubState {
        weeks: WeekModel[];
        disableCalendarActionsButtonGroup: boolean;
        intervalsBySingleDaySelection: ExtractedIntervals;
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
        disableCalendarDaysBefore: null,
        disableCalendarActionsButtonGroup: true,
        selection: defaultSelection,
        intervalsBySingleDaySelection: defaultExtractedIntervals
    };
};

const initState = createInitState();

export const calendarEventsReducer: Reducer<CalendarEventsState> = (state = initState, action: CalendarActions) => {
    switch (action.type) {
        case 'LOAD-CALENDAR-EVENTS-FINISHED':
            const intervals = action.calendarEvents.buildIntervalsModel();

            return {
                ...state,
                intervals: intervals.asReadOnly(),
                disableCalendarActionsButtonGroup: false
            };
        case 'CALENDAR-EVENT-CREATED':
            const intervalsWithNewEvent = state.intervals
                ? state.intervals.copyIntervalsModel()
                : new IntervalsModel();

            const calendarEvents = new CalendarEvents([action.calendarEvent]);

            calendarEvents.appendToIntervalsModel(intervalsWithNewEvent);

            return {
                ...state,
                intervals: intervalsWithNewEvent.asReadOnly()
            };
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
        case 'INTERVALS-BY-SINGLE-DAY-SELECTION':
            const intervalsBySingleDay = state.intervals
                ? state.intervals.get(state.selection.single.day.date)
                : null;

            const extractedIntervals = new ExtractedIntervals(intervalsBySingleDay);

            return {
                ...state,
                intervalsBySingleDaySelection: extractedIntervals
            };
        default:
            return state;
    }
};