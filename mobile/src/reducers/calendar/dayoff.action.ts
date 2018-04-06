import { Moment } from 'moment';
import { IntervalType } from './calendar.model';

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