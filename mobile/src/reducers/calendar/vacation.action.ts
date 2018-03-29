import { Moment } from 'moment';

export interface ConfirmClaimVacation {
    type: 'CONFIRM-VACATION';
    employeeId: string;
    startDate: Moment;
    endDate: Moment;
}

export const confirmVacation = (employeeId: string, startDate: Moment, endDate: Moment): ConfirmClaimVacation => ({ type: 'CONFIRM-VACATION', employeeId, startDate, endDate });