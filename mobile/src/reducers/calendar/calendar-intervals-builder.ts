import { CalendarEvents, CalendarEventsType } from './calendar-events.model';
import { IntervalsModel, IntervalType } from './calendar.model';
import moment from 'moment';

export class CalendarIntervalsBuilder {

    public buildIntervals(calendarEvents: CalendarEvents[]) {
        const intervalsModel = new IntervalsModel();

        this.insertCalendarEvents(intervalsModel, calendarEvents);

        return intervalsModel;
    }

    public appendCalendarEvents(existingModel: IntervalsModel, calendarEvents: CalendarEvents[]) {
        this.insertCalendarEvents(existingModel, calendarEvents);
    }

    private insertCalendarEvents(intervalsModel: IntervalsModel, calendarEvents: CalendarEvents[]) {

        for (let calendarEvent of calendarEvents) {
            const start = moment(calendarEvent.dates.startDate);

            if (calendarEvent.type === CalendarEventsType.Dayoff || calendarEvent.type === CalendarEventsType.Workout) {
                this.buildIntervalBoundary(start, intervalsModel, calendarEvent);
                continue;
            }

            if (start.isSame(calendarEvent.dates.endDate, 'day')) {
                intervalsModel.set(start, {
                    intervalType: IntervalType.IntervalFullBoundary,
                    calendarEvent: calendarEvent,
                    boundary: true
                });
                continue;
            }

            while (start.isSameOrBefore(calendarEvent.dates.endDate, 'day')) {
                let intervalType: IntervalType = IntervalType.Interval;

                if (start.isSame(calendarEvent.dates.startDate)) {
                    intervalType = IntervalType.StartInterval;
                } else if (start.isSame(calendarEvent.dates.endDate)) {
                    intervalType = IntervalType.EndInterval;
                }

                intervalsModel.set(start, {
                    intervalType: intervalType,
                    calendarEvent: calendarEvent,
                    boundary: false
                });

                start.add(1, 'days');
            }
        }

        return intervalsModel;
    }

    private buildIntervalBoundary(keyDate: moment.Moment, intervalsModel: IntervalsModel, calendarEvent: CalendarEvents) {
        const intervalType = this.getBoundaryType(calendarEvent);

        intervalsModel.set(keyDate, {
            intervalType: intervalType,
            calendarEvent: calendarEvent,
            boundary: true
        });
    }

    private getBoundaryType(calendarEvent: CalendarEvents): IntervalType | null {
        const { startWorkingHour, finishWorkingHour } = calendarEvent.dates;

        if (0 <= startWorkingHour && finishWorkingHour <= 4) {
            return IntervalType.IntervalLeftBoundary;
        }

        if (4 <= startWorkingHour && finishWorkingHour <= 8) {
            return IntervalType.IntervalRightBoundary;
        }

        if (0 <= startWorkingHour && finishWorkingHour <= 8) {
            return IntervalType.IntervalFullBoundary;
        }

        return null;
    }
}