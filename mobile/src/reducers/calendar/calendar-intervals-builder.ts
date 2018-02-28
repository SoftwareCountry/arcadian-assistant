import { CalendarEvents, CalendarEventsType } from './calendar-events.model';
import { IntervalsModel, IntervalType } from './calendar.model';
import moment from 'moment';

export class CalendarIntervalsBuilder {

    public buildIntervals(calendarEvents: CalendarEvents[]) {
        const intervalsModel = new IntervalsModel();

        for (let calendarEvent of calendarEvents) {
            const start = moment(calendarEvent.dates.startDate);

            if (calendarEvent.type === CalendarEventsType.Dayoff || calendarEvent.type === CalendarEventsType.AdditionalWork) {
                this.buildIntervalBoundary(start, intervalsModel, calendarEvent);
                continue;
            }

            if (start.isSame(calendarEvent.dates.endDate, 'day')) {
                intervalsModel.set(start, {
                    intervalType: 'intervalFullBoundary',
                    eventType: calendarEvent.type,
                    startDate: calendarEvent.dates.startDate,
                    endDate: calendarEvent.dates.endDate
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
                    eventType: calendarEvent.type,
                    startDate: calendarEvent.dates.startDate,
                    endDate: calendarEvent.dates.endDate
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
            eventType: calendarEvent.type,
            startDate: calendarEvent.dates.startDate,
            endDate: calendarEvent.dates.endDate
        });
    }

    private getBoundaryType(calendarEvent: CalendarEvents): IntervalType | null {
        const { startWorkingHour, finishWorkingHour } = calendarEvent.dates;

        if (startWorkingHour === 0 && finishWorkingHour === 0) {
            return null;
        }

        if ((startWorkingHour > 0 && startWorkingHour <= 4) && finishWorkingHour === 0) {
            return 'intervalLeftBoundary';
        }

        if (startWorkingHour === 0 && (finishWorkingHour >= 4 || finishWorkingHour <= 8)
        ) {
            return 'intervalRightBoundary';
        }

        if (startWorkingHour !== 0 && finishWorkingHour !== 0) {
            return 'intervalFullBoundary';
        }

        return null;
    }
}