import 'rxjs';
import { CalendarSelectionModeType, calendarSelectionMode, disableCalendarSelection } from '../calendar.action';
import { ActionsObservable } from 'redux-observable';
import { calendarSelectionModeEpic$ } from '../calendar.epics';


describe('calendarSelectionModeEpic', () => {
    it('should enable calendar selection', (done) => {
        const mode = CalendarSelectionModeType.SingleDay;
        const action$ = ActionsObservable.of(calendarSelectionMode(mode, '#abc'));

        calendarSelectionModeEpic$(action$).subscribe(x => {
            expect(x).toEqual(disableCalendarSelection(false, mode));
            done();
        });
    });
});