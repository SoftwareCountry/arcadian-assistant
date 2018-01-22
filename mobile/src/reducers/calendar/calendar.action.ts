import { Action } from 'redux';
import { DaysCounterItem, DaysCountersModel } from './days-counters.model';

export interface LoadDaysCounters extends Action {
    type: 'LOAD-DAYS-COUNTERS';
}

export const loadDaysCounters = (): LoadDaysCounters => ({ type: 'LOAD-DAYS-COUNTERS' });

export interface LoadDaysCountersFinished extends Action {
    type: 'LOAD-DAYS-COUNTERS-FINISHED';
    daysCounters: DaysCountersModel;
}

export const loadDaysFinished = (daysCounters: DaysCountersModel): LoadDaysCountersFinished => ({ type: 'LOAD-DAYS-COUNTERS-FINISHED', daysCounters });

export type CalendarActions = LoadDaysCounters | LoadDaysCountersFinished;