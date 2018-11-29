import moment, { Moment } from 'moment';
import { CalendarEvent, CalendarEventType, CalendarEventStatus } from './calendar-event.model';
import { IntervalsMetadata, ReadOnlyIntervalsMetadata } from './calendar-intervals-metadata.model';
import { Nullable, Optional } from 'types';

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
    intervalType: Optional<IntervalType>;
    calendarEvent: CalendarEvent;
    boundary: boolean;
}

export interface ReadOnlyIntervalsModel {
    readonly metadata: ReadOnlyIntervalsMetadata;

    get(date: Moment): IntervalModel[] | undefined;
    copy(): IntervalsModel;
}

type IntervalsModelDictionary = {
    [dateKey: string]: IntervalModel[];
};

export class IntervalsModel implements ReadOnlyIntervalsModel {
    constructor(
        private readonly intervalsDictionary: IntervalsModelDictionary = {},
        public readonly intervalsMetadata: IntervalsMetadata = new IntervalsMetadata([])
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

    public get metadata(): ReadOnlyIntervalsMetadata {
        return this.intervalsMetadata;
    }

    public copy(): IntervalsModel {
        const copiedDictionary = { ...this.intervalsDictionary };

        const keys = Object.keys(copiedDictionary);

        for (let key of keys) {
            copiedDictionary[key] = copiedDictionary[key]
                ? [...copiedDictionary[key]]
                : copiedDictionary[key];
        }

        const copiedMetadata = this.metadata.copy();

        return new IntervalsModel(copiedDictionary, copiedMetadata);
    }

    public static generateKey(date: Moment): string {
        return date.format('DD-MM-YYYY');
    }
}

export interface SingleSelection {
    day: Optional<DayModel>;
}

export interface IntervalSelection {
    startDay: Optional<DayModel>;
    endDay: Optional<DayModel>;
    color: Optional<string>;
}

export interface CalendarSelection {
    single: SingleSelection;
    interval: Optional<IntervalSelection>;
}

export class ExtractedIntervals {
    public readonly vacation: Optional<IntervalModel> = null;
    public readonly dayoff: Optional<IntervalModel> = null;
    public readonly sickleave: Optional<IntervalModel> = null;

    constructor(intervals: Optional<IntervalModel[]>) {
        if (intervals) {
            this.vacation = intervals.find(x => x.calendarEvent.type === CalendarEventType.Vacation);
            this.dayoff = intervals.find(x => x.calendarEvent.type === CalendarEventType.Dayoff || x.calendarEvent.type === CalendarEventType.Workout);
            this.sickleave = intervals.find(x => x.calendarEvent.type === CalendarEventType.Sickleave);
        }
    }
}

export class CalendarPageModel {
    public readonly pageId: string;

    constructor(
        public readonly date: Moment,
        public readonly weeks: WeekModel[],
        public readonly isPageFirst: boolean = false,
        public readonly isPageLast: boolean = false
    ) {
        this.pageId = date.format('MMMM YYYY');
    }
}
