import 'rxjs';
import { CalendarEvent, DatesInterval, CalendarEventStatus, CalendarEventType } from '../calendar-event.model';
import moment from 'moment';
import { ActionsObservable } from 'redux-observable';
import { calendarEventCreated } from '../calendar.action';
import { calendarEventCreatedEpic$ } from '../calendar.epics';
import { closeEventDialog } from '../event-dialog/event-dialog.action';


describe('calendarEventCreatedEpic', () => {
    let calendarEvent: CalendarEvent;
    
    beforeEach(() => {
        calendarEvent = new CalendarEvent();

        calendarEvent.calendarEventId = '1';
        calendarEvent.dates = new DatesInterval();
        calendarEvent.dates.startDate = moment();
        calendarEvent.dates.endDate = moment(calendarEvent.dates.startDate);
        calendarEvent.status = CalendarEventStatus.Requested;
        calendarEvent.type = CalendarEventType.Sickleave;
    });

    it('should close event dialog', (done) => {
        const action$ = ActionsObservable.of(calendarEventCreated(calendarEvent));

        calendarEventCreatedEpic$(action$).subscribe(x => {
            expect(x).toEqual(closeEventDialog());
            done();
        });
    });
});