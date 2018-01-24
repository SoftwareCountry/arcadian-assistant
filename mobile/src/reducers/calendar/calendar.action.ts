import { Action } from 'redux';
import { DaysCountersModel, DaysCounterRaw, DaysCounterModelItem, DaysCounterItemRaw } from './days-counters.model';
import { DaysCountersCalculator } from './days-counters-calculator';

export interface LoadDaysCounters extends Action {
    type: 'LOAD-DAYS-COUNTERS';
}

export const loadDaysCounters = (): LoadDaysCounters => ({ type: 'LOAD-DAYS-COUNTERS' });

export interface LoadDaysCountersFinished extends Action {
    type: 'LOAD-DAYS-COUNTERS-FINISHED';
    daysCounterRaw: DaysCounterRaw;
}

export const loadDaysFinished = (daysCounterRaw: DaysCounterRaw): LoadDaysCountersFinished => ({ type: 'LOAD-DAYS-COUNTERS-FINISHED', daysCounterRaw });

export interface CalculateDaysCounters {
    type: 'CALCULATE-DAYS-COUNTERS';
    daysCounters: DaysCountersModel;
}

export const calculateDaysCounters = (daysCounterRaw: DaysCounterRaw): CalculateDaysCounters => {
    const daysCalculator = new DaysCountersCalculator();

    const allVacationDays = daysCalculator.calculateAllVacationDays(daysCounterRaw.allVacationDays.timestamp);
    const daysOff = daysCalculator.calculateDaysOff(daysCounterRaw.daysOff.timestamp);

    return {
        type: 'CALCULATE-DAYS-COUNTERS',
        daysCounters: {
            allVacationDays: {
                days: allVacationDays,
                title: daysCounterRaw.allVacationDays.title,
                return: daysCounterRaw.allVacationDays.return
            },
            daysOff: {
                days: daysOff,
                title: daysCounterRaw.daysOff.title,
                return: daysCounterRaw.daysOff.return
            }
        }
    };
};

export type CalendarActions = LoadDaysCounters | LoadDaysCountersFinished | CalculateDaysCounters;