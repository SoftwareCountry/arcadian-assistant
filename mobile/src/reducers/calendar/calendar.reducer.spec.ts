import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';
import moment from 'moment';
import { loadUserEmployeeFinished } from '../user/user.action';
import { Employee } from '../organization/employee.model';
import { daysCountersReducer } from './days-counters.reducer';

describe('calendar action', () => {
    describe('calculate days counters', () => {

        describe('all vacation days counter', () => {

            it('should render 0, if days are 0', () => {
                const employee = new Employee();
                
                employee.vacationDaysLeft = 0;
                employee.hoursCredit = 0;

                const action = loadUserEmployeeFinished(employee);
                const state = daysCountersReducer(null, action);

                expect(state.allVacationDays.toString()).toBe('0');
            });

            it('should render days, if days are not 0', () => {
                const employee = new Employee();
                
                employee.vacationDaysLeft = 28;
                employee.hoursCredit = 0;

                const action = loadUserEmployeeFinished(employee);
                const state = daysCountersReducer(null, action);

                expect(state.allVacationDays.toString()).toBe('28');
            });

            it('should have title ["days of", "vacation left"]', () => {
                const employee = new Employee();
                
                employee.vacationDaysLeft = 28;
                employee.hoursCredit = 0;

                const action = loadUserEmployeeFinished(employee);
                const state = daysCountersReducer(null, action);
                expect(state.allVacationDays.title).toEqual(['days', 'of vacation left']);
            });
        });

        describe('hours credit counter', () => {

            it('should render 0, if hours are 0', () => {
                const employee = new Employee();

                employee.hoursCredit = 0;

                const action = loadUserEmployeeFinished(employee);
                const state = daysCountersReducer(null, action);

                expect(state.hoursCredit.toString()).toBe('0');
            });

            describe('hours are divisble by entire day', () => {
                it('should render entire days', () => {
                    const employee = new Employee();

                    employee.hoursCredit = 4 * 2;
    
                    const action = loadUserEmployeeFinished(employee);
                    const state = daysCountersReducer(null, action);

                    expect(state.hoursCredit.toString()).toBe('1');
                });

                it('should render entire days, if hours are negative', () => {
                    const employee = new Employee();

                    employee.hoursCredit = -(4 * 2);
    
                    const action = loadUserEmployeeFinished(employee);
                    const state = daysCountersReducer(null, action);

                    expect(state.hoursCredit.toString()).toBe('1');
                });
            });

            describe('hours are NOT divisble by entire day', () => {
                it('should render only fraction, if hours <= half day', () => {
                    const employee = new Employee();

                    employee.hoursCredit = 1;
    
                    let action = loadUserEmployeeFinished(employee);
                    let state = daysCountersReducer(null, action);
                    expect(state.hoursCredit.toString()).toBe(ConvertHoursCreditToDays.fractionSymbol);

                    employee.hoursCredit = 4;

                    action = loadUserEmployeeFinished(employee);
                    state = daysCountersReducer(null, action);
                    expect(state.hoursCredit.toString()).toBe(ConvertHoursCreditToDays.fractionSymbol);
                });

                it('should render days with fraction, if rest <= half day', () => {
                    const employee = new Employee();

                    const rest = 2;
                    employee.hoursCredit = 4 + 4 + rest;
    
                    const action = loadUserEmployeeFinished(employee);
                    const state = daysCountersReducer(null, action);
                    expect(state.hoursCredit.toString()).toBe('1' + ConvertHoursCreditToDays.fractionSymbol);
                });

                it('should render days rounded up, if rest > half day', () => {
                    const employee = new Employee();

                    const rest = 5;
                    employee.hoursCredit = 4 + 4 + rest;
    
                    const action = loadUserEmployeeFinished(employee);
                    const state = daysCountersReducer(null, action);

                    expect(state.hoursCredit.toString()).toBe('2');
                });

                it('should render days with fraction, if hours are negative', () => {
                    const employee = new Employee();

                    const rest = 2;
                    employee.hoursCredit = -(4 + 4 + rest);
    
                    const action = loadUserEmployeeFinished(employee);
                    const state = daysCountersReducer(null, action);

                    expect(state.hoursCredit.toString()).toBe('1' + ConvertHoursCreditToDays.fractionSymbol);
                });
            });

            it('should have title ["dayoffs", "return"], if hours > 0', () => {
                const employee = new Employee();

                employee.hoursCredit = 5;

                const action = loadUserEmployeeFinished(employee);
                const state = daysCountersReducer(null, action);

                expect(state.hoursCredit.title).toEqual(['daysoff', 'return']);
            });

            it('should have title ["dayoffs", "available"], if hours < 0', () => {
                const employee = new Employee();

                employee.hoursCredit = -4;

                const action = loadUserEmployeeFinished(employee);
                const state = daysCountersReducer(null, action);

                expect(state.hoursCredit.title).toEqual(['daysoff', 'available']);
            });
        });
    });
});