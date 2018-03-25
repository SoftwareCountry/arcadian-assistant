import moment, { Moment } from 'moment';
import { CalendarEvent, CalendarEventType, CalendarEventStatus } from './calendar-event.model';

export interface DayModel {
    date: Moment;
    today: boolean;
    belongsToCurrentMonth: boolean;
}

export interface WeekModel {
    days: DayModel[];
    weekIndex: number;
}

export enum IntervalType {
    StartInterval = 'StartInterval',
    Interval = 'Interval',
    EndInterval = 'EndInterval',
    IntervalFullBoundary = 'IntervalFullBoundary',
    IntervalLeftBoundary = 'IntervalLeftBoundary',
    IntervalRightBoundary =  'IntervalRightBoundary'
}

export interface IntervalModel {
    intervalType: IntervalType;
    calendarEvent: CalendarEvent;
    boundary: boolean;
}

export interface ReadOnlyIntervalsModel {
    get(date: Moment): IntervalModel[] | undefined;
    copy(): IntervalsModel;
}

type IntervalsModelDictionary = {
    [dateKey: string]: IntervalModel[];
};

export class IntervalsModel implements ReadOnlyIntervalsModel {
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
            this.vacation = intervals.find(x => x.calendarEvent.type === CalendarEventType.Vacation);
            this.dayoff = intervals.find(x => x.calendarEvent.type === CalendarEventType.Dayoff || x.calendarEvent.type === CalendarEventType.Workout);
            this.sickleave = intervals.find(x => x.calendarEvent.type === CalendarEventType.Sickleave);
        }
    }
}