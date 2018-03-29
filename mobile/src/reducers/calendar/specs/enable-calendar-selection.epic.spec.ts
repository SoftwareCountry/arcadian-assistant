import 'rxjs';
import { CalendarSelectionModeType, disableCalendarSelection, disableSelectIntervalsBySingleDaySelection } from '../calendar.action';
import { ActionsObservable } from 'redux-observable';
import { enableCalendarSelectionEpic$ } from '../calendar.epics';

describe('enableCalendarSelectionEpic', () => {
    describe('when selection mode is interval', () => {
        it('should disable select intervals by single day selection', (done) => {
            const mode = CalendarSelectionModeType.SingleDay;
            const action$ = ActionsObservable.of(disableCalendarSelection(false, CalendarSelectionModeType.Interval));

            enableCalendarSelectionEpic$(action$).subscribe(x => {
                expect(x).toEqual(disableSelectIntervalsBySingleDaySelection(true));
                done();
            });
        });
    });

    describe('when selection mode is single', () => {
        it('should enable select intervals by single day selection', (done) => {
            const mode = CalendarSelectionModeType.SingleDay;
            const action$ = ActionsObservable.of(disableCalendarSelection(false, CalendarSelectionModeType.SingleDay));
    
            enableCalendarSelectionEpic$(action$).subscribe(x => {
                expect(x).toEqual(disableSelectIntervalsBySingleDaySelection(false));
                done();
            });
        });
    });
});