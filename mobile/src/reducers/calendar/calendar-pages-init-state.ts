import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import moment, { Moment } from 'moment';
import { CalendarPageModel } from './calendar.model';

export const createCalendarPagesInitState = (date: Moment): CalendarPageModel[] => {
    const builder = new CalendarWeeksBuilder();

    const prevDate = moment(date);
    prevDate.add(-1, 'months');

    const prevMonthWeeks = builder.buildWeeks(prevDate.month(), prevDate.year());

    const currentDate = moment(date);
    const currentMonthWeeks = builder.buildWeeks(currentDate.month(), currentDate.year());

    const nextDate = moment(date);
    nextDate.add(1, 'months');

    const nextMonthWeeks = builder.buildWeeks(nextDate.month(), nextDate.year());

    return [
        new CalendarPageModel(prevDate, prevMonthWeeks),
        new CalendarPageModel(currentDate, currentMonthWeeks),
        new CalendarPageModel(nextDate, nextMonthWeeks)
    ];
};