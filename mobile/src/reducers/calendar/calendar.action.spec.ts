import { calculateDaysCounters } from './calendar.action';
import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';

describe('calendar action', () => {
    describe('calculate days counters', () => {

        describe('all vacation days counter', () => {

            it('should render 0, if days are 0', () => {
                const action = calculateDaysCounters(0, 0);

                expect(action.daysCounters.allVacationDays.toString()).toBe('0');
            });

            it('should render days, if days are not 0', () => {
                const action = calculateDaysCounters(28, 0);

                expect(action.daysCounters.allVacationDays.toString()).toBe('28');
            });

            it('should have title', () => {
                const action = calculateDaysCounters(28, 0);
                expect(action.daysCounters.allVacationDays.title).toBe('days of vacation left');
            });
        });

        describe('hours credit counter', () => {

            it('should render 0, if hours are 0', () => {
                const hours = 0;
                const action = calculateDaysCounters(0, hours);

                expect(action.daysCounters.hoursCredit.toString()).toBe('0');
            });

            describe('hours are divisble by entire day', () => {
                it('should render entire days', () => {
                    const hours = 4 * 2;
                    const action = calculateDaysCounters(0, hours);

                    expect(action.daysCounters.hoursCredit.toString()).toBe('1');
                });

                it('should render entire days, if hours are negative', () => {
                    const hours = -(4 * 2);
                    const action = calculateDaysCounters(0, hours);

                    expect(action.daysCounters.hoursCredit.toString()).toBe('1');
                });
            });

            describe('hours are NOT divisble by entire day', () => {
                it('should render only fraction, if hours <= half day', () => {
                    let hours = 1;
                    let action = calculateDaysCounters(0, hours);
                    expect(action.daysCounters.hoursCredit.toString()).toBe(ConvertHoursCreditToDays.fractionSymbol);

                    hours = 4;
                    action = calculateDaysCounters(0, hours);
                    expect(action.daysCounters.hoursCredit.toString()).toBe(ConvertHoursCreditToDays.fractionSymbol);
                });

                it('should render days with fraction, if rest <= half day', () => {
                    const rest = 2;
                    const hours = 4 + 4 + rest;
                    const action = calculateDaysCounters(0, hours);
                    expect(action.daysCounters.hoursCredit.toString()).toBe('1' + ConvertHoursCreditToDays.fractionSymbol);
                });

                it('should render days rounded up, if rest > half day', () => {
                    const rest = 5;
                    const hours = 4 + 4 + rest;
                    const action = calculateDaysCounters(0, hours);

                    expect(action.daysCounters.hoursCredit.toString()).toBe('2');
                });

                it('should render days with fraction, if hours are negative', () => {
                    const rest = 2;
                    const hours = -(4 + 4 + rest);
                    const action = calculateDaysCounters(0, hours);
                    expect(action.daysCounters.hoursCredit.toString()).toBe('1' + ConvertHoursCreditToDays.fractionSymbol);
                });
            });

            it('should have title "dayoffs to return", if hours > 0', () => {
                const hours = 4;
                const action = calculateDaysCounters(0, hours);

                expect(action.daysCounters.hoursCredit.title).toBe('dayoffs to return');
            });

            it('should have title "dayoffs to left", if hours < 0', () => {
                const hours = -4;
                const action = calculateDaysCounters(0, hours);

                expect(action.daysCounters.hoursCredit.title).toBe('dayoffs to left');
            });
        });
    });
});