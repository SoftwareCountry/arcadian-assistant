import { Action } from 'redux';
import { DaysCountersModel, VacationDaysCounter, HoursCreditCounter, TodayCounter } from './days-counters.model';
import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';
import { Employee } from '../organization/employee.model';
import moment from 'moment';

export interface CalculateDaysCounters {
    type: 'CALCULATE-DAYS-COUNTERS';
    daysCounters: DaysCountersModel;
}

export const calculateDaysCounters = (vacationDaysLeft: number, hoursCredit: number): CalculateDaysCounters => {
    const allVacationDaysCounter = new VacationDaysCounter(vacationDaysLeft);
    
    const daysConverter = new ConvertHoursCreditToDays();
    const calculatedDays = daysConverter.convert(hoursCredit);

    const hoursCreditCounter = new HoursCreditCounter(hoursCredit, calculatedDays.days, calculatedDays.rest);

    return {
        type: 'CALCULATE-DAYS-COUNTERS',
        daysCounters: {
            allVacationDays: allVacationDaysCounter,
            hoursCredit: hoursCreditCounter
        }
    };
};

export interface CalculateTodayCounter {
    type: 'CALCULATE-TODAY-COUNTER';
    today: TodayCounter;
}

export const calculateTodayCounter = (): CalculateTodayCounter => {
    const today = new TodayCounter();

    return {
        type: 'CALCULATE-TODAY-COUNTER',
        today: today
    };
};

export type CalendarActions = CalculateDaysCounters | CalculateTodayCounter;