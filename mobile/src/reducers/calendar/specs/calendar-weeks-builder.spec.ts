import { WeekModel } from '../calendar.model';
import moment from 'moment';
import { CalendarWeeksBuilder } from '../calendar-weeks-builder';

describe('calendarWeeksBuilder', () => {
    describe('fillWeekWithPrevMonthDays', () => {
        it('should fill the week with days going until the first day', () => {
            const builder = new CalendarWeeksBuilder();

            /*
                        2018
                Su Mo Tu We Th Fr Sa
                28 29 30 31 1  2  3
                ----------- -------
                Prev month  february
            */

            const currentDate = moment({ day: 1, month: 1, year: 2018 });
            const currentWeek = currentDate.week();

            const weekModel: WeekModel = { days: [], weekIndex: 1 };

            builder.fillWeekWithPrevMonthDays(weekModel, currentWeek, currentDate, moment());

            const prevMonthDays = [28, 29, 30, 31];

            expect(weekModel.days.length).toBe(prevMonthDays.length);

            const daysBeforeFirstMonthDay = weekModel.days.map(x => x.date.date());

            expect(daysBeforeFirstMonthDay).toEqual(prevMonthDays);

            expect(weekModel.days.every(x => !x.belongsToCurrentMonth)).toBeTruthy();
        });

        it('should not fill the week, if there are no days going before the first day', () => {
            const builder = new CalendarWeeksBuilder();

            /*
                        2017
                Su Mo Tu We Th Fr Sa
                1  2  3  4  5  6  7
                --------------------
                    October
            */

            const currentDate = moment({ day: 1, month: 9, year: 2017 });
            const currentWeek = currentDate.week();

            const weekModel: WeekModel = { days: [], weekIndex: 1 };

            builder.fillWeekWithPrevMonthDays(weekModel, currentWeek, currentDate, moment());

            const prevMonthDays: number[] = [];

            expect(weekModel.days.length).toBe(prevMonthDays.length);

            const daysBeforeFirstMonthDay = weekModel.days.map(x => x.date.date());

            expect(daysBeforeFirstMonthDay).toEqual(prevMonthDays);
        });
    });

    describe('buildWeeks', () => {
        it('should build 6 weeks per calendar page', () => {
            const builder = new CalendarWeeksBuilder();

            const weeks = builder.buildWeeks(1, 2017);

            expect(weeks.length).toBe(6);
        });

        it('should build 7 days per calendar week', () => {
            const builder = new CalendarWeeksBuilder();

            const weeks = builder.buildWeeks(1, 2017);

            weeks.forEach(x => expect(x.days.length).toBe(7));
        });
    });
});