import { Reducer } from 'redux';
import { CalendarActions } from './calendar.action';
import { backToClaimSickLeave } from './sick-leave.action';
import { DayModel, WeekModel, IntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { EventDialogModel, ProlongSickLeaveDialogModel, EditSickLeaveDialogModel } from './event-dialog/event-dialog.model';
import { selectEndDateSickLeaveReducer, claimSickLeaveReducer, confirmClaimSickLeaveReducer, selectStartDateSickLeaveReducer, confirmStartDateSickLeaveReducer, backToClaimSickLeaveReducre } from './sick-leave.reducer';

export interface DialogActiveState {
    model: EventDialogModel<any>;
}

export interface EditingOfIntervalsState {
    unchangedIntervals: IntervalsModel;
}

export interface IntervalsSubState {
    intervals: IntervalsModel;
}

export interface EventDialogSubState {
    dialog: DialogActiveState;
}

export interface EditingOfIntervalsSubState {
    editingOfIntervals: EditingOfIntervalsState;
}

export interface DisableDaysCalendarDaysBeforeSubState {
    disableCalendarDaysBefore: DayModel;
}

export interface CalendarEventsState extends 
IntervalsSubState, 
EventDialogSubState, 
EditingOfIntervalsSubState,
DisableDaysCalendarDaysBeforeSubState {
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

    const defaultDialogState: DialogActiveState = {
        model: null
    };

    const defaultEditingOfIntervalsState: EditingOfIntervalsState = {
        unchangedIntervals: null
    };

    return {
        weeks: weeks,
        selectedCalendarDay: todayModel,
        dialog: defaultDialogState,
        intervals: null,
        editingOfIntervals: defaultEditingOfIntervalsState,
        disableCalendarDaysBefore: null,
        disableCalendarActionsButtonGroup: true
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
        case 'SELECT-CALENDAR-DAY':
            const startSickLeaveDialog = selectStartDateSickLeaveReducer(state, action);
            const endSickLeaveEventState = selectEndDateSickLeaveReducer(state, action);

            return {
                ...state,
                ...startSickLeaveDialog,
                ...endSickLeaveEventState,
                selectedCalendarDay: action.day
            };
        case 'SELECT-CALENDAR-MONTH':
            const builder = new CalendarWeeksBuilder();
            const weeks = builder.buildWeeks(action.month, action.year);

            return {
                ...state,
                weeks: weeks
            };
        case 'CLAIM-SICK-LEAVE':
            const claimSickLeaveDialog = claimSickLeaveReducer(state, action);

            return {
                ...state,
                ...claimSickLeaveDialog
            };
        case 'CONFIRM-START-DATE-SICK-LEAVE':
            const confirmStartDateSickLeaveDialog = confirmStartDateSickLeaveReducer(state, action);

            return {
                ...state,
                ...confirmStartDateSickLeaveDialog
            };
        case 'BACK-TO-CLAIM-SICK-LEAVE':
            const backToClaimSickLeaveDialog = backToClaimSickLeaveReducre(state, action);

            return {
                ...state,
                ...backToClaimSickLeaveDialog
            };
        case 'CONFIRM-CLAIM-SICK-LEAVE':
            const confirmClaimSickLeaveState = confirmClaimSickLeaveReducer(state, action);

            return {
                ...state,
                ...confirmClaimSickLeaveState
            };
        case 'PROLONG-SICK-LEAVE':
            const prolongSickLeaveDialog = new ProlongSickLeaveDialogModel();

            return {
                ...state,
                dialog: {
                    ...state.dialog,
                    model: prolongSickLeaveDialog
                }
            };

        case 'EDIT-SICK-LEAVE':
            const editSickLeaveDialog = new EditSickLeaveDialogModel();

            return {
                ...state,
                dialog: {
                    ...state.dialog,
                    active: true,
                    model: editSickLeaveDialog
                }
            };

        case 'CANCEL-CALENDAR-DIALOG':
            const restoredIntervals = state.editingOfIntervals.unchangedIntervals
                ? state.editingOfIntervals.unchangedIntervals
                : state.intervals;

            return {
                ...state,
                intervals: restoredIntervals,
                dialog: {
                    model: null
                },
                editingOfIntervals: {
                    unchangedIntervals: null,
                },
                disableCalendarDaysBefore: null
            };

        default:
            return state;
    }
};