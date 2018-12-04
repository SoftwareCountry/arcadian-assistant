import 'rxjs';
import { CalendarSelectionModeType, calendarSelectionMode, disableSelectIntervalsBySingleDaySelection, selectIntervalsBySingleDaySelection } from '../calendar.action';
import { closeEventDialog, EventDialogActions, stopEventDialogProgress } from '../event-dialog/event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import { closeEventDialogEpic$ } from '../event-dialog/event-dialog.epics';
import { Action } from 'redux';
import { filter } from 'rxjs/operators';

describe('closeEventDialogEpic', () => {
    let mode: CalendarSelectionModeType;
    let action$: ActionsObservable<EventDialogActions>;

    beforeEach(() => {
        mode = CalendarSelectionModeType.SingleDay;
        action$ = ActionsObservable.of(closeEventDialog());
    });

    it('should select single day selection', (done) => {
        closeEventDialogEpic$(action$).pipe(
            filter((x: Action) => x.type === 'CALENDAR-SELECTION-MODE'))
            .subscribe((x: Action) => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                done();
            });
    });

    it('should enable select intervals by single day selection', (done) => {
        closeEventDialogEpic$(action$).pipe(
            filter((x: Action) => x.type === 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION'))
            .subscribe((x: Action) => {
                expect(x).toEqual(disableSelectIntervalsBySingleDaySelection(false));
                done();
            });
    });

    it('should select intervals by single day selection', (done) => {
        closeEventDialogEpic$(action$).pipe(
            filter((x: Action) => x.type === 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION'))
            .subscribe((x: Action) => {
                expect(x).toEqual(selectIntervalsBySingleDaySelection());
                done();
            });
    });

    it('should stop event dialog progress', (done) => {
        closeEventDialogEpic$(action$).pipe(
            filter((x: Action) => x.type === 'STOP-EVENT-DIALOG-PROGRESS'))
            .subscribe((x: Action) => {
                expect(x).toEqual(stopEventDialogProgress());
                done();
            });
    });
});
