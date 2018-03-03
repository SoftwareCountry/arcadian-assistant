import { Reducer } from 'redux';
import { CalendarActions, cancelDialog } from './calendar.action';
import { editSickLeave, prolongSickLeave, confirmSickLeave, completeSickLeave, confirmProlongSickLeave } from './sick-leave.action';
import { DayModel, WeekModel, IntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { EventDialogModel, ClaimSickLeaveDialogModel, ProlongSickLeaveDialogModel, EditSickLeaveDialogModel } from './event-dialog/event-dialog.model';
import { selectSickLeaveEndDateReducer, claimSickLeaveReducer, confirmClaimSickLeaveReducer } from './sick-leave.reducer';

export interface DialogActiveState {
    active: boolean;
    model: EventDialogModel<any>;
}

export interface EditingOfIntervalsState {
    active: boolean;
    unchangedIntervals: IntervalsModel;
    startDay: DayModel;
    endDay: DayModel;
}

export interface CalendarEventsState {
    weeks: WeekModel[];
    selectedCalendarDay: DayModel;
    intervals: IntervalsModel;
    dialog: DialogActiveState;
    editingOfIntervals: EditingOfIntervalsState;
    disableCalendarDaysBefore: DayModel;
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
        active: false,
        model: null
    };

    const defaultEditingOfIntervalsState: EditingOfIntervalsState = {
        active: false,
        unchangedIntervals: null,
        startDay: null,
        endDay: null
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
            const newSickLeaveEventState = selectSickLeaveEndDateReducer(state, action);

            return {
                ...state,
                ...newSickLeaveEventState,
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
                    active: false,
                    model: null
                },
                editingOfIntervals: {
                    active: false,
                    unchangedIntervals: null,
                    startDay: null,
                    endDay: null
                },
                disableCalendarDaysBefore: null
            };

        default:
            return state;
    }
};