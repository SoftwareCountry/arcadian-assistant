import 'rxjs';
import { ActionsObservable, ofType } from 'redux-observable';
import { openEventDialog, OpenEventDialog } from '../event-dialog/event-dialog.action';
import { EventDialogType } from '../event-dialog/event-dialog-type.model';
import { CalendarSelectionModeType, calendarSelectionMode, disableCalendarSelection } from '../calendar.action';
import { openEventDialogEpic$ } from '../event-dialog/event-dialog.epics';
import { CalendarEventsColor } from '../../../calendar/styles';

describe('openEventDialogEpic', () => {
    describe('when claim sickleave', () => {
        it('should select single day selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ClaimSickLeave));

            openEventDialogEpic$(action$).subscribe(x => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                done();
            });
        });
    });

    describe('when confirm start date sickleave', () => {
        it('should select interval selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ConfirmStartDateSickLeave));

            openEventDialogEpic$(action$).subscribe(x => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave));
                done();
            });
        });
    });

    describe('when edit sickleave', () => {
        let action$: ActionsObservable<OpenEventDialog>;

        beforeEach(() => {
            action$ = ActionsObservable.of(openEventDialog(EventDialogType.EditSickLeave));
        });

        it('should select single selection mode', (done) => {
            openEventDialogEpic$(action$)
                .filter(x => x.type === 'CALENDAR-SELECTION-MODE')
                .subscribe(x => {
                    expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                    done();
                });
        });

        it('should disable selection mode', (done) => {
            openEventDialogEpic$(action$)
                .filter(x => x.type === 'DISABLE-CALENDAR-SELECTION')
                .subscribe(x => {
                    expect(x).toEqual(disableCalendarSelection(true));
                    done();
                });
        });
    });

    describe('when prolong sickleave', () => {
        it('should select interval selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.ProlongSickLeave));

            openEventDialogEpic$(action$).subscribe(x => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.Interval, CalendarEventsColor.sickLeave));
                done();
            });
        });
    });
});