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
    calendarEvent: CalendarEvents;
    boundary: boolean;
}

type IntervalsModelDictionary = {
    [dateKey: string]: IntervalModel[];
};

export class IntervalsModel {
    constructor(
        private readonly intervalsDictionary: IntervalsModelDictionary = {}
    ) { }

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

    public copy(): IntervalsModel {
        const copiedDictionary = { ...this.intervalsDictionary };

        const keys = Object.keys(copiedDictionary);

        for (let key of keys) {
            copiedDictionary[key] = copiedDictionary[key]
                ? [...copiedDictionary[key]]
                : copiedDictionary[key];
        }

        return new IntervalsModel(copiedDictionary);
    }

    public static generateKey(date: Moment): string {
        return date.format('DD-MM-YYYY');
    }
}

export interface SingleSelection {
    day: DayModel;
}

export interface IntervalSelection {
    startDay: DayModel;
    endDay: DayModel;
    color: string;
}

export interface CalendarSelection {
    single: SingleSelection;
    interval: IntervalSelection;
}

export class ExtractedIntervals {
    public readonly vacation: IntervalModel;
    public readonly dayoff: IntervalModel;
    public readonly sickleave: IntervalModel;

    constructor(intervals: IntervalModel[]) {
        if (intervals) {
            this.vacation = intervals.find(x => x.calendarEvent.type === CalendarEventsType.Vacation);
            this.dayoff = intervals.find(x => x.calendarEvent.type === CalendarEventsType.Dayoff || x.calendarEvent.type === CalendarEventsType.Workout);
            this.sickleave = intervals.find(x => x.calendarEvent.type === CalendarEventsType.Sickleave);
        }
    }
}