import { Moment } from 'moment';
import { IntervalType } from './calendar.model';
import { CalendarEvent } from './calendar-event.model';
import { Action } from 'redux';

export interface ChosenTypeDayOff extends Action {
    type: 'CHOSEN-TYPE-DAY-OFF';
    isWorkout: boolean;
}

export const chosenTypeDayOff = (
    isWorkout: boolean
): ChosenTypeDayOff => ({ type: 'CHOSEN-TYPE-DAY-OFF', isWorkout });

export interface ConfirmProcessDayOff extends Action {
    type: 'CONFIRM-PROCESS-DAY-OFF';
    employeeId: string;
    date: Moment;
    isWorkout: boolean;
    intervalType: IntervalType;
}

export const confirmProcessDayOff = (
    employeeId: string,
    date: Moment,
    isWorkout: boolean,
    intervalType: IntervalType
): ConfirmProcessDayOff => ({ type: 'CONFIRM-PROCESS-DAY-OFF', employeeId, date, isWorkout, intervalType });

export interface CancelDayOff extends Action {
    type: 'CANCEL-DAY-OFF';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const cancelDayOff = (
    employeeId: string,
    calendarEvent: CalendarEvent
): CancelDayOff => ({ type: 'CANCEL-DAY-OFF', employeeId, calendarEvent });

export type DayOffActions = ChosenTypeDayOff | ConfirmProcessDayOff | CancelDayOff;
