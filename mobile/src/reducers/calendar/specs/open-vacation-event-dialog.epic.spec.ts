import 'rxjs';
import { ActionsObservable } from 'redux-observable';
import { openEventDialog, OpenEventDialog, EventDialogActions } from '../event-dialog/event-dialog.action';
import { EventDialogType } from '../event-dialog/event-dialog-type.model';
import { CalendarSelectionModeType, calendarSelectionMode, disableCalendarSelection, disableSelectIntervalsBySingleDaySelection } from '../calendar.action';
import { openEventDialogEpic$ } from '../event-dialog/event-dialog.epics';
import { CalendarEventsColor } from '../../../calendar/styles';

describe('openEventDialogEpic', () => {
    describe('when request vacation', () => {
        it('should select single selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.RequestVacation));

            openEventDialogEpic$(action$).subscribe(x => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                done();
            });
        });
    });

    describe('when confirm start date vacation', () => {
        it('should select interval selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ConfirmStartDateVacation));

            openEventDialogEpic$(action$).subscribe(x => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.vacation));
                done();
            });
        });
    });

    describe('when edit vacation', () => {
        let action$: ActionsObservable<EventDialogActions>;

        beforeEach(() => {
            action$ = ActionsObservable.of(openEventDialog(EventDialogType.EditVacation));
        });

        it('should select single selection mode', (done) => {
            openEventDialogEpic$(action$)
                .filter(x => x.type === 'CALENDAR-SELECTION-MODE')
                .subscribe(x => {
                    expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                    done();
                });
        });

        it('should disable calendar selection', (done) => {
            openEventDialogEpic$(action$)
                .filter(x => x.type === 'DISABLE-CALENDAR-SELECTION')
                .subscribe(x => {
                    expect(x).toEqual(disableCalendarSelection(true));
                    done();
                });
        });
    });

    describe('when change vacation start date', () => {
        let action$: ActionsObservable<EventDialogActions>;

        beforeEach(() => {
            action$ = ActionsObservable.of(openEventDialog(EventDialogType.ChangeVacationStartDate));
        });

        it('should select single selection mode', (done) => {
            openEventDialogEpic$(action$)
                .filter(x => x.type === 'CALENDAR-SELECTION-MODE')
                .subscribe(x => {
                    expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                    done();
                });
        });

        it('should disable select intervals by single day selection', (done) => {
            openEventDialogEpic$(action$)
                .filter(x => x.type === 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION')
                .subscribe(x => {
                    expect(x).toEqual(disableSelectIntervalsBySingleDaySelection(true));
                    done();
                });
        });
    });

    describe('when change vacation end date', () => {
        it('should select interval selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ChangeVacationEndDate));

            openEventDialogEpic$(action$).subscribe(x => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.vacation));
                done();
            });
        });
    });
});