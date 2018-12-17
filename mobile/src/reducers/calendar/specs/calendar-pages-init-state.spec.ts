import { CalendarPageModel } from '../calendar.model';
import { createCalendarPagesInitState } from '../calendar-pages-init-state';
import moment, { Moment } from 'moment';

describe('when calendar pages state initialized', () => {
    let initState: CalendarPageModel[];
    let currentDate: Moment;

    beforeEach(() => {
        currentDate = moment();
        initState = createCalendarPagesInitState(currentDate);
    });

    it('should return three pages (previos, current, next)', () => {
        expect(initState.length).toBe(3);
    });

    it('should have current page with current month', () => {
        const [prev, curr, next] = initState;

        expect(currentDate.isSame(curr.date, 'day')).toBeTruthy();
    });

    it('should have previous page with previous month', () => {
        const [prev, curr, next] = initState;

        const prevDate = moment(currentDate);
        prevDate.add(-1, 'months');

        expect(prevDate.isSame(prev.date, 'day')).toBeTruthy();
    });

    it('should have next page with next month', () => {
        const [prev, curr, next] = initState;

        const nextDate = moment(currentDate);
        nextDate.add(1, 'months');

        expect(nextDate.isSame(next.date, 'day')).toBeTruthy();
    });
});
