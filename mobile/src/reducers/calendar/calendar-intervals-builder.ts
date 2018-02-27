import { CalendarEvents, CalendarEventsType } from './calendar-events.model';
import { IntervalsModel, IntervalType } from './calendar.model';
import moment from 'moment';

export class CalendarIntervalsBuilder {

    public buildIntervals(calendarEvents: CalendarEvents[]) {
        const intervalsModel = new IntervalsModel();

        for (let calendarEvent of calendarEvents) {
            const start = moment(calendarEvent.dates.startDate);

            if (start.isSame(calendarEvent.dates.endDate, 'day')) {
                intervalsModel.set(start, {
                    intervalType: calendarEvent.type === CalendarEventsType.Dayoff || calendarEvent.type === CalendarEventsType.AdditionalWork
                        ? 'intervalLeftBoundary' 
                        : 'intervalFullBoundary',
                    eventType: calendarEvent.type
                });
                continue;
            }

            while (start.isSameOrBefore(calendarEvent.dates.endDate, 'day')) {
                let intervalType: IntervalType = 'interval';

                if (start.isSame(calendarEvent.dates.startDate)) {
                    intervalType = 'startInterval';
                } else if (start.isSame(calendarEvent.dates.endDate)) {
                    intervalType = 'endInterval';
                }

                intervalsModel.set(start, {
                    intervalType: intervalType,
                    eventType: calendarEvent.type
                });

                start.add(1, 'days');
            }
        }

        return intervalsModel;
    }
}