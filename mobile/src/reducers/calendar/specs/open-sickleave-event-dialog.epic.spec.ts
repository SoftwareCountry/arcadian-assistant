import 'rxjs';
import { ActionsObservable } from 'redux-observable';
import { openEventDialog, OpenEventDialog } from '../event-dialog/event-dialog.action';
import { EventDialogType } from '../event-dialog/event-dialog-type.model';
import { calendarSelectionMode, CalendarSelectionModeType, disableCalendarSelection } from '../calendar.action';
import { openEventDialogEpic$ } from '../event-dialog/event-dialog.epics';
import { CalendarEventsColor } from '../../../calendar/styles';
import { Action } from 'redux';
import { filter } from 'rxjs/operators';

describe('openEventDialogEpic', () => {
    describe('when claim sick leave', () => {
        it('should select single day selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ClaimSickLeave));

            openEventDialogEpic$(action$).subscribe((x: Action) => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                done();
            });
        });
    });

    describe('when confirm start date sick leave', () => {
        it('should select interval selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ConfirmStartDateSickLeave));

            openEventDialogEpic$(action$).subscribe((x: Action) => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave));
                done();
            });
        });
    });

    describe('when edit sick leave', () => {
        let action$: ActionsObservable<OpenEventDialog>;

        beforeEach(() => {
            action$ = ActionsObservable.of(openEventDialog(EventDialogType.EditSickLeave));
        });

        it('should select single selection mode', (done) => {
            openEventDialogEpic$(action$).pipe(
                filter((x: Action) => x.type === 'CALENDAR-SELECTION-MODE'))
                .subscribe((x: Action) => {
                    expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                    done();
                });
        });

        it('should disable selection mode', (done) => {
            openEventDialogEpic$(action$).pipe(
                filter((x: Action) => x.type === 'DISABLE-CALENDAR-SELECTION'))
                .subscribe((x: Action) => {
                    expect(x).toEqual(disableCalendarSelection(true));
                    done();
                });
        });
    });

    describe('when prolong sick leave', () => {
        it('should select interval selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ProlongSickLeave));

            openEventDialogEpic$(action$).subscribe((x: Action) => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave));
                done();
            });
        });
    });
});
