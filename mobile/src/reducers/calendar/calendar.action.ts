import { Action } from 'redux';
import { DaysCountersModel, VacationDaysCounter, HoursCreditCounter } from './days-counters.model';
import { ConvertHoursCreditToDays } from './days-counters-calculator';
import { Employee } from '../organization/employee.model';

export interface CalculateDaysCounters {
    type: 'CALCULATE-DAYS-COUNTERS';
    daysCounters: DaysCountersModel;
}

export const calculateDaysCounters = (employee: Employee): CalculateDaysCounters => {
    const allVacationDaysCounter = new VacationDaysCounter(employee.vacationDaysLeft);
    
    const daysConverter = new ConvertHoursCreditToDays();
    const calculatedDays = daysConverter.convert(employee.hoursCredit);

    const hoursCreditCounter = new HoursCreditCounter(employee.hoursCredit, calculatedDays.days, calculatedDays.rest);

    return {
        type: 'CALCULATE-DAYS-COUNTERS',
        daysCounters: {
            allVacationDays: allVacationDaysCounter,
            hoursCredit: hoursCreditCounter
        }
    };
};

export type CalendarActions = CalculateDaysCounters;