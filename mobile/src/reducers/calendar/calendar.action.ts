import { Action } from 'redux';
import { DaysCountersModel, VacationDaysCounter, HoursCreditCounter } from './days-counters.model';
import { ConvertHoursCreditToDays } from './convert-hours-credit-to-days';
import { Employee } from '../organization/employee.model';

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

export type CalendarActions = CalculateDaysCounters;