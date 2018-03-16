// import { CalendarEventsState, calendarEventsReducer } from './calendar-events.reducer';
// import { claimSickLeave, confirmSickLeave, confirmStartDateSickLeave, backToClaimSickLeave } from './sick-leave.action';
// import moment from 'moment';
// import { eventDialogTextDateFormat, ClaimSickLeaveDialogModel } from './event-dialog/event-dialog.model';
// import { cancelDialog, loadCalendarEventsFinished, selectCalendarDay } from './calendar.action';
// import { IntervalsModel, DayModel } from './calendar.model';
// import { CalendarEventsType, CalendarEvents } from './calendar-events.model';
// import { Employee } from '../organization/employee.model';

// describe('claimSickLeaveReducer', () => {
//     let state: CalendarEventsState;

//     beforeEach(() => {
//         const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
//         state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);
//     });

//     beforeEach(() => {
//         const action = claimSickLeave();
//         state = calendarEventsReducer(state, action);
//     });

//     it('should have dialog model', () => {
//         expect(state.dialog.model).toBeInstanceOf(ClaimSickLeaveDialogModel);
//     });

//     it('should have dialog with sick_leave icon', () => {
//         expect(state.dialog.model.icon).toBe('sick_leave');
//     });

//     it('should have dialog with text', () => {
//         expect(state.dialog.model.text).toBe(`Your sick leave starts on ${state.selectedCalendarDay.date.format(eventDialogTextDateFormat)}`);
//     });

//     it('should have dialog with cancel which is cancelDialog action', () => {
//         expect(state.dialog.model.cancelLabel).toBe('Back');
//         expect(state.dialog.model.cancelAction()).toEqual(cancelDialog());
//     });

//     it('should have dialog with confirm which is confirmStartDateSickLeave action', () => {
//         expect(state.dialog.model.acceptLabel).toBe('Confirm');
//         expect(state.dialog.model.acceptAction({ userEmployee: new Employee() })).toEqual(confirmStartDateSickLeave(state.selectedCalendarDay.date));
//     });
// });

// describe('select sickleave startDate', () => {
//     let state: CalendarEventsState;
//     let startDay: DayModel;

//     beforeEach(() => {
//         const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
//         state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);
//     });

//     beforeEach(() => {
//         const action = claimSickLeave();
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         startDay = state.selectedCalendarDay;
//     });

//     beforeEach(() => {
//         const action = selectCalendarDay(startDay);
//         state = calendarEventsReducer(state, action);
//     });

//     it('should have dialog with text', () => {
//         expect(state.dialog.model.text)
//             .toBe(`Your sick leave starts on ${state.selectedCalendarDay.date.format(eventDialogTextDateFormat)}`);
//     });

//     it('should have dialog with cancel which is cancelDialog action', () => {
//         expect(state.dialog.model.cancelLabel).toBe('Back');
//         expect(state.dialog.model.cancelAction()).toEqual(cancelDialog());
//     });

//     it('should have dialog with confirm which is confirmStartDateSickLeave action', () => {
//         expect(state.dialog.model.acceptLabel).toBe('Confirm');
//         expect(state.dialog.model.acceptAction({ userEmployee: undefined })).toEqual(confirmStartDateSickLeave(state.selectedCalendarDay.date));
//     });
// });

// describe('confirm sickleave startdate', () => {
//     let state: CalendarEventsState;
//     let intervalsAfterClaim: IntervalsModel;
//     let intervalsBeforeClaim: IntervalsModel;
//     let startDay: DayModel;

//     beforeEach(() => {
//         const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
//         state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);

//         intervalsBeforeClaim = state.intervals;
//     });

//     beforeEach(() => {
//         const action = claimSickLeave();
//         state = calendarEventsReducer(state, action);

//         intervalsAfterClaim = state.intervals;
//     });

//     beforeEach(() => {
//         startDay = state.selectedCalendarDay;
//     });

//     beforeEach(() => {
//         const action = selectCalendarDay(startDay);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         const action = confirmStartDateSickLeave(startDay.date);
//         state = calendarEventsReducer(state, action);
//     });

//     it('should have unchanged intervals', () => {
//         expect(state.editingOfIntervals.unchangedIntervals).toBe(intervalsBeforeClaim);
//     });

//     it('should have intervals as copy', () => {
//         expect(state.intervals).not.toBe(intervalsBeforeClaim);
//     });

//     it('should disable calendar days before start day', () => {
//         expect(state.disableCalendarDaysBefore.date.isSame(startDay.date, 'day')).toBeTruthy();
//     });

//     it('should have dialog with cancel which is backToclaimSickLeave', () => {
//         expect(state.dialog.model.cancelLabel).toBe('Back');
//         expect(state.dialog.model.cancelAction()).toEqual(backToClaimSickLeave(startDay.date));
//     });

//     it('should have dialog with confirm which is confirmStartDateSickLeave action', () => {
//         expect(state.dialog.model.acceptLabel).toBe('Confirm');
//         expect(state.dialog.model.acceptAction({ userEmployee: undefined })).toEqual(confirmSickLeave(undefined, undefined));
//     });
// });

// describe('back to claimSickleave', () => {
//     let state: CalendarEventsState;
//     let intervalsBeforeClaim: IntervalsModel;
//     let startDay: DayModel;

//     beforeEach(() => {
//         const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
//         state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);

//         intervalsBeforeClaim = state.intervals;
//     });

//     beforeEach(() => {
//         const action = claimSickLeave();
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         startDay = state.selectedCalendarDay;
//     });

//     beforeEach(() => {
//         const action = selectCalendarDay(startDay);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         const action = confirmStartDateSickLeave(startDay.date);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         const action = backToClaimSickLeave(startDay.date);
//         state = calendarEventsReducer(state, action);
//     });

//     it('should have dialog with text', () => {
//         expect(state.dialog.model.text)
//             .toBe(`Your sick leave starts on ${state.selectedCalendarDay.date.format(eventDialogTextDateFormat)}`);
//     });

//     it('should enable calendar days before start day', () => {
//         expect(state.disableCalendarDaysBefore).toBeNull();
//     });

//     it('should restore intervals which was before claim', () => {
//         expect(state.intervals).toBe(intervalsBeforeClaim);
//     });

//     it('should have dialog with cancel which is cancelDialog action', () => {
//         expect(state.dialog.model.cancelLabel).toBe('Back');
//         expect(state.dialog.model.cancelAction()).toEqual(cancelDialog());
//     });

//     it('should have dialog with confirm which is confirmStartDateSickLeave action', () => {
//         expect(state.dialog.model.acceptLabel).toBe('Confirm');
//         expect(state.dialog.model.acceptAction({ userEmployee: undefined })).toEqual(confirmStartDateSickLeave(state.selectedCalendarDay.date));
//     });
// });

// describe('select sickleave enddate', () => {
//     let state: CalendarEventsState;
//     let startDay: DayModel;
//     let endDay: DayModel;

//     beforeEach(() => {
//         const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
//         state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);
//     });

//     beforeEach(() => {
//         const action = claimSickLeave();
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         startDay = state.selectedCalendarDay;
//     });

//     beforeEach(() => {
//         const action = selectCalendarDay(startDay);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         const action = confirmStartDateSickLeave(startDay.date);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         endDay = state.selectedCalendarDay;
//     });

//     beforeEach(() => {
//         const action = selectCalendarDay(endDay);
//         state = calendarEventsReducer(state, action);
//     });

//     it ('should add draft interval', () => {

//         const intervals = state.intervals.get(endDay.date);

//         expect(intervals[0].draft).toBeTruthy();
//         expect(intervals[0].eventType).toBe(CalendarEventsType.Sickleave);
//         expect(intervals[0].startDate.isSame(startDay.date, 'day')).toBeTruthy();
//         expect(intervals[0].endDate.isSame(endDay.date, 'day')).toBeTruthy();
//     });

//     it('should have dialog with text', () => {
//         expect(state.dialog.model.text)
//             .toBe(`Your sick leave has started on ${startDay.date.format(eventDialogTextDateFormat)} and will be complete on ${startDay.date.format(eventDialogTextDateFormat)}`);
//     });
// });

// describe('confirm sickleave', () => {
//     let state: CalendarEventsState;
//     let startDay: DayModel;
//     let endDay: DayModel;

//     beforeEach(() => {
//         const loadCalendarEventsFinishedAction = loadCalendarEventsFinished([]);
//         state = calendarEventsReducer(undefined, loadCalendarEventsFinishedAction);
//     });

//     beforeEach(() => {
//         const action = claimSickLeave();
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         startDay = state.selectedCalendarDay;
//     });

//     beforeEach(() => {
//         const action = selectCalendarDay(startDay);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         const action = confirmStartDateSickLeave(startDay.date);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         endDay = state.selectedCalendarDay;
//     });

//     beforeEach(() => {
//         const action = selectCalendarDay(endDay);
//         state = calendarEventsReducer(state, action);
//     });

//     beforeEach(() => {
//         const action = confirmSickLeave(new Employee(), new CalendarEvents());
//         state = calendarEventsReducer(state, action);
//     });

//     it('should reset dialog model to null', () => {
//         expect(state.dialog.model).toBeNull();
//     });

//     it('should reset editing of intervals unchanged intervals to null', () => {
//         expect(state.editingOfIntervals.unchangedIntervals).toBeNull();
//     });

//     it('should enable calendar days before start day', () => {
//         expect(state.disableCalendarDaysBefore).toBeNull();
//     });
// });