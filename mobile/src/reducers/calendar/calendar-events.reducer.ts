import { Reducer } from 'redux';
import { CalendarActions, cancelDialog } from './calendar.action';
import { editSickLeave, prolongSickLeave, confirmSickLeave, completeSickLeave, confirmProlongSickLeave } from './sick-leave.action';
import { DayModel, WeekModel, IntervalsModel } from './calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { EventDialogModel, ClaimSickLeaveDialogModel, ProlongSickLeaveDialogModel, EditSickLeaveDialogModel } from './event-dialog/event-dialog.model';
import { addNewSickLeaveEventReducer, claimSickLeaveReducer, confirmClaimSickLeaveReducer } from './sick-leave.reducer';

export interface DialogActiveState {
    active: boolean;
    model: EventDialogModel<any>;
}

export interface EditIntervalsState {
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
    editIntervals: EditIntervalsState;
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

    const defaultEditIntervalsState: EditIntervalsState = {
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
        editIntervals: defaultEditIntervalsState
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
                intervals: intervals
            };
        case 'SELECT-CALENDAR-DAY':
            const newSickLeaveEventState = addNewSickLeaveEventReducer(state, action);

            return {
                ...state,
                intervals: newSickLeaveEventState.intervals,
                dialog: newSickLeaveEventState.dialog,
                editIntervals: newSickLeaveEventState.editIntervals,
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
                intervals: claimSickLeaveDialog.intervals,
                dialog: claimSickLeaveDialog.dialog,
                editIntervals: claimSickLeaveDialog.editIntervals
            };
        case 'CONFIRM-CLAIM-SICK-LEAVE':
            const confirmClaimSickLeaveState = confirmClaimSickLeaveReducer(state, action);

            return {
                ...state,
                intervals: confirmClaimSickLeaveState.intervals,
                dialog: confirmClaimSickLeaveState.dialog,
                editIntervals: confirmClaimSickLeaveState.editIntervals
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
            const restoredIntervals = state.editIntervals.unchangedIntervals
                ? state.editIntervals.unchangedIntervals
                : state.intervals;

            return {
                ...state,
                intervals: restoredIntervals,
                dialog: {
                    active: false,
                    model: null
                },
                editIntervals: {
                    active: false,
                    unchangedIntervals: null,
                    startDay: null,
                    endDay: null
                }
            };

        default:
            return state;
    }
};