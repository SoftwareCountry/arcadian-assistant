import { Moment } from 'moment';

export interface ClaimDayOff {
    type: 'CLAIM-DAY-OFF';
    startDate: Moment;
}

export const claimDayOff = (startDate: Moment): ClaimDayOff => ({ type: 'CLAIM-DAY-OFF', startDate });

export interface ConfirmClaimDayOff {
    type: 'CONFIRM-CLAIM-DAY-OFF';
}

export const confirmDayOff = (): ConfirmClaimDayOff => ({ type: 'CONFIRM-CLAIM-DAY-OFF' });

export interface EditDayOff {
    type: 'EDIT-DAY-OFF';
}

export const editDayOff = (): EditDayOff => ({ type: 'EDIT-DAY-OFF' });

export interface CompleteDayOff {
    type: 'COMPLETE-DAY-OFF';
}

export const completeDayOff = (): CompleteDayOff => ({ type: 'COMPLETE-DAY-OFF' });

export interface ProlongDayOff {
    type: 'PROLONG-DAY-OFF';
}

export const prolongDayOff = (): ProlongDayOff => ({ type: 'PROLONG-DAY-OFF' });

export interface ConfirmProlongDayOff {
    type: 'CONFIRM-PROLONG-DAY-OFF';
}

export const confirmProlongDayOff = (): ConfirmProlongDayOff => ({ type: 'CONFIRM-PROLONG-DAY-OFF' });

export type DayOffActions = ClaimDayOff | ConfirmClaimDayOff | EditDayOff | CompleteDayOff | ProlongDayOff | ConfirmProlongDayOff;