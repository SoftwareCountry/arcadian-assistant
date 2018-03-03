import { CalendarEventsState, calendarEventsReducer, EditingOfIntervalsState } from './calendar-events.reducer';
import { claimSickLeaveReducer } from './sick-leave.reducer';
import { claimSickLeave, confirmSickLeave, ConfirmClaimSickLeave } from './sick-leave.action';
import moment from 'moment';
import { eventDialogTextDateFormat, ClaimSickLeaveDialogModel } from './event-dialog/event-dialog.model';
import { cancelDialog, LoadCalendarEventsFinished, loadCalendarEventsFinished, selectCalendarDay } from './calendar.action';
import { IntervalsModel, DayModel } from './calendar.model';
import { CalendarEventsType } from './calendar-events.model';

describe('claimSickLeaveReducer', () => {
    let state: CalendarEventsState;
    let intervalsAfterClaim: IntervalsModel;
    let intervalsBeforeClaim: IntervalsModel;
    let editingOfIntervals: EditingOfIntervalsState;
    const startDate = moment();

    beforeEach(() => {
        const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
        state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);

        intervalsBeforeClaim = state.intervals;
    });

    beforeEach(() => {
        const action = claimSickLeave(startDate);
        state = calendarEventsReducer(state, action);

        intervalsAfterClaim = state.intervals;
        editingOfIntervals = state.editingOfIntervals;
    });

    it('should activate dialog', () => {
        expect(state.dialog.active).toBeTruthy();
    });

    it('should has sick_leave icon', () => {
        expect(state.dialog.model.icon).toBe('sick_leave');
    });

    it('should has text', () => {
        expect(state.dialog.model.text).toBe(`Your sick leave has started on ${startDate.format(eventDialogTextDateFormat)}`);
    });

    it('should has back is cancel dialog action', () => {
        expect(state.dialog.model.cancel.label).toBe('Back');
        expect(state.dialog.model.cancel.action).toBe(cancelDialog);
    });

    it('should has confirm is cancel dialog action', () => {
        expect(state.dialog.model.accept.label).toBe('Confirm');
        expect(state.dialog.model.accept.action).toBe(confirmSickLeave);
    });

    it('should activate editing of intervals', () => {
        expect(editingOfIntervals.active).toBeTruthy();
    });

    it('should has edit startDay is selected day', () => {
        expect(editingOfIntervals.startDay).toBe(state.selectedCalendarDay);
    });

    it('should has unchanged intervals', () => {
        expect(editingOfIntervals.unchangedIntervals).toBe(intervalsBeforeClaim);
    });

    it('should has intervals as copy', () => {
        expect(state.intervals).not.toBe(intervalsBeforeClaim);
    });

    it('should disable calendar days before start day', () => {
        expect(state.disableCalendarDaysBefore.date.isSame(startDate, 'day')).toBeTruthy();
    });
});

describe('select sick leave endDate', () => {
    let state: CalendarEventsState;
    let intervalsAfterClaim: IntervalsModel;
    let intervalsBeforeClaim: IntervalsModel;
    const startDate = moment();
    let endDay: DayModel;

    beforeEach(() => {
        const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
        state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);

        intervalsBeforeClaim = state.intervals;
    });

    beforeEach(() => {
        const action = claimSickLeave(startDate);
        state = calendarEventsReducer(state, action);

        intervalsAfterClaim = state.intervals;
    });

    beforeEach(() => {
        endDay = state.selectedCalendarDay;
    });

    beforeEach(() => {
        const action = selectCalendarDay(endDay);
        state = calendarEventsReducer(state, action);
    });

    it ('should add draft interval', () => {

        const intervals = state.intervals.get(endDay.date);

        expect(intervals[0].draft).toBeTruthy();
        expect(intervals[0].eventType).toBe(CalendarEventsType.SickLeave);
        expect(intervals[0].startDate.isSame(startDate, 'day')).toBeTruthy();
        expect(intervals[0].endDate.isSame(endDay.date, 'day')).toBeTruthy();
    });

    it('should has dialog with text', () => {
        expect(state.dialog.model.text)
            .toBe(`Your sick leave has started on ${startDate.format(eventDialogTextDateFormat)} and will be complete on ${endDay.date.format(eventDialogTextDateFormat)}`);
    });

    it('should has editing of intervals with end day', () => {
        expect(state.editingOfIntervals.endDay).toBe(endDay);
    });
});

describe('confirm claim sick leave', () => {
    let state: CalendarEventsState;
    let intervalsAfterClaim: IntervalsModel;
    let intervalsBeforeClaim: IntervalsModel;
    const startDate = moment();
    let endDay: DayModel;

    beforeEach(() => {
        const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
        state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);

        intervalsBeforeClaim = state.intervals;
    });

    beforeEach(() => {
        const action = claimSickLeave(startDate);
        state = calendarEventsReducer(state, action);

        intervalsAfterClaim = state.intervals;
    });

    beforeEach(() => {
        endDay = state.selectedCalendarDay;
    });

    beforeEach(() => {
        const action = selectCalendarDay(endDay);
        state = calendarEventsReducer(state, action);
    });

    beforeEach(() => {
        const action = confirmSickLeave();
        state = calendarEventsReducer(state, action);
    });

    it('should add interval as non-draft', () => {
        const intervals = state.intervals.get(endDay.date);

        expect(intervals[0].draft).toBeFalsy();
        expect(intervals[0].eventType).toBe(CalendarEventsType.SickLeave);
        expect(intervals[0].startDate.isSame(startDate, 'day')).toBeTruthy();
        expect(intervals[0].endDate.isSame(endDay.date, 'day')).toBeTruthy();
    });

    it('should add interval as non-draft', () => {
        const intervals = state.intervals.get(endDay.date);

        expect(intervals[0].draft).toBeFalsy();
        expect(intervals[0].eventType).toBe(CalendarEventsType.SickLeave);
        expect(intervals[0].startDate.isSame(startDate, 'day')).toBeTruthy();
        expect(intervals[0].endDate.isSame(endDay.date, 'day')).toBeTruthy();
    });

    it('should disactivate dialog', () => {
        expect(state.dialog.active).toBeFalsy();
    });

    it('should reset dialog model to null', () => {
        expect(state.dialog.model).toBeNull();
    });

    it('should disactivate edititing of intervals', () => {
        expect(state.editingOfIntervals.active).toBeFalsy();
    });

    it('should reset editing of intervals endDay to null', () => {
        expect(state.editingOfIntervals.endDay).toBeNull();
    });

    it('should reset editing of intervals startDay to null', () => {
        expect(state.editingOfIntervals.startDay).toBeNull();
    });

    it('should reset edit unchanged intervals to null', () => {
        expect(state.editingOfIntervals.unchangedIntervals).toBeNull();
    });

    it('should enable calendar days before start day', () => {
        expect(state.disableCalendarDaysBefore).toBeNull();
    });
});