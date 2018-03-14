import { Moment } from 'moment';

export interface ClaimSickLeave {
    type: 'CLAIM-SICK-LEAVE';
    startDate: Moment;
}

export const claimSickLeave = (startDate: Moment): ClaimSickLeave => ({ type: 'CLAIM-SICK-LEAVE', startDate });

export interface ConfirmClaimSickLeave {
    type: 'CONFIRM-CLAIM-SICK-LEAVE';
}

export const confirmSickLeave = (): ConfirmClaimSickLeave => ({ type: 'CONFIRM-CLAIM-SICK-LEAVE' });

export interface EditSickLeave {
    type: 'EDIT-SICK-LEAVE';
}

export const editSickLeave = (): EditSickLeave => ({ type: 'EDIT-SICK-LEAVE' });

export interface CompleteSickLeave {
    type: 'COMPLETE-SICK-LEAVE';
}

export const completeSickLeave = (): CompleteSickLeave => ({ type: 'COMPLETE-SICK-LEAVE' });

export interface ProlongSickLeave {
    type: 'PROLONG-SICK-LEAVE';
}

export const prolongSickLeave = (): ProlongSickLeave => ({ type: 'PROLONG-SICK-LEAVE' });

export interface ConfirmProlongSickLeave {
    type: 'CONFIRM-PROLONG-SICK-LEAVE';
}

export const confirmProlongSickLeave = (): ConfirmProlongSickLeave => ({ type: 'CONFIRM-PROLONG-SICK-LEAVE' });

export type SickLeaveActions = ClaimSickLeave | ConfirmClaimSickLeave | EditSickLeave | CompleteSickLeave | ProlongSickLeave | ConfirmProlongSickLeave;