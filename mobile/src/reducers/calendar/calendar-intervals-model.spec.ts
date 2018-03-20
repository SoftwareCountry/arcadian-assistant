import { IntervalsModel, IntervalModel, IntervalType } from './calendar.model';
import moment from 'moment';
import { CalendarEventsType, CalendarEvents, DatesInterval } from './calendar-events.model';
import { Moment } from 'moment';

describe('IntervalsModel', () => {
    let calendarEvent: CalendarEvents;
    let date: Moment;

    beforeEach(() => {
        calendarEvent = new CalendarEvents();
        calendarEvent.type = CalendarEventsType.Workout;
        calendarEvent.dates = new DatesInterval();
        calendarEvent.dates.startDate = date;
        calendarEvent.dates.endDate = date;

        date = moment();
    });

    it('should generate string key with "DD-MM-YYYY" format', () => {
        const key = IntervalsModel.generateKey(date);
        expect(key).toBe(date.format('DD-MM-YYYY'));
    });

    describe('set', () => {
        it('should create new array and put element, if there is no such key', () => {
            const intervalsModel = new IntervalsModel();

            const intervalModel: IntervalModel = {
                intervalType: IntervalType.StartInterval,
                calendarEvent: calendarEvent,
                boundary: true
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
                intervalType: IntervalType.StartInterval,
                calendarEvent: calendarEvent,
                boundary: true
            };

            intervalsModel.set(key, intervalModel);

            const existingArray = intervalsModel.get(key);

            const intervalModel2: IntervalModel = {
                intervalType: IntervalType.StartInterval,
                calendarEvent: calendarEvent,
                boundary: true
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
        const array = intervalsModel.get(date);
        expect(array).toBeUndefined();
    });

    it('should get elements by key', () => {
        const intervalsModel = new IntervalsModel();
        const intervalModel: IntervalModel = {
            intervalType: IntervalType.StartInterval,
            calendarEvent: calendarEvent,
            boundary: true
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
            let copiedCalendarEvent = new CalendarEvents();
            copiedCalendarEvent.type = CalendarEventsType.Workout;
            copiedCalendarEvent.dates = new DatesInterval();
            copiedCalendarEvent.dates.startDate = date;
            copiedCalendarEvent.dates.endDate = date;

            intervalsModel = new IntervalsModel();

            const startInterval: IntervalModel = {
                intervalType: IntervalType.StartInterval,
                calendarEvent: copiedCalendarEvent,
                boundary: false
            };

            const interval: IntervalModel = {
                intervalType: IntervalType.Interval,
                calendarEvent: copiedCalendarEvent,
                boundary: false
            };

            const endInterval: IntervalModel = {
                intervalType: IntervalType.EndInterval,
                calendarEvent: copiedCalendarEvent,
                boundary: false
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