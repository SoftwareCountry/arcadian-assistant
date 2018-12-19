import { AppState } from '../reducers/app.reducer';
import { Employee } from '../reducers/organization/employee.model';
import { Optional } from 'types';
import { DayModel, IntervalModel, ReadOnlyIntervalsModel } from '../reducers/calendar/calendar.model';
import { CalendarEvent } from '../reducers/calendar/calendar-event.model';

//----------------------------------------------------------------------------
export function getEmployee(state: AppState): Optional<Employee> {
    return (state.organization && state.userInfo && state.userInfo.employeeId) ?
        state.organization.employees.employeesById.get(state.userInfo.employeeId) :
        undefined;
}

//----------------------------------------------------------------------------
function getVacationIntervals(intervals: IntervalModel[], exclude: CalendarEvent[] = []): IntervalModel[] {
    return intervals
        .filter(interval => interval.calendarEvent.isVacation)
        .filter(interval => !exclude.find(event => interval.calendarEvent.calendarEventId === event.calendarEventId));
}

//----------------------------------------------------------------------------
export function isIntersectingAnotherVacation(startDay: DayModel | undefined,
                                              endDay: DayModel | undefined,
                                              intervals: ReadOnlyIntervalsModel,
                                              exclude: CalendarEvent[] = []): boolean {
    if (!startDay) {
        return false;
    }

    if (!endDay) {
        const intervalsForStartDay = intervals.get(startDay.date);
        if (!intervalsForStartDay) {
            return false;
        }
        return getVacationIntervals(intervalsForStartDay, exclude).length > 0;
    }

    let currentDay = startDay.date.clone();
    while (currentDay.isSameOrBefore(endDay.date)) {
        const intervalsForCurrentDay = intervals.get(currentDay);
        if (intervalsForCurrentDay) {
            return getVacationIntervals(intervalsForCurrentDay, exclude).length > 0;
        }
        currentDay = currentDay.add(1, 'days');
    }
    return false;
}
