import { UserActions, LoadUserEmployeeFinished } from '../user/user.action';
import { ActionsObservable } from 'redux-observable';
import { Observable } from 'rxjs/Observable';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { apiUrl } from '../const';
import { deserializeArray } from 'santee-dcts';
import { loadCalendarEventsFinished } from './calendar.action';
import { loadFailedError } from '../errors/errors.action';
import { CalendarEvents, CalendarEventsType, CalendarEventStatus, DatesPeriod } from './calendar-events.model';
import moment from 'moment';

export const loadCalendarEventsFinishedEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished>) =>
    action$.ofType('LOAD-USER-EMPLOYEE-FINISHED')
        // .switchMap(x =>
        //     ajaxGetJSON(`${apiUrl}/employees/${x.employee.employeeId}/events`).map((obj: Object[]) => deserializeArray(obj, CalendarEvents))
        // )
        // .map(x => loadCalendarEventsFinished(x))
        // TODO: Next PR: Take events from API and add Agenda to save periods. Now it's just a mock to show how my calendar works.
        .map(() => {
            const mockEvent = new CalendarEvents();
            const mockPeriod = new DatesPeriod();
            const startDate = moment();

            startDate.add(-4, 'days');

            const endDate = moment(startDate);

            endDate.add(14, 'days');

            mockPeriod.startDate = startDate;
            mockPeriod.endDate = endDate;
            mockPeriod.startWorkingHour = 0;
            mockPeriod.finishWorkingHour = 8;

            mockEvent.calendarEventId = '1';
            mockEvent.type = CalendarEventsType.Vacation;
            mockEvent.dates = mockPeriod;
            mockEvent.status = CalendarEventStatus.Approved;

            // --

            const mockEvent2 = new CalendarEvents();
            const mockPeriod2 = new DatesPeriod();
            const startDate2 = moment();

            startDate2.add(-8, 'days');

            const endDate2 = moment(startDate2);

            endDate2.add(14, 'days');

            mockPeriod2.startDate = startDate2;
            mockPeriod2.endDate = endDate2;
            mockPeriod2.startWorkingHour = 0;
            mockPeriod2.finishWorkingHour = 8;

            mockEvent2.calendarEventId = '2';
            mockEvent2.type = CalendarEventsType.SickLeave;
            mockEvent2.dates = mockPeriod2;
            mockEvent2.status = CalendarEventStatus.Approved;

            // --

            const mockEvent3 = new CalendarEvents();
            const mockPeriod3 = new DatesPeriod();
            const startDate3 = moment();

            startDate3.add(-9, 'days');

            mockPeriod3.startDate = startDate3;
            mockPeriod3.endDate = startDate3;
            mockPeriod3.startWorkingHour = 0;
            mockPeriod3.finishWorkingHour = 8;

            mockEvent3.calendarEventId = '2';
            mockEvent3.type = CalendarEventsType.Dayoff;
            mockEvent3.dates = mockPeriod3;
            mockEvent3.status = CalendarEventStatus.Approved;

            return loadCalendarEventsFinished([mockEvent, mockEvent2, mockEvent3]);
        })
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));

