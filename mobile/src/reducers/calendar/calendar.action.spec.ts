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
            });

            describe('hours are NOT divisble by entire day', () => {
                describe('rest <= half day', () => {
                    it('should render days with fraction', () => {
                        let hours = 1;
                        let action = calculateDaysCounters(0, hours);
                        expect(action.daysCounters.hoursCredit.toString()).toBe(ConvertHoursCreditToDays.fractionSymbol);

                        hours = 4;
                        action = calculateDaysCounters(0, hours);
                        expect(action.daysCounters.hoursCredit.toString()).toBe(ConvertHoursCreditToDays.fractionSymbol);

                        hours = 4 + 4 + 2;
                        action = calculateDaysCounters(0, hours);
                        expect(action.daysCounters.hoursCredit.toString()).toBe('1' + ConvertHoursCreditToDays.fractionSymbol);
                    });
                });

                describe('rest > half day', () => {
                    it('should render days rounded up', () => {
                        const hours = 4 + 4 + 4 + 1;
                        const action = calculateDaysCounters(0, hours);

                        expect(action.daysCounters.hoursCredit.toString()).toBe('2');
                    });
                });
            });

        });
    });
});