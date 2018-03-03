import moment from 'moment';
import { WeekModel, IntervalsModel, IntervalModel, IntervalType } from './calendar.model';
import { CalendarEvents, CalendarEventsType, DatesInterval } from './calendar-events.model';
import { CalendarIntervalsBuilder } from './calendar-intervals-builder';

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
                eventType: CalendarEventsType.AdditionalWork,
                startDate: date,
                endDate: date,
                boundary: true,
                draft: false
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
                eventType: CalendarEventsType.AdditionalWork,
                startDate: key,
                endDate: key,
                boundary: true,
                draft: false
            };

            intervalsModel.set(key, intervalModel);

            const existingArray = intervalsModel.get(key);

            const intervalModel2: IntervalModel = {
                intervalType: 'startInterval',
                eventType: CalendarEventsType.AdditionalWork,
                startDate: key,
                endDate: key,
                boundary: true,
                draft: false
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
            eventType: CalendarEventsType.AdditionalWork,
            startDate: date,
            endDate: date,
            boundary: true,
            draft: false
        };

        intervalsModel.set(date, intervalModel);
        const array = intervalsModel.get(date);

        expect(array.length).toEqual(1);
        expect(array[0]).toEqual(intervalModel);
    });

    describe('copy', () => {
        let startDate = moment({ day: 1, month: 1, year: 2018 });
        let intervalDate = moment({ day: 2, month: 1, year: 2018 });
        let endDate = moment({ day: 3, month: 1, year: 2018 });

        let intervalsModel: IntervalsModel;

        beforeEach(() => {
            intervalsModel = new IntervalsModel();

            const startInterval: IntervalModel = {
                intervalType: 'startInterval',
                eventType: CalendarEventsType.SickLeave,
                startDate: startDate,
                endDate: endDate,
                boundary: false,
                draft: false
            };

            const interval: IntervalModel = {
                intervalType: 'interval',
                eventType: CalendarEventsType.SickLeave,
                startDate: startDate,
                endDate: endDate,
                boundary: false,
                draft: false
            };

            const endInterval: IntervalModel = {
                intervalType: 'endInterval',
                eventType: CalendarEventsType.SickLeave,
                startDate: startDate,
                endDate: endDate,
                boundary: false,
                draft: false
            };

            intervalsModel.set(startDate, startInterval);
            intervalsModel.set(intervalDate, interval);
            intervalsModel.set(endDate, endInterval);
        });

        it('should return copy of IntervalsModel', () => {
            const copied = intervalsModel.copy();

            expect(intervalsModel).not.toBe(copied);
        });

        it('should return shallow copy of intervals', () => {
            const copied = intervalsModel.copy();

            expect(intervalsModel.get(startDate)).not.toBe(copied.get(startDate));
            expect(intervalsModel.get(intervalDate)).not.toBe(copied.get(intervalDate));
            expect(intervalsModel.get(endDate)).not.toBe(copied.get(endDate));
        });

        it('should not copy intervals elements', () => {
            const copied = intervalsModel.copy();

            expect(intervalsModel.get(startDate)[0]).toBe(copied.get(startDate)[0]);
            expect(intervalsModel.get(intervalDate)[0]).toBe(copied.get(intervalDate)[0]);
            expect(intervalsModel.get(endDate)[0]).toBe(copied.get(endDate)[0]);
        });
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
        expect(intervals[0].intervalType).toBe('intervalFullBoundary');
        expect(intervals[0].startDate).toBe(event.dates.startDate);
        expect(intervals[0].endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeTruthy();
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
        expect(intervals[0].startDate).toBe(event.dates.startDate);
        expect(intervals[0].endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeFalsy();

        const two = moment({ day: 2, month: 0, year: 2018 });
        intervals = intervalsModel.get(two);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('interval');
        expect(intervals[0].startDate).toBe(event.dates.startDate);
        expect(intervals[0].endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeFalsy();

        const three = moment({ day: 3, month: 0, year: 2018 });
        intervals = intervalsModel.get(three);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('interval');
        expect(intervals[0].startDate).toBe(event.dates.startDate);
        expect(intervals[0].endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeFalsy();

        const four = moment({ day: 4, month: 0, year: 2018 });
        intervals = intervalsModel.get(four);

        expect(intervals[0].eventType).toBe(event.type);
        expect(intervals[0].intervalType).toBe('endInterval');
        expect(intervals[0].startDate).toBe(event.dates.startDate);
        expect(intervals[0].endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeFalsy();
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
        expect(intervals[0].startDate).toBe(event1.dates.startDate);
        expect(intervals[0].endDate).toBe(event1.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 2, month: 0, year: 2018 }));

        expect(intervals.length).toBe(2);
        expect(intervals[0].intervalType).toBe('interval');
        expect(intervals[0].eventType).toBe(CalendarEventsType.Vacation);
        expect(intervals[0].startDate).toBe(event1.dates.startDate);
        expect(intervals[0].endDate).toBe(event1.dates.endDate);
        expect(intervals[1].intervalType).toBe('startInterval');
        expect(intervals[1].eventType).toBe(CalendarEventsType.SickLeave);
        expect(intervals[1].startDate).toBe(event2.dates.startDate);
        expect(intervals[1].endDate).toBe(event2.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 3, month: 0, year: 2018 }));

        expect(intervals.length).toBe(2);
        expect(intervals[0].intervalType).toBe('endInterval');
        expect(intervals[0].eventType).toBe(CalendarEventsType.Vacation);
        expect(intervals[0].startDate).toBe(event1.dates.startDate);
        expect(intervals[0].endDate).toBe(event1.dates.endDate);
        expect(intervals[1].intervalType).toBe('interval');
        expect(intervals[1].eventType).toBe(CalendarEventsType.SickLeave);
        expect(intervals[1].startDate).toBe(event2.dates.startDate);
        expect(intervals[1].endDate).toBe(event2.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 4, month: 0, year: 2018 }));

        expect(intervals.length).toBe(1);
        expect(intervals[0].intervalType).toBe('endInterval');
        expect(intervals[0].eventType).toBe(CalendarEventsType.SickLeave);
        expect(intervals[0].startDate).toBe(event2.dates.startDate);
        expect(intervals[0].endDate).toBe(event2.dates.endDate);
    });

    describe('dayoff', () => {

        const testDayoff = (testedEventType: CalendarEventsType.Dayoff | CalendarEventsType.AdditionalWork) => {
            describe(testedEventType, () => {
                it('should return interval left boundary, if interval [startWorkingHour, finishWorkingHour] is between [0, 4]', () => {
                    const date1 = moment({ day: 1, month: 0, year: 2018 });

                    const event1 = new CalendarEvents();
                    event1.calendarEventId = '1';
                    event1.type = testedEventType;
                    event1.dates = new DatesInterval();
                    event1.dates.startDate = moment(date1);
                    event1.dates.endDate = moment(date1);

                    event1.dates.startWorkingHour = 0;
                    event1.dates.finishWorkingHour = 4;

                    const builder = new CalendarIntervalsBuilder();
                    const intervalsModel = builder.buildIntervals([event1]);

                    const intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

                    expect(intervals.length).toBe(1);
                    expect(intervals[0].intervalType).toBe('intervalLeftBoundary');
                    expect(intervals[0].eventType).toBe(testedEventType);
                    expect(intervals[0].startDate).toBe(event1.dates.startDate);
                    expect(intervals[0].endDate).toBe(event1.dates.endDate);
                    expect(intervals[0].boundary).toBeTruthy();
                });

                it('should return interval right boundary, if interval [startWorkingHour, finishWorkingHour] is between [4, 8]', () => {
                    const date1 = moment({ day: 1, month: 0, year: 2018 });

                    const event1 = new CalendarEvents();
                    event1.calendarEventId = '1';
                    event1.type = testedEventType;
                    event1.dates = new DatesInterval();
                    event1.dates.startDate = moment(date1);
                    event1.dates.endDate = moment(date1);

                    event1.dates.startWorkingHour = 4;
                    event1.dates.finishWorkingHour = 8;

                    const builder = new CalendarIntervalsBuilder();
                    const intervalsModel = builder.buildIntervals([event1]);

                    const intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

                    expect(intervals.length).toBe(1);
                    expect(intervals[0].intervalType).toBe('intervalRightBoundary');
                    expect(intervals[0].eventType).toBe(testedEventType);
                    expect(intervals[0].startDate).toBe(event1.dates.startDate);
                    expect(intervals[0].endDate).toBe(event1.dates.endDate);
                    expect(intervals[0].boundary).toBeTruthy();
                });

                it('should return interval full boundary, if interval [startWorkingHour, finishWorkingHour] is between [0, 8]', () => {
                    const date1 = moment({ day: 1, month: 0, year: 2018 });

                    const event1 = new CalendarEvents();
                    event1.calendarEventId = '1';
                    event1.type = testedEventType;
                    event1.dates = new DatesInterval();
                    event1.dates.startDate = moment(date1);
                    event1.dates.endDate = moment(date1);

                    event1.dates.startWorkingHour = 4;
                    event1.dates.finishWorkingHour = 6;

                    const builder = new CalendarIntervalsBuilder();
                    const intervalsModel = builder.buildIntervals([event1]);

                    const intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

                    expect(intervals.length).toBe(1);
                    expect(intervals[0].intervalType).toBe('intervalFullBoundary');
                    expect(intervals[0].eventType).toBe(testedEventType);
                    expect(intervals[0].startDate).toBe(event1.dates.startDate);
                    expect(intervals[0].endDate).toBe(event1.dates.endDate);
                    expect(intervals[0].boundary).toBeTruthy();
                });
            });

            testDayoff(CalendarEventsType.Dayoff);
            testDayoff(CalendarEventsType.AdditionalWork);
        };
    });
});