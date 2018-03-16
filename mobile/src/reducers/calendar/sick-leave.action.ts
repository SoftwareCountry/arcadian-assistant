import { Moment } from 'moment';
import { Employee } from '../organization/employee.model';
import { CalendarEvents } from './calendar-events.model';

export interface ConfirmClaimSickLeave {
    type: 'CONFIRM-CLAIM-SICK-LEAVE';
    employeeId: string;
    startDate: Moment;
    endDate: Moment;
}

export const confirmSickLeave = (employeeId: string, startDate: Moment, endDate: Moment): ConfirmClaimSickLeave => ({ type: 'CONFIRM-CLAIM-SICK-LEAVE', employeeId, startDate, endDate });

export interface CompleteSickLeave {
    type: 'COMPLETE-SICK-LEAVE';
}

export const completeSickLeave = (): CompleteSickLeave => ({ type: 'COMPLETE-SICK-LEAVE' });

export interface ConfirmProlongSickLeave {
    type: 'CONFIRM-PROLONG-SICK-LEAVE';
}

export const confirmProlongSickLeave = (): ConfirmProlongSickLeave => ({ type: 'CONFIRM-PROLONG-SICK-LEAVE' });

export type SickLeaveActions =  ConfirmClaimSickLeave | CompleteSickLeave | ConfirmProlongSickLeave;