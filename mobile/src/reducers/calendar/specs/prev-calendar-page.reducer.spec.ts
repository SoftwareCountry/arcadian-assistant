import { calendarEventsReducer, CalendarEventsState } from '../calendar-events.reducer';
import moment, { Moment } from 'moment';
import { prevCalendarPage } from '../calendar.action';
import { CalendarPageModel } from '../calendar.model';

describe('prev calendar page reducer', () => {
    let state: CalendarEventsState;
    let initPages: CalendarPageModel[];

    beforeEach(() => {
        state = calendarEventsReducer(undefined, { type: '' });
        initPages = [...state.pages];
    });

    beforeEach(() => {
        const action = prevCalendarPage();
        state = calendarEventsReducer(state, action);
    });

    it('should add a page to the head and remove the last page from the tail', () => {
        const [oldPrevPage, oldCurrentPage, oldNextPage] = initPages;
        const [prevPage, currentPage, nextPage] = state.pages;

        expect(oldPrevPage).toBe(currentPage);
        expect(oldCurrentPage).toBe(nextPage);

        const date = moment(currentPage.date);
        date.add(-1, 'months');

        expect(prevPage.date.isSame(date, 'day')).toBeTruthy();
    });
});