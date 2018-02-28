import moment, { Moment } from 'moment';
import { CalendarEvents, CalendarEventsType } from './calendar-events.model';

export interface DayModel {
    date: Moment;
    today: boolean;
    belongsToCurrentMonth: boolean;
}

export interface WeekModel {
    days: DayModel[];
    weekIndex: number;
}

export type IntervalType = 'startInterval' | 'interval' | 'endInterval' | 'intervalFullBoundary' | 'intervalLeftBoundary' | 'intervalRightBoundary';

export interface IntervalModel {
    intervalType: IntervalType;
    eventType: CalendarEventsType;
    startDate: moment.Moment;
    endDate: moment.Moment;
}

export class IntervalsModel {
    private intervalsDictionary: {
        [dateKey: string]: IntervalModel[];
    } = {};

    public set(date: Moment, interval: IntervalModel) {
        const dateKey = IntervalsModel.generateKey(date);

        let intervals = this.intervalsDictionary[dateKey];

        if (!intervals) {
            this.intervalsDictionary[dateKey] = intervals = [];
        }

        intervals.push(interval);
    }

    public get(date: Moment): IntervalModel[] | undefined {
        const dateKey = IntervalsModel.generateKey(date);
        return this.intervalsDictionary[dateKey];
    }

    public static generateKey(date: Moment): string {
        return date.format('DD-MM-YYYY');
    }
}