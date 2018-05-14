import { dataMember, required } from 'santee-dcts';
import { CalendarEvent, CalendarEventType } from './calendar-event.model';
import { IntervalsModel, IntervalType, ReadOnlyIntervalsModel, IntervalModel } from './calendar.model';
import moment from 'moment';
import { IntervalTypeConverter } from './interval-type-converter';

export class PendingRequests {
    @dataMember()
    @required()
    public events: Map<string, CalendarEvent[]>;
}

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

    public get all(): CalendarEvent[] {
        return this.calendarEvents;
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
                intervalsModel.intervalsMetadata.addCalendarEvent(calendarEvent);
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

        intervalsModel.intervalsMetadata.addCalendarEvent(calendarEvent);
    }

    private getBoundaryType(calendarEvent: CalendarEvent): IntervalType | null {
        const { startWorkingHour, finishWorkingHour } = calendarEvent.dates;

        const intervalType = IntervalTypeConverter.hoursToIntervalType(startWorkingHour, finishWorkingHour);

        if (!intervalType) {
            return null;
        }

        return intervalType;
    }
}