import { CalendarPageModel } from '../calendar.model';
import moment, { Moment } from 'moment';

describe('calendar page model', () => {
    let calendarPageModel: CalendarPageModel;
    let date: Moment;

    beforeEach(() => {
        date = moment();
        calendarPageModel = new CalendarPageModel(date, []);
    });

    it('should generate pageId', () => {
        expect(calendarPageModel.pageId).toBe(date.format('MMMM YYYY'));
    });
});