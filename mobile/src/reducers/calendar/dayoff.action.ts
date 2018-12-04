import { Moment } from 'moment';
import { IntervalType } from './calendar.model';
import { CalendarEvent } from './calendar-event.model';
import { Action } from 'redux';

export interface ChosenTypeDayoff extends Action {
    type: 'CHOSEN-TYPE-DAYOFF';
    isWorkout: boolean;
}

export const chosenTypeDayoff = (
    isWorkout: boolean
): ChosenTypeDayoff => ({ type: 'CHOSEN-TYPE-DAYOFF', isWorkout });

export interface ConfirmProcessDayoff extends Action {
    type: 'CONFIRM-PROCESS-DAYOFF';
    employeeId: string;
    date: Moment;
    isWorkout: boolean;
    intervalType: IntervalType;
}

export const confirmProcessDayoff = (
    employeeId: string,
    date: Moment,
    isWorkout: boolean,
    intervalType: IntervalType
): ConfirmProcessDayoff => ({ type: 'CONFIRM-PROCESS-DAYOFF', employeeId, date, isWorkout, intervalType });

export interface CancelDayoff extends Action {
    type: 'CANCEL-DAYOFF';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const cancelDayoff = (
    employeeId: string,
    calendarEvent: CalendarEvent
): CancelDayoff => ({ type: 'CANCEL-DAYOFF', employeeId, calendarEvent });

export type DayoffActions = ChosenTypeDayoff | ConfirmProcessDayoff | CancelDayoff;
