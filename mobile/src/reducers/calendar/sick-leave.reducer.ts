import { SelectCalendarDay } from './calendar.action';
import { CalendarEventsState, EventDialogSubState, IntervalsSubState, EditingOfIntervalsSubState, DisableDaysCalendarDaysBeforeSubState } from './calendar-events.reducer';
import { CalendarEvents, DatesInterval, CalendarEventsType, CalendarEventStatus } from './calendar-events.model';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';
import { ClaimSickLeaveDialogModel, SelectEndDateSickLeaveDialogModel } from './event-dialog/event-dialog.model';
import { ClaimSickLeave, ConfirmClaimSickLeave, ConfirmStartDateSickLeave, BackToClaimSickLeave } from './sick-leave.action';

export const claimSickLeaveReducer = (state: CalendarEventsState, action: ClaimSickLeave): EventDialogSubState | null => {
    const claimSickLeaveDialog = new ClaimSickLeaveDialogModel();

    claimSickLeaveDialog.startDate = state.selectedCalendarDay.date;

    return {
        dialog: {
            model: claimSickLeaveDialog
        }
    };
};

interface ConfirmStartDateSickLeaveSubState extends IntervalsSubState, EventDialogSubState, EditingOfIntervalsSubState, DisableDaysCalendarDaysBeforeSubState {}

export const confirmStartDateSickLeaveReducer = (state: CalendarEventsState, action: ConfirmStartDateSickLeave): ConfirmStartDateSickLeaveSubState | null => {
    const selectEndDateDialog = new SelectEndDateSickLeaveDialogModel();

    selectEndDateDialog.startDate = action.startDate;

    const editedIntervals = state.intervals
        ? state.intervals.copy()
        : null;

    return {
        intervals: editedIntervals,
        dialog: {
            model: selectEndDateDialog
        },
        editingOfIntervals: {
            unchangedIntervals: state.intervals,
        },
        disableCalendarDaysBefore: state.selectedCalendarDay
    };
};

interface BackToClaimSickLeaveSubState extends IntervalsSubState, EventDialogSubState, EditingOfIntervalsSubState, DisableDaysCalendarDaysBeforeSubState {}

export const backToClaimSickLeaveReducer = (state: CalendarEventsState, action: BackToClaimSickLeave): BackToClaimSickLeaveSubState | null => {
    const claimSickLeaveDialog = new ClaimSickLeaveDialogModel();

    claimSickLeaveDialog.startDate = action.startDate;

    const restoredIntervals = state.editingOfIntervals.unchangedIntervals
        ? state.editingOfIntervals.unchangedIntervals
        : state.intervals;

    return {
        intervals: restoredIntervals,
        dialog: {
            model: claimSickLeaveDialog
        },
        editingOfIntervals: {
            unchangedIntervals: null
        },
        disableCalendarDaysBefore: null
    };
};

export const selectStartDateSickLeaveReducer = (state: CalendarEventsState, action: SelectCalendarDay): EventDialogSubState | null => {
    if (state.dialog.model instanceof ClaimSickLeaveDialogModel) {
        const dialogModel = state.dialog.model.copy();

        dialogModel.startDate = action.day.date;

        return {
            dialog: {
                ...state.dialog,
                model: dialogModel
            }
        };
    }

    return null;
};

interface SelectEndDateSickLeaveSubState extends IntervalsSubState, EventDialogSubState {}

export const selectEndDateSickLeaveReducer = (state: CalendarEventsState, action: SelectCalendarDay): SelectEndDateSickLeaveSubState | null => {

    if (state.dialog.model instanceof SelectEndDateSickLeaveDialogModel) {
        const newCalendarEvents = new CalendarEvents();

        newCalendarEvents.type = CalendarEventsType.Sickleave;
        newCalendarEvents.dates = new DatesInterval();
        newCalendarEvents.dates.startWorkingHour = 0;
        newCalendarEvents.dates.finishWorkingHour = 8;
        newCalendarEvents.dates.startDate = state.dialog.model.startDate;
        newCalendarEvents.dates.endDate = action.day.date;
        newCalendarEvents.status = CalendarEventStatus.Requested;

        const changedIntervals = state.editingOfIntervals.unchangedIntervals
            ? state.editingOfIntervals.unchangedIntervals.copy()
            : null;

        if (changedIntervals) {
            const builder = new CalendarIntervalsBuilder();
            builder.appendCalendarEvents(changedIntervals, [newCalendarEvents], { draft: true });
        }

        const dialogModel = state.dialog.model.copy();
        dialogModel.endDate = newCalendarEvents.dates.endDate;
        dialogModel.calendarEvents = newCalendarEvents;

        return {
            intervals: changedIntervals,
            dialog: {
                model: dialogModel
            }
        };
    }

    return null;
};

interface ConfirmClaimSickLeaveSubState extends IntervalsSubState, EventDialogSubState, EditingOfIntervalsSubState, DisableDaysCalendarDaysBeforeSubState {}

export const confirmClaimSickLeaveReducer = (state: CalendarEventsState, action: ConfirmClaimSickLeave): ConfirmClaimSickLeaveSubState => {
    const restoredIntervals = state.editingOfIntervals.unchangedIntervals
        ? state.editingOfIntervals.unchangedIntervals
        : state.intervals;

    return {
        intervals: restoredIntervals,
        dialog: {
            model: null
        },
        editingOfIntervals: {
            unchangedIntervals: null,
        },
        disableCalendarDaysBefore: null
    };
};