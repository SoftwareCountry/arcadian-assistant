import { DatesInterval } from '../calendar-event.model';
import moment from 'moment';

describe('DatesInterval', () => {
    let dates: DatesInterval;

    beforeEach(() => {
        dates = new DatesInterval();
        dates.startDate = moment({ day: 1, month: 0, year: 2018 });
        dates.endDate = moment({ day: 1, month: 1, year: 2018 });
    });

    it('should serialize startDate, endDate to DD/MM/YYYY date string', () => {
        expect(JSON.stringify(dates)).toBe('{"startDate":"01/01/2018","endDate":"02/01/2018"}');
    });
});
