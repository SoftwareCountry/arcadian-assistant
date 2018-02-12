// import { CalendarPage, WeekModel, DayModel } from './calendar-page';
// import moment from 'moment';

// describe('calendar', () => {
//     it('should fill the week with days going until the first day', () => {
//         const calendar = new CalendarPage({ hidePrevNextMonthDays: false });

//         /*
//                     2018
//             Su Mo Tu We Th Fr Sa
//             28 29 30 31 1  2  3
//             ----------- -------
//             Prev month  february
//         */

//         const currentDate = moment({ day: 1, month: 1, year: 2018 });
//         const currentWeek = currentDate.week();

//         const weekModel: WeekModel<DayModel> = { days: [], weekIndex: 1 };

//         calendar.fillWeekWithPrevMonthDays(weekModel, currentWeek, currentDate, moment(), day => day);

//         const prevMonthDays = [28, 29, 30, 31];

//         expect(weekModel.days.length).toBe(prevMonthDays.length);

//         const daysBeforeFirstMonthDay = weekModel.days.map(x => x.day);

//         expect(daysBeforeFirstMonthDay).toEqual(prevMonthDays);

//     });

//     it('should not fill the week, if there are no days going before the first day', () => {
//         const calendar = new CalendarPage({ hidePrevNextMonthDays: false, calendarEvents: [] });

//         /*
//                     2017
//             Su Mo Tu We Th Fr Sa
//             1  2  3  4  5  6  7
//             --------------------
//                 October
//         */

//         const currentDate = moment({ day: 1, month: 9, year: 2017 });
//         const currentWeek = currentDate.week();

//         const weekModel: WeekModel<DayModel> = { days: [], weekIndex: 1 };

//         calendar.fillWeekWithPrevMonthDays(weekModel, currentWeek, currentDate, moment(), day => day);

//         const prevMonthDays: number[] = [];

//         expect(weekModel.days.length).toBe(prevMonthDays.length);

//         const daysBeforeFirstMonthDay = weekModel.days.map(x => x.day);

//         expect(daysBeforeFirstMonthDay).toEqual(prevMonthDays);
//     });
// });