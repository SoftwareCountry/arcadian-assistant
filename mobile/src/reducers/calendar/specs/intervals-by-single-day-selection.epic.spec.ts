import { ActionsObservable } from 'redux-observable';
import moment from 'moment';
import { DayModel } from '../calendar.model';
import { CalendarEvent, DatesInterval, CalendarEventStatus, CalendarEventType } from '../calendar-event.model';
import { CalendarEvents } from '../calendar-events.model';
import { intervalsBySingleDaySelectionEpic$ } from '../calendar.epics';
import { SelectCalendarDay, LoadCalendarEventsFinished, selectCalendarDay, loadCalendarEventsFinished, selectIntervalsBySingleDaySelection } from '../calendar.action';
import { Action } from 'redux';
import { concat, Observable, of } from 'rxjs';

describe('intervalsBySingleDaySelectionEpic', () => {
    let action$: ActionsObservable<SelectCalendarDay | LoadCalendarEventsFinished >;
    beforeEach(() => {
        const day: DayModel = {
            date: moment(),
            today: true,
            belongsToCurrentMonth: true
        };

        const employeeId = '1';
        const loadedCalendarEvent = new CalendarEvent();

        loadedCalendarEvent.calendarEventId = '1';
        loadedCalendarEvent.dates = new DatesInterval();
        loadedCalendarEvent.dates.startDate = moment();
        loadedCalendarEvent.dates.endDate = moment(loadedCalendarEvent.dates.startDate);
        loadedCalendarEvent.status = CalendarEventStatus.Requested;
        loadedCalendarEvent.type = CalendarEventType.Sickleave;

        const calendarEvents = new CalendarEvents([loadedCalendarEvent]);

        const createdCalendarEvent = new CalendarEvent();

        createdCalendarEvent.calendarEventId = '1';
        createdCalendarEvent.dates = new DatesInterval();
        createdCalendarEvent.dates.startDate = moment();
        createdCalendarEvent.dates.endDate = moment(createdCalendarEvent.dates.startDate);
        createdCalendarEvent.status = CalendarEventStatus.Requested;
        createdCalendarEvent.type = CalendarEventType.Sickleave;

        action$ = new ActionsObservable(concat(
            of(selectCalendarDay(day)),
            of(loadCalendarEventsFinished(new CalendarEvents([loadedCalendarEvent]), employeeId))
        ));
    });

    it('should select intervals by single day selection', (done) => {
        intervalsBySingleDaySelectionEpic$(action$).subscribe((x: Action) => {
            expect(x).toEqual(selectIntervalsBySingleDaySelection());
            done();
        });
    });
});
