import 'rxjs';
import { ActionsObservable } from 'redux-observable';
import { CalendarSelectionModeType, disableCalendarSelection, disableSelectIntervalsBySingleDaySelection } from '../calendar.action';
import { disableCalendarSelectionEpic$ } from '../calendar.epics';

describe('disableCalendarSelectionEpic', () => {
    it('should disable select intervals by single day selection', (done) => {
        const mode = CalendarSelectionModeType.SingleDay;
        const action$ = ActionsObservable.of(disableCalendarSelection(true, CalendarSelectionModeType.SingleDay));

        disableCalendarSelectionEpic$(action$).subscribe(x => {
            expect(x).toEqual(disableSelectIntervalsBySingleDaySelection(true));
            done();
        });
    });
});