import {CalendarEventsState, CalendarPagesSubState} from '../calendar-events.reducer';
import {SelectCalendarDay} from '../calendar.action';
import moment from 'moment';
import {createCalendarPagesInitState} from '../calendar-pages-init-state';

export const resetCalendarPageReducer = (state: CalendarEventsState, action: SelectCalendarDay): CalendarPagesSubState => {
    const [
        prevPage,
        currentPage,
        nextPage
    ] = createCalendarPagesInitState(moment());

    return {
        pages: [prevPage, currentPage, nextPage],
    };
};
