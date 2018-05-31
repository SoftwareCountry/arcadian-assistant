import { calendarEventsReducer, CalendarEventsState, CalendarPagesSubState } from '../calendar-events.reducer';
import moment, { Moment } from 'moment';
import { prevCalendarPage } from '../calendar.action';
import { CalendarPageModel } from '../calendar.model';
import { createCalendarPagesInitState } from '../calendar-pages-init-state';
import { prevCalendarPageReducer } from '../prev-calendar-page.reducer';

describe('prev calendar page reducer', () => {
    let state: CalendarEventsState;
    let initPages: CalendarPageModel[];

    beforeEach(() => {
        state = calendarEventsReducer(undefined, { type: '' });
        initPages = [...state.pages];
    });

    describe('when prev calendar page', () => {
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

    describe('when prev page belongs to 1993 year', () => {
        const year = 1993;
        let subState: CalendarPagesSubState;

        beforeEach(() => {
            const pages = createCalendarPagesInitState(moment({ day: 1, month: 0, year: year + 1 }));
            const action = prevCalendarPage();
            subState = prevCalendarPageReducer({ ...state, pages: pages }, action);
        });

        it('should mark the prev page as calendar first page', () => {
            const [prevPage, currentPage, nextPage] = subState.pages;
            expect(prevPage.isPageFirst).toBeTruthy();
            expect(currentPage.isPageFirst).toBeFalsy();
            expect(nextPage.isPageFirst).toBeFalsy();
        });
    });
});