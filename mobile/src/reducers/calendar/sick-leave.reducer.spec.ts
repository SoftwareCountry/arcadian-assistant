import { CalendarEventsState, calendarEventsReducer, EditIntervalsState } from './calendar-events.reducer';
import { claimSickLeaveReducer } from './sick-leave.reducer';
import { claimSickLeave, confirmSickLeave } from './sick-leave.action';
import moment from 'moment';
import { eventDialogTextDateFormat, ClaimSickLeaveDialogModel } from './event-dialog/event-dialog.model';
import { cancelDialog, LoadCalendarEventsFinished, loadCalendarEventsFinished } from './calendar.action';
import { IntervalsModel } from './calendar.model';

describe('claimSickLeaveReducer', () => {
    let state: CalendarEventsState;
    let dialog: ClaimSickLeaveDialogModel;
    let intervalsAfterClaim: IntervalsModel;
    let intervalsBeforeClaim: IntervalsModel;
    let editIntervals: EditIntervalsState;
    const startDate = moment();

    beforeEach(() => {
        const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
        state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);

        intervalsBeforeClaim = state.intervals;

        const action = claimSickLeave(startDate);
        state = calendarEventsReducer(state, action);

        intervalsAfterClaim = state.intervals;
        dialog = state.dialog.model as ClaimSickLeaveDialogModel;
        editIntervals = state.editIntervals;
    });

    it('should active dialog', () => {
        expect(state.dialog.active).toBeTruthy();
    });

    it('should has sick_leave icon', () => {
        expect(dialog.icon).toBe('sick_leave');
    });

    it('should has text', () => {
        expect(dialog.text).toBe(`Your sick leave has started on ${startDate.format(eventDialogTextDateFormat)}`);
    });

    it('should has back is cancel dialog action', () => {
        expect(dialog.cancel.label).toBe('Back');
        expect(dialog.cancel.action).toBe(cancelDialog);
    });

    it('should has confirm is cancel dialog action', () => {
        expect(dialog.accept.label).toBe('Confirm');
        expect(dialog.accept.action).toBe(confirmSickLeave);
    });

    it('should has startDate = selected day', () => {
        expect(dialog.startDate.isSame(state.selectedCalendarDay.date, 'day')).toBeTruthy();
    });

    it('should has endDate is undefined', () => {
        expect(dialog.endDate).toBeUndefined();
    });

    it('should active edit intervals', () => {
        expect(editIntervals.active).toBeTruthy();
    });

    it('should has edit startDay is selected day', () => {
        expect(editIntervals.startDay).toBe(state.selectedCalendarDay);
    });

    it('should has unchanged intervals', () => {
        expect(editIntervals.unchangedIntervals).toBe(intervalsBeforeClaim);
    });

    it('should has intervals as copy', () => {
        expect(state.intervals).not.toBe(intervalsBeforeClaim);
    });
});