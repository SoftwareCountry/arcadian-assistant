import { calendarEventsReducer, CalendarEventsState } from '../calendar-events.reducer';
import moment, { Moment } from 'moment';
import { nextCalendarPage } from '../calendar.action';
import { CalendarPageModel } from '../calendar.model';

describe('next calendar page reducer', () => {
    let state: CalendarEventsState;
    let initPages: CalendarPageModel[];

    beforeEach(() => {
        state = calendarEventsReducer(undefined, { type: '' });
        initPages = [...state.pages];
    });

    beforeEach(() => {
        const action = nextCalendarPage();
        state = calendarEventsReducer(state, action);
    });

    it('should add a page to the tail and remove the first page from the head', () => {
        const [oldPrevPage, oldCurrentPage, oldNextPage] = initPages;
        const [prevPage, currentPage, nextPage] = state.pages;

        expect(oldCurrentPage).toBe(prevPage);
        expect(oldNextPage).toBe(currentPage);

        const date = moment(currentPage.date);
        date.add(1, 'months');

        expect(nextPage.date.isSame(date, 'day')).toBeTruthy();
    });
});