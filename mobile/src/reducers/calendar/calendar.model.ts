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

type PeriodType = 'startPeriod' | 'period' | 'endPeriod' | 'dotPeriod';

export interface PeriodModel {
    periodType: PeriodType;
    eventType: CalendarEventsType;
}

export class PeriodsModel {
    private periodsDictionary: {
        // serjKim: maybe object with event type specific keys:
        // { [dateKey: string] { 'Vacation':  PeriodModel, 'Dayoff': PeriodModel } } instead of array PeriodModel[]...
        [dateKey: string]: PeriodModel[]; 
    } = {};

    public set(date: Moment, period: PeriodModel) {
        const dateKey = PeriodsModel.generateKey(date);

        let periods = this.periodsDictionary[dateKey];

        if (!periods) {
            this.periodsDictionary[dateKey] = periods = [];
        }

        periods.push(period);
    }

    public get(date: Moment): PeriodModel[] | undefined {
        const dateKey = PeriodsModel.generateKey(date);
        return this.periodsDictionary[dateKey];
    }

    public static generateKey(date: Moment): string {
        return date.format('DD-MM-YYYY');
    }
}

export class CalendarPeriodsBuilder {

    public buildPeriods(calendarEvents: CalendarEvents[]) {
        const periodsModel = new PeriodsModel();

        for (let calendarEvent of calendarEvents) {
            const start = moment(calendarEvent.dates.startDate);

            if (start.isSame(calendarEvent.dates.endDate, 'day')) {
                periodsModel.set(start, {
                    periodType: 'dotPeriod',
                    eventType: calendarEvent.type
                });
                continue;
            }

            while (start.isSameOrBefore(calendarEvent.dates.endDate, 'day')) {
                let periodType: PeriodType = 'period';

                if (start.isSame(calendarEvent.dates.startDate)) {
                    periodType = 'startPeriod';
                } else if (start.isSame(calendarEvent.dates.endDate)) {
                    periodType = 'endPeriod';
                }

                periodsModel.set(start, {
                    periodType: periodType,
                    eventType: calendarEvent.type
                });

                start.add(1, 'days');
            }
        }

        return periodsModel;
    }
}