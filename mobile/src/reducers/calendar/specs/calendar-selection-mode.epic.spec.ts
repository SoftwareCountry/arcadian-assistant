import 'rxjs';
import { CalendarSelectionModeType, calendarSelectionMode, disableCalendarSelection } from '../calendar.action';
import { ActionsObservable } from 'redux-observable';
import { calendarSelectionModeEpic$ } from '../calendar.epics';
import { Action } from 'redux';


describe('calendarSelectionModeEpic', () => {
    it('should enable calendar selection', (done) => {
        const mode = CalendarSelectionModeType.SingleDay;
        const action$ = ActionsObservable.of(calendarSelectionMode(mode, '#abc'));

        calendarSelectionModeEpic$(action$).subscribe((x: Action) => {
            expect(x).toEqual(disableCalendarSelection(false));
            done();
        });
    });
});
