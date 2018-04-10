import { Moment } from 'moment';
import { IntervalType } from './calendar.model';
import { CalendarEvent } from './calendar-event.model';

export interface ConfirmProcessDayoff {
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

export interface CancelDayoff {
    type: 'CANCEL-DAYOFF';
    employeeId: string;
    calendarEvent: CalendarEvent;
}

export const cancelDayoff = (
    employeeId: string,
    calendarEvent: CalendarEvent
): CancelDayoff => ({ type: 'CANCEL-DAYOFF', employeeId, calendarEvent });