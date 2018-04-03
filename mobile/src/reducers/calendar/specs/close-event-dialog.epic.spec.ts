import 'rxjs';
import { CalendarSelectionModeType, calendarSelectionMode, disableSelectIntervalsBySingleDaySelection, selectIntervalsBySingleDaySelection } from '../calendar.action';
import { closeEventDialog, EventDialogActions } from '../event-dialog/event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import { closeEventDialogEpic$ } from '../event-dialog/event-dialog.epics';

describe('closeEventDialogEpic', () => {
    let mode: CalendarSelectionModeType;
    let action$: ActionsObservable<EventDialogActions>;

    beforeEach(() => {
        mode = CalendarSelectionModeType.SingleDay;
        action$ = ActionsObservable.of(closeEventDialog());
    });

    it('should select single day selection', (done) => {
        closeEventDialogEpic$(action$)
            .filter(x => x.type === 'CALENDAR-SELECTION-MODE')
            .subscribe(x => {
                expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
                done();
            });
    });

    it('should enable select intervals by single day selection', (done) => {
        closeEventDialogEpic$(action$)
            .filter(x => x.type === 'DISABLE-SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION')
            .subscribe(x => {
                expect(x).toEqual(disableSelectIntervalsBySingleDaySelection(false));
                done();
            });
    });

    it('should select intervals by single day selection', (done) => {
        closeEventDialogEpic$(action$)
            .filter(x => x.type === 'SELECT-INTERVALS-BY-SINGLE-DAY-SELECTION')
            .subscribe(x => {
                expect(x).toEqual(selectIntervalsBySingleDaySelection());
                done();
            });
    });
});