import moment from 'moment';
import { WeekModel, IntervalsModel, IntervalModel, IntervalType } from '../calendar.model';
import { CalendarEvent, CalendarEventType, DatesInterval } from '../calendar-event.model';
import { CalendarEvents } from '../calendar-events.model';

describe('CalendarEvents', () => {
    it('should build interval boundary if startDate and endDate is the same', () => {
        const event = new CalendarEvent();

        const date = moment();

        event.calendarEventId = '1';
        event.type = CalendarEventType.Vacation;
        event.dates = new DatesInterval();
        event.dates.startDate = moment(date);
        event.dates.endDate = moment(date);

        const calendarEvents = new CalendarEvents([event]);

        const intervalsModel = calendarEvents.buildIntervalsModel();

        const intervals = intervalsModel.get(date);

        expect(intervals[0].calendarEvent.type).toBe(event.type);
        expect(intervals[0].intervalType).toBe(IntervalType.IntervalFullBoundary);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeTruthy();
    });

    it('should build interval if endDate > startDate', () => {
        const event = new CalendarEvent();

        const date = moment({ day: 1, month: 0, year: 2018 });

        event.calendarEventId = '1';
        event.type = CalendarEventType.Vacation;
        event.dates = new DatesInterval();
        event.dates.startDate = moment(date);
        event.dates.endDate = moment(date);

        event.dates.endDate.add(3, 'days');

        const calendarEvents = new CalendarEvents([event]);
        const intervalsModel = calendarEvents.buildIntervalsModel();

        const one = moment({ day: 1, month: 0, year: 2018 });
        let intervals = intervalsModel.get(one);

        expect(intervals[0].calendarEvent.type).toBe(event.type);
        expect(intervals[0].intervalType).toBe(IntervalType.StartInterval);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeFalsy();

        const two = moment({ day: 2, month: 0, year: 2018 });
        intervals = intervalsModel.get(two);

        expect(intervals[0].calendarEvent.type).toBe(event.type);
        expect(intervals[0].intervalType).toBe(IntervalType.Interval);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeFalsy();

        const three = moment({ day: 3, month: 0, year: 2018 });
        intervals = intervalsModel.get(three);

        expect(intervals[0].calendarEvent.type).toBe(event.type);
        expect(intervals[0].intervalType).toBe(IntervalType.Interval);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event.dates.endDate);
        expect(intervals[0].boundary).toBeFalsy();

        const four = moment({ day: 4, month: 0, year: 2018 });
        intervals = intervalsModel.get(four);

        expect(intervals[0].calendarEvent.type).toBe(event.type);
        expect(intervals[0].intervalType).toBe(IntervalType.EndInterval);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event.dates.endDate);
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

        const event1 = new CalendarEvent();
        event1.calendarEventId = '1';
        event1.type = CalendarEventType.Vacation;
        event1.dates = new DatesInterval();
        event1.dates.startDate = moment(date1);
        event1.dates.endDate = moment(date1);
        event1.dates.endDate.add(2, 'days');

        const date2 = moment({ day: 2, month: 0, year: 2018 });

        const event2 = new CalendarEvent();
        event2.calendarEventId = '1';
        event2.type = CalendarEventType.Sickleave;
        event2.dates = new DatesInterval();
        event2.dates.startDate = moment(date2);
        event2.dates.endDate = moment(date2);
        event2.dates.endDate.add(2, 'days');

        const calendarEvents = new CalendarEvents([event1, event2]);
        const intervalsModel = calendarEvents.buildIntervalsModel();

        let intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

        expect(intervals.length).toBe(1);
        expect(intervals[0].intervalType).toBe(IntervalType.StartInterval);
        expect(intervals[0].calendarEvent.type).toBe(CalendarEventType.Vacation);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event1.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event1.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 2, month: 0, year: 2018 }));

        expect(intervals.length).toBe(2);
        expect(intervals[0].intervalType).toBe(IntervalType.Interval);
        expect(intervals[0].calendarEvent.type).toBe(CalendarEventType.Vacation);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event1.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event1.dates.endDate);
        expect(intervals[1].intervalType).toBe(IntervalType.StartInterval);
        expect(intervals[1].calendarEvent.type).toBe(CalendarEventType.Sickleave);
        expect(intervals[1].calendarEvent.dates.startDate).toBe(event2.dates.startDate);
        expect(intervals[1].calendarEvent.dates.endDate).toBe(event2.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 3, month: 0, year: 2018 }));

        expect(intervals.length).toBe(2);
        expect(intervals[0].intervalType).toBe(IntervalType.EndInterval);
        expect(intervals[0].calendarEvent.type).toBe(CalendarEventType.Vacation);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event1.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event1.dates.endDate);
        expect(intervals[1].intervalType).toBe(IntervalType.Interval);
        expect(intervals[1].calendarEvent.type).toBe(CalendarEventType.Sickleave);
        expect(intervals[1].calendarEvent.dates.startDate).toBe(event2.dates.startDate);
        expect(intervals[1].calendarEvent.dates.endDate).toBe(event2.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 4, month: 0, year: 2018 }));

        expect(intervals.length).toBe(1);
        expect(intervals[0].intervalType).toBe(IntervalType.EndInterval);
        expect(intervals[0].calendarEvent.type).toBe(CalendarEventType.Sickleave);
        expect(intervals[0].calendarEvent.dates.startDate).toBe(event2.dates.startDate);
        expect(intervals[0].calendarEvent.dates.endDate).toBe(event2.dates.endDate);
    });

    it('should append calendar events to existing model', () => {
        /*
            event1: [01-01-2018  02-01-2018  03-01-2018]
            event2:             [02-01-2018  03-01-2018  04-01-2018]
            event3: [01-01-2018  02-01-2018  03-01-2018]
        */

        const date1 = moment({ day: 1, month: 0, year: 2018 });

        const event1 = new CalendarEvent();
        event1.calendarEventId = '1';
        event1.type = CalendarEventType.Vacation;
        event1.dates = new DatesInterval();
        event1.dates.startDate = moment(date1);
        event1.dates.endDate = moment(date1);
        event1.dates.endDate.add(2, 'days');

        const date2 = moment({ day: 2, month: 0, year: 2018 });

        const event2 = new CalendarEvent();
        event2.calendarEventId = '2';
        event2.type = CalendarEventType.Sickleave;
        event2.dates = new DatesInterval();
        event2.dates.startDate = moment(date2);
        event2.dates.endDate = moment(date2);
        event2.dates.endDate.add(2, 'days');

        let calendarEvents = new CalendarEvents([event1, event2]);
        const intervalsModel = calendarEvents.buildIntervalsModel();

        const date3 = moment({ day: 1, month: 0, year: 2018 });

        const event3 = new CalendarEvent();
        event3.calendarEventId = '3';
        event3.type = CalendarEventType.Vacation;
        event3.dates = new DatesInterval();
        event3.dates.startDate = moment(date3);
        event3.dates.endDate = moment(date3);
        event3.dates.endDate.add(2, 'days');

        calendarEvents = new CalendarEvents([event3]);
        calendarEvents.appendToIntervalsModel(intervalsModel);

        let intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

        expect(intervals[1].intervalType).toBe(IntervalType.StartInterval);
        expect(intervals[1].calendarEvent.type).toBe(CalendarEventType.Vacation);
        expect(intervals[1].calendarEvent.dates.startDate).toBe(event3.dates.startDate);
        expect(intervals[1].calendarEvent.dates.endDate).toBe(event3.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 2, month: 0, year: 2018 }));

        expect(intervals[2].intervalType).toBe(IntervalType.Interval);
        expect(intervals[2].calendarEvent.type).toBe(CalendarEventType.Vacation);
        expect(intervals[2].calendarEvent.dates.startDate).toBe(event3.dates.startDate);
        expect(intervals[2].calendarEvent.dates.endDate).toBe(event3.dates.endDate);

        intervals = intervalsModel.get(moment({ day: 3, month: 0, year: 2018 }));

        expect(intervals[2].intervalType).toBe(IntervalType.EndInterval);
        expect(intervals[2].calendarEvent.type).toBe(CalendarEventType.Vacation);
        expect(intervals[2].calendarEvent.dates.startDate).toBe(event3.dates.startDate);
        expect(intervals[2].calendarEvent.dates.endDate).toBe(event3.dates.endDate);
    });

    it('should fill metadata', () => {
        const date1 = moment({ day: 1, month: 0, year: 2018 });

        const event1 = new CalendarEvent();
        event1.calendarEventId = '1';
        event1.type = CalendarEventType.Vacation;
        event1.dates = new DatesInterval();
        event1.dates.startDate = moment(date1);
        event1.dates.endDate = moment(date1);
        event1.dates.endDate.add(2, 'days');

        const date2 = moment({ day: 2, month: 0, year: 2018 });

        const event2 = new CalendarEvent();
        event2.calendarEventId = '2';
        event2.type = CalendarEventType.Sickleave;
        event2.dates = new DatesInterval();
        event2.dates.startDate = moment(date2);
        event2.dates.endDate = moment(date2);
        event2.dates.endDate.add(2, 'days');

        const date3 = moment({ day: 2, month: 0, year: 2018 });

        const event3 = new CalendarEvent();
        event3.calendarEventId = '3';
        event3.type = CalendarEventType.Sickleave;
        event3.dates = new DatesInterval();
        event3.dates.startDate = moment(date3);
        event3.dates.endDate = moment(date3);

        const calendarEvents = new CalendarEvents([event1, event2, event3]);
        const intervalsModel = calendarEvents.buildIntervalsModel();

        expect(intervalsModel.metadata.calendarEvents.length).toBe(3);
        expect(intervalsModel.metadata.calendarEvents.find(x => x === event1)).toBe(event1);
        expect(intervalsModel.metadata.calendarEvents.find(x => x === event2)).toBe(event2);
        expect(intervalsModel.metadata.calendarEvents.find(x => x === event3)).toBe(event3);
    });

    describe('dayoff', () => {

        const testDayoff = (testedEventType: CalendarEventType.Dayoff | CalendarEventType.Workout) => {
            describe(testedEventType, () => {
                it('should return interval left boundary, if interval [startWorkingHour, finishWorkingHour] is between [0, 4]', () => {
                    const date1 = moment({ day: 1, month: 0, year: 2018 });

                    const event1 = new CalendarEvent();
                    event1.calendarEventId = '1';
                    event1.type = testedEventType;
                    event1.dates = new DatesInterval();
                    event1.dates.startDate = moment(date1);
                    event1.dates.endDate = moment(date1);

                    event1.dates.startWorkingHour = 0;
                    event1.dates.finishWorkingHour = 4;

                    const calendarEvents = new CalendarEvents([event1]);
                    const intervalsModel = calendarEvents.buildIntervalsModel();

                    const intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

                    expect(intervals.length).toBe(1);
                    expect(intervals[0].intervalType).toBe(IntervalType.IntervalFullBoundary);
                    expect(intervals[0].calendarEvent.type).toBe(testedEventType);
                    expect(intervals[0].calendarEvent.dates.startDate).toBe(event1.dates.startDate);
                    expect(intervals[0].calendarEvent.dates.endDate).toBe(event1.dates.endDate);
                    expect(intervals[0].boundary).toBeTruthy();
                });

                it('should return interval right boundary, if interval [startWorkingHour, finishWorkingHour] is between [4, 8]', () => {
                    const date1 = moment({ day: 1, month: 0, year: 2018 });

                    const event1 = new CalendarEvent();
                    event1.calendarEventId = '1';
                    event1.type = testedEventType;
                    event1.dates = new DatesInterval();
                    event1.dates.startDate = moment(date1);
                    event1.dates.endDate = moment(date1);

                    event1.dates.startWorkingHour = 4;
                    event1.dates.finishWorkingHour = 8;

                    const calendarEvents = new CalendarEvents([event1]);
                    const intervalsModel = calendarEvents.buildIntervalsModel();

                    const intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

                    expect(intervals.length).toBe(1);
                    expect(intervals[0].intervalType).toBe(IntervalType.IntervalRightBoundary);
                    expect(intervals[0].calendarEvent.type).toBe(testedEventType);
                    expect(intervals[0].calendarEvent.dates.startDate).toBe(event1.dates.startDate);
                    expect(intervals[0].calendarEvent.dates.endDate).toBe(event1.dates.endDate);
                    expect(intervals[0].boundary).toBeTruthy();
                });

                it('should return interval full boundary, if interval [startWorkingHour, finishWorkingHour] is between [0, 8]', () => {
                    const date1 = moment({ day: 1, month: 0, year: 2018 });

                    const event1 = new CalendarEvent();
                    event1.calendarEventId = '1';
                    event1.type = testedEventType;
                    event1.dates = new DatesInterval();
                    event1.dates.startDate = moment(date1);
                    event1.dates.endDate = moment(date1);

                    event1.dates.startWorkingHour = 4;
                    event1.dates.finishWorkingHour = 6;

                    const calendarEvents = new CalendarEvents([event1]);
                    const intervalsModel = calendarEvents.buildIntervalsModel();

                    const intervals = intervalsModel.get(moment({ day: 1, month: 0, year: 2018 }));

                    expect(intervals.length).toBe(1);
                    expect(intervals[0].intervalType).toBe(IntervalType.IntervalFullBoundary);
                    expect(intervals[0].calendarEvent.type).toBe(testedEventType);
                    expect(intervals[0].calendarEvent.dates.startDate).toBe(event1.dates.startDate);
                    expect(intervals[0].calendarEvent.dates.endDate).toBe(event1.dates.endDate);
                    expect(intervals[0].boundary).toBeTruthy();
                });
            });

            testDayoff(CalendarEventType.Dayoff);
            testDayoff(CalendarEventType.Workout);
        };
    });
});