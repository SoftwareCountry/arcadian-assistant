import { Reducer } from 'redux';
import { Map, Set } from 'immutable';
import { CalendarActions } from './calendar.action';
import {
    CalendarPageModel,
    CalendarSelection,
    DayModel,
    ExtractedIntervals,
    ReadOnlyIntervalsModel
} from './calendar.model';
import moment from 'moment';
import { intervalSelectionReducer, singleDaySelectionReducer } from './calendar-selection.reducer';
import { calendarSelectionModeReducer } from './calendar-selection-mode.reducer';
import { CalendarEvent, CalendarEventId } from './calendar-event.model';
import { UserActions } from '../user/user.action';
import { nextCalendarPageReducer } from './next-calendar-page.reducer';
import { prevCalendarPageReducer } from './prev-calendar-page.reducer';
import { createCalendarPagesInitState } from './calendar-pages-init-state';
import { Optional } from 'types';
import { resetCalendarPagesReducer } from './reset-calendar-pages.reducer';
import { Approval } from './approval.model';
import { ApprovalAction, ApprovalActionType } from './approval.action';

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
    approvals: Map<CalendarEventId, Set<Approval>>;
    userEmployeeId: Optional<string>;
}

export interface CalendarPagesSubState {
    pages: CalendarPageModel[];
}

export interface CalendarEventsState extends IntervalsSubState, DisableCalendarDaysBeforeSubState, EventsMapSubState, SelectionSubState, CalendarPagesSubState {
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

    let todayModel: Optional<DayModel> = undefined;
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
        interval: undefined
    };

    const defaultExtractedIntervals = new ExtractedIntervals(undefined);

    return {
        pages: [prevPage, currentPage, nextPage],
        intervals: undefined,
        events: Map(),
        approvals: Map(),
        userEmployeeId: undefined,
        disableCalendarDaysBefore: undefined,
        disableCalendarActionsButtonGroup: true,
        selection: defaultSelection,
        selectedIntervalsBySingleDaySelection: defaultExtractedIntervals,
        disableSelectIntervalsBySingleDaySelection: false,
        disableSelection: false
    };
};

const initState = createInitState();

export const calendarEventsReducer: Reducer<CalendarEventsState> = (state = initState, action: CalendarActions | UserActions | ApprovalAction) => {
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
        case 'RESET-CALENDAR-PAGES':
            const initPageState = resetCalendarPagesReducer(state, action);
            return {
                ...state,
                ...initPageState
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
                : undefined;

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
        case ApprovalActionType.loadApprovalsFinished:
            return {
                ...state,
                approvals: action.approvals.map(approvalsArray => Set(approvalsArray)),
            };
        case ApprovalActionType.approveFinished:
            const eventId = action.approval.eventId;
            const approvals = state.approvals.get(action.approval.eventId, Set());

            return {
                ...state,
                approvals: state.approvals.set(eventId, approvals.add(action.approval)),
            };

        default:
            return state;
    }
};
