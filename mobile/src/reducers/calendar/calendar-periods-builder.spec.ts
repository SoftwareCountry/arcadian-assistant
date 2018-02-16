import moment from 'moment';
import { CalendarWeeksBuilder, WeekModel, PeriodsModel, PeriodModel, CalendarPeriodsBuilder } from './calendar.model';
import { CalendarEvents, CalendarEventsType, DatesPeriod } from './calendar-events.model';

describe('PeriodsModel', () => {
    it('should generate string key with "DD-MM-YYYY" format', () => {
        const date = moment();
        const key = PeriodsModel.generateKey(date);
        expect(key).toBe(date.format('DD-MM-YYYY'));
    });

    describe('set', () => {
        it('should create new array and put element, if there is no such key', () => {
            const periodsModel = new PeriodsModel();
            const date = moment();
            const periodModel: PeriodModel = {
                periodType: 'startPeriod',
                eventType: CalendarEventsType.AdditionalWork
            };

            periodsModel.set(date, periodModel);
            const array = periodsModel.get(date);

            expect(array.length).toEqual(1);
            expect(array[0]).toEqual(periodModel);
        });

        it('should add element to the existing array, if key exists', () => {
            const periodsModel = new PeriodsModel();

            const key = moment();

            const periodModel: PeriodModel = {
                periodType: 'startPeriod',
                eventType: CalendarEventsType.AdditionalWork
            };

            periodsModel.set(key, periodModel);

            const existingArray = periodsModel.get(key);

            const periodModel2: PeriodModel = {
                periodType: 'startPeriod',
                eventType: CalendarEventsType.AdditionalWork
            };

            periodsModel.set(key, periodModel2);

            const array = periodsModel.get(key);

            expect(array).toBe(existingArray);
            expect(array[0]).toBe(periodModel);
            expect(array[1]).toBe(periodModel2);
        });
    });

    it('should get undefined, if there is no such key', () => {
        const periodsModel = new PeriodsModel();
        const date = moment();
        const array = periodsModel.get(date);
        expect(array).toBeUndefined();
    });

    it('should get elements by key', () => {
        const periodsModel = new PeriodsModel();
        const date = moment();
        const periodModel: PeriodModel = {
            periodType: 'startPeriod',
            eventType: CalendarEventsType.AdditionalWork
        };

        periodsModel.set(date, periodModel);
        const array = periodsModel.get(date);

        expect(array.length).toEqual(1);
        expect(array[0]).toEqual(periodModel);
    });
});

describe('CalendarPeriodsBuilder', () => {
    it('should build dot-period if startDate and endDate is the same', () => {
        const event = new CalendarEvents();

        const date = moment();

        event.calendarEventId = '1';
        event.type = CalendarEventsType.Vacation;
        event.dates = new DatesPeriod();
        event.dates.startDate = moment(date);
        event.dates.endDate = moment(date);

        const builder = new CalendarPeriodsBuilder();

        const periodsModel = builder.buildPeriods([event]);

        const periods = periodsModel.get(date);

        expect(periods[0].eventType).toBe(event.type);
        expect(periods[0].periodType).toBe('dotPeriod');
    });

    it('should build period if endDate > startDate', () => {
        const event = new CalendarEvents();

        const date = moment({ day: 1, month: 0, year: 2018 });

        event.calendarEventId = '1';
        event.type = CalendarEventsType.Vacation;
        event.dates = new DatesPeriod();
        event.dates.startDate = moment(date);
        event.dates.endDate = moment(date);

        event.dates.endDate.add(3, 'days');

        const builder = new CalendarPeriodsBuilder();
        const periodsModel = builder.buildPeriods([event]);

        const one = moment({ day: 1, month: 0, year: 2018 });
        let periods = periodsModel.get(one);

        expect(periods[0].eventType).toBe(event.type);
        expect(periods[0].periodType).toBe('startPeriod');

        const two = moment({ day: 2, month: 0, year: 2018 });
        periods = periodsModel.get(two);

        expect(periods[0].eventType).toBe(event.type);
        expect(periods[0].periodType).toBe('period');

        const three = moment({ day: 3, month: 0, year: 2018 });
        periods = periodsModel.get(three);

        expect(periods[0].eventType).toBe(event.type);
        expect(periods[0].periodType).toBe('period');

        const four = moment({ day: 4, month: 0, year: 2018 });
        periods = periodsModel.get(four);

        expect(periods[0].eventType).toBe(event.type);
        expect(periods[0].periodType).toBe('endPeriod');
    });

    it('should build crossing periods', () => {
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
        event1.dates = new DatesPeriod();
        event1.dates.startDate = moment(date1);
        event1.dates.endDate = moment(date1);
        event1.dates.endDate.add(2, 'days');

        const date2 = moment({ day: 2, month: 0, year: 2018 });

        const event2 = new CalendarEvents();
        event2.calendarEventId = '1';
        event2.type = CalendarEventsType.SickLeave;
        event2.dates = new DatesPeriod();
        event2.dates.startDate = moment(date2);
        event2.dates.endDate = moment(date2);
        event2.dates.endDate.add(2, 'days');

        const builder = new CalendarPeriodsBuilder();
        const periodsModel = builder.buildPeriods([event1, event2]);

        let periods = periodsModel.get(moment({ day: 1, month: 0, year: 2018 }));

        expect(periods.length).toBe(1);
        expect(periods[0].periodType).toBe('startPeriod');
        expect(periods[0].eventType).toBe(CalendarEventsType.Vacation);
        
        periods = periodsModel.get(moment({ day: 2, month: 0, year: 2018 }));

        expect(periods.length).toBe(2);
        expect(periods[0].periodType).toBe('period');
        expect(periods[0].eventType).toBe(CalendarEventsType.Vacation);
        expect(periods[1].periodType).toBe('startPeriod');
        expect(periods[1].eventType).toBe(CalendarEventsType.SickLeave);

        periods = periodsModel.get(moment({ day: 3, month: 0, year: 2018 }));

        expect(periods.length).toBe(2);
        expect(periods[0].periodType).toBe('endPeriod');
        expect(periods[0].eventType).toBe(CalendarEventsType.Vacation);
        expect(periods[1].periodType).toBe('period');
        expect(periods[1].eventType).toBe(CalendarEventsType.SickLeave);

        periods = periodsModel.get(moment({ day: 4, month: 0, year: 2018 }));

        expect(periods.length).toBe(1);
        expect(periods[0].periodType).toBe('endPeriod');
        expect(periods[0].eventType).toBe(CalendarEventsType.SickLeave);
    });
});