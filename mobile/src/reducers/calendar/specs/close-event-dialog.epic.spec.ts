import 'rxjs';
import { CalendarSelectionModeType, calendarSelectionMode } from '../calendar.action';
import { closeEventDialog } from '../event-dialog/event-dialog.action';
import { ActionsObservable } from 'redux-observable';
import { closeEventDialogEpic$ } from '../event-dialog/event-dialog.epics';

describe('closeEventDialogEpic', () => {
    it('should select single day selection', (done) => {
        const mode = CalendarSelectionModeType.SingleDay;
        const action$ = ActionsObservable.of(closeEventDialog());

        closeEventDialogEpic$(action$).subscribe(x => {
            expect(x).toEqual(calendarSelectionMode(CalendarSelectionModeType.SingleDay));
            done();
        });
    });
});