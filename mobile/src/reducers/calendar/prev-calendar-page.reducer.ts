import { CalendarEventsState, CalendarPagesSubState } from './calendar-events.reducer';
import { PrevCalendarPage } from './calendar.action';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarPageModel } from './calendar.model';

export const prevCalendarPageReducer = (state: CalendarEventsState, action: PrevCalendarPage): CalendarPagesSubState => {
    const firstPage = state.pages[0];
    const firstPageDate = moment(firstPage.date);

    firstPageDate.add(-1, 'months');

    const firstBuilder = new CalendarWeeksBuilder();
    const firstMonthWeeks = firstBuilder.buildWeeks(firstPageDate.month(), firstPageDate.year());

    const isPageFirst = firstPageDate.year() === 1993;

    const pages = [new CalendarPageModel(firstPageDate, firstMonthWeeks, isPageFirst), ...state.pages];

    pages.pop();

    return {
        pages: pages
    };
};