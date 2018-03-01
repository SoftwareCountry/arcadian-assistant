import { UserActions, LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { apiUrl } from '../const';
import { deserializeArray } from 'santee-dcts';
import { loadCalendarEventsFinished } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents, CalendarEventsType, CalendarEventStatus, DatesInterval } from './calendar-events.model';
import moment from 'moment';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        // .switchMap(x =>
        //     ajaxGetJSON(`${apiUrl}/employees/${x.employee.employeeId}/events`).map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        // )
        // .map(x => loadCalendarEventsFinished(x))
        // TODO: Next PR: Take events from API and add Agenda to save intervals. Now it's just a mock to show how my calendar works.
        .map(() => {
            const mockEvent = new CalendarEvents();
            const mockInterval = new DatesInterval();
            const startDate = moment();

            startDate.add(-4, 'days');

            const endDate = moment(startDate);

            endDate.add(14, 'days');

            mockInterval.startDate = startDate;
            mockInterval.endDate = endDate;
            mockInterval.startWorkingHour = 0;
            mockInterval.finishWorkingHour = 8;

            mockEvent.calendarEventId = '1';
            mockEvent.type = CalendarEventsType.Vacation;
            mockEvent.dates = mockInterval;
            mockEvent.status = CalendarEventStatus.Approved;

            // --

            const mockEvent2 = new CalendarEvents();
            const mockInterval2 = new DatesInterval();
            const startDate2 = moment();

            startDate2.add(-8, 'days');

            const endDate2 = moment(startDate2);

            endDate2.add(14, 'days');

            mockInterval2.startDate = startDate2;
            mockInterval2.endDate = endDate2;
            mockInterval2.startWorkingHour = 0;
            mockInterval2.finishWorkingHour = 3;

            mockEvent2.calendarEventId = '2';
            mockEvent2.type = CalendarEventsType.SickLeave;
            mockEvent2.dates = mockInterval2;
            mockEvent2.status = CalendarEventStatus.Approved;

            // --

            const mockEvent3 = new CalendarEvents();
            const mockInterval3 = new DatesInterval();
            const startDate3 = moment();

            startDate3.add(-9, 'days');

            mockInterval3.startDate = startDate3;
            mockInterval3.endDate = startDate3;
            mockInterval3.startWorkingHour = 0;
            mockInterval3.finishWorkingHour = 4;

            mockEvent3.calendarEventId = '2';
            mockEvent3.type = CalendarEventsType.Dayoff;
            mockEvent3.dates = mockInterval3;
            mockEvent3.status = CalendarEventStatus.Approved;

            return loadCalendarEventsFinished([mockEvent, mockEvent2, mockEvent3]);
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

