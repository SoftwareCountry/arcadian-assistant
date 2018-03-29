import 'rxjs';
import { ActionsObservable } from 'redux-observable';
import { openEventDialog } from '../event-dialog/event-dialog.action';
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
        it('should disable single selection mode', (done) => {
            const action$ = ActionsObservable.of(openEventDialog(EventDialogType.EditSickLeave));

            openEventDialogEpic$(action$).subscribe(x => {
                expect(x).toEqual(disableCalendarSelection(true, CalendarSelectionModeType.SingleDay));
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