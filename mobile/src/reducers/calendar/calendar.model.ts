import moment, { Moment } from 'moment';

export interface DayModel {
    date: Moment;
    today: boolean;
    belongsToCurrentMonth: boolean;
}

export interface WeekModel {
    days: DayModel[];
    weekIndex: number;
}

export class CalendarModelBuilder {
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

    public createWeeks(currentMonth: number, currentYear: number): WeekModel[] {
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