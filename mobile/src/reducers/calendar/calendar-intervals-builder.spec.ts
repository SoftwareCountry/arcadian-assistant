import moment from 'moment';
import { CalendarWeeksBuilder, WeekModel, IntervalsModel, IntervalModel, CalendarIntervalsBuilder } from './calendar.model';
import { CalendarEvents, CalendarEventsType, DatesInterval } from './calendar-events.model';

describe('IntervalsModel', () => {
    it('should generate string key with "DD-MM-YYYY" format', () => {
        const date = moment();
        const key = IntervalsModel.generateKey(date);
        expect(key).toBe(date.format('DD-MM-YYYY'));
    });

    describe('set', () => {
        it('should create new array and put element, if there is no such key', () => {
            const intervalsModel = new IntervalsModel();
            const date = moment();
            const intervalModel: IntervalModel = {
                intervalType: 'startInterval',
                eventType: CalendarEventsType.AdditionalWork
            };

            intervalsModel.set(date, intervalModel);
            const array = intervalsModel.get(date);

            expect(array.length).toEqual(1);
            expect(array[0]).toEqual(intervalModel);
        });

        it('should add element to the existing array, if key exists', () => {
            const intervalsModel = new IntervalsModel();

            const key = moment();

            const intervalModel: IntervalModel = {
                intervalType: 'startInterval',
                eventType: CalendarEventsType.AdditionalWork
            };

            intervalsModel.set(key, intervalModel);

            const existingArray = intervalsModel.get(key);

            const intervalModel2: IntervalModel = {
                intervalType: 'startInterval',
                eventType: CalendarEventsType.AdditionalWork
            };

            intervalsModel.set(key, intervalModel2);

            const array = intervalsModel.get(key);

            expect(array).toBe(existingArray);
            expect(array[0]).toBe(intervalModel);
            expect(array[1]).toBe(intervalModel2);
        });
    });

    it('should get undefined, if there is no such key', () => {
        const intervalsModel = new IntervalsModel();
        const date = moment();
        const array = intervalsModel.get(date);
        expect(array).toBeUndefined();
    });

    it('should get elements by key', () => {
        const intervalsModel = new IntervalsModel();
        const date = moment();
        const intervalModel: IntervalModel = {
            intervalType: 'startInterval',
            eventType: CalendarEventsType.AdditionalWork
        };

        intervalsModel.set(date, intervalModel);
        const array = intervalsModel.get(date);

        expect(array.length).toEqual(1);
        expect(array[0]).toEqual(intervalModel);
    });
});

describe('CalendarIntervalsBuilder', () => {
    it('should build interval boundary if startDate and endDate is the same', () => {
        const event = new CalendarEvents();

        const date = moment();

        event.calendarEventId = '1';
        event.type = CalendarEventsType.Vacation;
        event.dates = new DatesInterval();
        event.dates.startDate = moment(date);
        event.dates.endDate = moment(date);

        const builder = new CalendarIntervalsBuilder();

        const intervalsModel = builder.buildIntervals([event]);

        const intervals = intervalsModel.get(date);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('dotInterval');
    });

    it('should build interval if endDate > startDate', () => {
        const event = new CalendarEvents();

        const date = moment({ day: 1, month: 0, year: 2018 });

        event.calendarEventId = '1';
        event.type = CalendarEventsType.Vacation;
        event.dates = new DatesInterval();
        event.dates.startDate = moment(date);
        event.dates.endDate = moment(date);

        event.dates.endDate.add(3, 'days');

        const builder = new CalendarIntervalsBuilder();
        const intervalsModel = builder.buildIntervals([event]);

        const one = moment({ day: 1, month: 0, year: 2018 });
        let intervals = intervalsModel.get(one);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('startInterval');

        const two = moment({ day: 2, month: 0, year: 2018 });
        intervals = intervalsModel.get(two);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('interval');

        const three = moment({ day: 3, month: 0, year: 2018 });
        intervals = intervalsModel.get(three);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('interval');

        const four = moment({ day: 4, month: 0, year: 2018 });
        intervals = intervalsModel.get(four);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('endInterval');
    });

    it('should build crossing intervals', () => {
        /*
            event1: [01-01-2018  02-01-2018  03-01-2018]
            event2:             [02-01-2018  03-01-2018  04-01-2018]
                                ----------------------
                                    crossing dates
        */

        const date1 = moment({ day: 1, month: 0, year: 2018 });

        const event1 = new CalendarEvents();
        event1.calendarEventId = '1';
        event1.type = CalendarEventsType.Vacation;
        event1.dates = new DatesInterval();
        event1.dates.startDate = moment(date1);
        event1.dates.endDate = moment(date1);
        event1.dates.endDate.add(2, 'days');

        const date2 = moment({ day: 2, month: 0, year: 2018 });

        const event2 = new CalendarEvents();
        event2.calendarEventId = '1';
        event2.type = CalendarEventsType.SickLeave;
        event2.dates = new DatesInterval();
        event2.dates.startDate = moment(date2);
        event2.dates.endDate = moment(date2);
        event2.dates.endDate.add(2, 'days');

        const builder = new CalendarIntervalsBuilder();
        const intervalsModel = builder.buildIntervals([event1, event2]);

        let intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

        expect(intervals.length).toBe(1);
        expect(intervals[0].intervalType).toBe('startInterval');
        expect(intervals[0].eventType).toBe(CalendarEventsType.Vacation);
        
        intervals = intervalsModel.get(moment({ day: 2, month: 0, year: 2018 }));

        expect(intervals.length).toBe(2);
        expect(intervals[0].intervalType).toBe('interval');
        expect(intervals[0].eventType).toBe(CalendarEventsType.Vacation);
        expect(intervals[1].intervalType).toBe('startInterval');
        expect(intervals[1].eventType).toBe(CalendarEventsType.SickLeave);

        intervals = intervalsModel.get(moment({ day: 3, month: 0, year: 2018 }));

        expect(intervals.length).toBe(2);
        expect(intervals[0].intervalType).toBe('endInterval');
        expect(intervals[0].eventType).toBe(CalendarEventsType.Vacation);
        expect(intervals[1].intervalType).toBe('interval');
        expect(intervals[1].eventType).toBe(CalendarEventsType.SickLeave);

        intervals = intervalsModel.get(moment({ day: 4, month: 0, year: 2018 }));

        expect(intervals.length).toBe(1);
        expect(intervals[0].intervalType).toBe('endInterval');
        expect(intervals[0].eventType).toBe(CalendarEventsType.SickLeave);
    });
});