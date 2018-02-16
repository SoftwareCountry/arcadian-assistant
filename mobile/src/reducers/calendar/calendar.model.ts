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

export class CalendarWeeksBuilder {
    private readonly weeksPerPage = 6;
    private readonly daysPerWeek = 7;

    public fillWeekWithPrevMonthDays(
        weekModel: WeekModel,
        currentWeek: number,
        date: Moment,
        today: Moment
    ) {
        const before = moment(date);
        before.add(-1, 'days');

        while (before.week() === currentWeek) {

            const day = {
                date: moment(before),
                today: before.isSame(today, 'day'),
                belongsToCurrentMonth: false
            };

            weekModel.days.unshift(day);

            before.add(-1, 'days');
        }
    }

    public buildWeeks(currentMonth: number, currentYear: number): WeekModel[] {
        const date = moment({
            date: 1, // start filling from the first day of the month
            month: currentMonth,
            year: currentYear });

        const currentWeek = date.week();

        const weeksResult: WeekModel[] = [];

        let weekIndex = 1;
        let weekModel: WeekModel = { days: [], weekIndex };

        const today = moment();

        this.fillWeekWithPrevMonthDays(
            weekModel,
            currentWeek,
            date,
            today);

        while (weekIndex <= this.weeksPerPage) {
            if (weekModel.days.length === this.daysPerWeek) {
                weeksResult.push(weekModel);
                ++weekIndex;
                weekModel = { days: [], weekIndex };
            }

            const day = {
                date: moment(date),
                today: date.isSame(today, 'day'),
                belongsToCurrentMonth: date.month() === currentMonth
            };

            weekModel.days.push(day);

            date.add(1, 'days');
        }

        return weeksResult;
    }
}

type IntervalType = 'startInterval' | 'interval' | 'endInterval' | 'intervalBoundary';

export interface IntervalModel {
    intervalType: IntervalType;
    eventType: CalendarEventsType;
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

export class CalendarIntervalsBuilder {

    public buildIntervals(calendarEvents: CalendarEvents[]) {
        const intervalsModel = new IntervalsModel();

        for (let calendarEvent of calendarEvents) {
            const start = moment(calendarEvent.dates.startDate);

            if (start.isSame(calendarEvent.dates.endDate, 'day')) {
                intervalsModel.set(start, {
                    intervalType: 'intervalBoundary',
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