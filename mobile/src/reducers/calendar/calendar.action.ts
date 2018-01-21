import { Action } from 'redux';
import { DaysItem, Days } from './days.model';

export interface LoadDays extends Action {
    type: 'LOAD-DAYS';
}

export const loadDays = (): LoadDays => ({ type: 'LOAD-DAYS' });

export interface LoadDaysFinished extends Action {
    type: 'LOAD-DAYS-FINISHED';
    days: Days;
}

export const loadDaysFinished = (days: Days): LoadDaysFinished => ({ type: 'LOAD-DAYS-FINISHED', days });

export type CalendarActions = LoadDays | LoadDaysFinished;