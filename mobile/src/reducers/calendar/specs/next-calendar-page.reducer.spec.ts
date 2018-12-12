import { calendarEventsReducer, CalendarEventsState, CalendarPagesSubState } from '../calendar-events.reducer';
import moment from 'moment';
import { nextCalendarPage } from '../calendar.action';
import { CalendarPageModel } from '../calendar.model';
import { createCalendarPagesInitState } from '../calendar-pages-init-state';
import { nextCalendarPageReducer } from '../next-calendar-page.reducer';

describe('next calendar page reducer', () => {
    let state: CalendarEventsState;
    let initPages: CalendarPageModel[];

    beforeEach(() => {
        state = calendarEventsReducer(undefined, { type: '' });
        initPages = [...state.pages];
    });

    describe('when next calendar page', () => {

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

    describe('when next page belongs to 9999 year', () => {
        const year = 9999;
        let subState: CalendarPagesSubState;

        beforeEach(() => {
            const pages = createCalendarPagesInitState(moment({ day: 1, month: 0, year: year }));
            const action = nextCalendarPage();
            subState = nextCalendarPageReducer({ ...state, pages: pages }, action);
        });

        it('should mark the next page as calendar last page', () => {
            const [prevPage, currentPage, nextPage] = subState.pages;
            expect(prevPage.isPageLast).toBeFalsy();
            expect(currentPage.isPageLast).toBeFalsy();
            expect(nextPage.isPageLast).toBeTruthy();
        });
    });
});
