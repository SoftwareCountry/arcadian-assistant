import { CalendarEvents, CalendarEventsType } from './calendar-events.model';
import { IntervalsModel, IntervalType } from './calendar.model';
import moment from 'moment';

interface BuildConfig {
    /**
     * Intervals which has just been added but not saved yet
     */
    draft?: boolean;
}

export class CalendarIntervalsBuilder {

    public buildIntervals(
        calendarEvents: CalendarEvents[],
        config: BuildConfig = {}
    ) {
        const intervalsModel = new IntervalsModel();

        this.insertCalendarEvents(intervalsModel, calendarEvents, config);

        return intervalsModel;
    }

    public appendCalendarEvents(
        existingModel: IntervalsModel,
        calendarEvents: CalendarEvents[],
        config: BuildConfig = {}
    ) {
        this.insertCalendarEvents(existingModel, calendarEvents, config);
    }

    private insertCalendarEvents(
        intervalsModel: IntervalsModel,
        calendarEvents: CalendarEvents[],
        { draft = false }: BuildConfig = {}
    ) {

        for (let calendarEvent of calendarEvents) {
            const start = moment(calendarEvent.dates.startDate);

            if (calendarEvent.type === CalendarEventsType.Dayoff || calendarEvent.type === CalendarEventsType.Workout) {
                this.buildIntervalBoundary(start, intervalsModel, calendarEvent, draft);
                continue;
            }

            if (start.isSame(calendarEvent.dates.endDate, 'day')) {
                intervalsModel.set(start, {
                    intervalType: 'intervalFullBoundary',
                    eventType: calendarEvent.type,
                    startDate: calendarEvent.dates.startDate,
                    endDate: calendarEvent.dates.endDate,
                    boundary: true,
                    draft: draft
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
                    endDate: calendarEvent.dates.endDate,
                    boundary: false,
                    draft: draft
                });

                start.add(1, 'days');
            }
        }

        return intervalsModel;
    }

    private buildIntervalBoundary(keyDate: moment.Moment, intervalsModel: IntervalsModel, calendarEvent: CalendarEvents, draft: boolean) {
        const intervalType = this.getBoundaryType(calendarEvent);

        intervalsModel.set(keyDate, {
            intervalType: intervalType,
            eventType: calendarEvent.type,
            startDate: calendarEvent.dates.startDate,
            endDate: calendarEvent.dates.endDate,
            boundary: true,
            draft: draft
        });
    }

    private getBoundaryType(calendarEvent: CalendarEvents): IntervalType | null {
        const { startWorkingHour, finishWorkingHour } = calendarEvent.dates;

        if (0 <= startWorkingHour && finishWorkingHour <= 4) {
            return 'intervalLeftBoundary';
        }

        if (4 <= startWorkingHour && finishWorkingHour <= 8) {
            return 'intervalRightBoundary';
        }

        if (0 <= startWorkingHour && finishWorkingHour <= 8) {
            return 'intervalFullBoundary';
        }

        return null;
    }
}