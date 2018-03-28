import { CalendarEvent, CalendarEventType } from './calendar-event.model';
import { IntervalsModel, IntervalType, ReadOnlyIntervalsModel, IntervalModel } from './calendar.model';
import moment from 'moment';

export class CalendarEvents {

    constructor(private readonly calendarEvents: CalendarEvent[]) {}

    public buildIntervalsModel(): IntervalsModel {
        const intervalsModel = new IntervalsModel();

        this.insertCalendarEvents(intervalsModel);

        return intervalsModel;
    }

    public appendToIntervalsModel(existingModel: IntervalsModel) {
        this.insertCalendarEvents(existingModel);
    }

    private insertCalendarEvents(intervalsModel: IntervalsModel) {

        for (let calendarEvent of this.calendarEvents) {
            const start = moment(calendarEvent.dates.startDate);

            if (calendarEvent.type === CalendarEventType.Dayoff || calendarEvent.type === CalendarEventType.Workout) {
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

            intervalsModel.intervalsMetadata.addCalendarEvent(calendarEvent);
        }

        return intervalsModel;
    }

    private buildIntervalBoundary(keyDate: moment.Moment, intervalsModel: IntervalsModel, calendarEvent: CalendarEvent) {
        const intervalType = this.getBoundaryType(calendarEvent);

        intervalsModel.set(keyDate, {
            intervalType: intervalType,
            calendarEvent: calendarEvent,
            boundary: true
        });
    }

    private getBoundaryType(calendarEvent: CalendarEvent): IntervalType | null {
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