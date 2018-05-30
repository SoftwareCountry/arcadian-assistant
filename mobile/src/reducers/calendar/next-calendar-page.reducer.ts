import { CalendarEventsState, CalendarPagesSubState } from './calendar-events.reducer';
import { NextCalendarPage } from './calendar.action';
import moment from 'moment';
import { CalendarWeeksBuilder } from './calendar-weeks-builder';
import { CalendarPageModel } from './calendar.model';

export const nextCalendarPageReducer = (state: CalendarEventsState, action: NextCalendarPage): CalendarPagesSubState => {
    const lastPage = state.pages[state.pages.length - 1];
    const lastPageDate = moment(lastPage.date);

    lastPageDate.add(1, 'months');

    const lastBuilder = new CalendarWeeksBuilder();
    const lastMonthWeeks = lastBuilder.buildWeeks(lastPageDate.month(), lastPageDate.year());

    const pages = [...state.pages, new CalendarPageModel(lastPageDate, lastMonthWeeks)];

    pages.shift();

    return {
        pages: pages
    };
};