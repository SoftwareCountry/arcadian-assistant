import 'rxjs';
import { CalendarSelectionModeType, disableSelectIntervalsBySingleDaySelection, selectIntervalsBySingleDaySelection } from '../calendar.action';
import { ActionsObservable } from 'redux-observable';
import { enableSelectIntervalsBySingleDaySelectionEpic$ } from '../calendar.epics';

describe('enableSelectIntervalsBySingleDaySelectionEpic', () => {
    it('should select intervals by single day selection', (done) => {
        const mode = CalendarSelectionModeType.SingleDay;
        const action$ = ActionsObservable.of(disableSelectIntervalsBySingleDaySelection(false));

        enableSelectIntervalsBySingleDaySelectionEpic$(action$).subscribe(x => {
            expect(x).toEqual(selectIntervalsBySingleDaySelection());
            done();
        });
    });
});