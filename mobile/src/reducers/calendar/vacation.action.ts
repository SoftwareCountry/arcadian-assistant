import { Moment } from 'moment';

export interface ClaimVacation {
    type: 'CLAIM-VACATION';
    startDate: Moment;
}

export const claimVacation = (startDate: Moment): ClaimVacation => ({ type: 'CLAIM-VACATION', startDate });

export interface ConfirmClaimVacation {
    type: 'CONFIRM-CLAIM-VACATION';
}

export const confirmVacation = (): ConfirmClaimVacation => ({ type: 'CONFIRM-CLAIM-VACATION' });

export interface EditVacation {
    type: 'EDIT-VACATION';
}

export const editVacation = (): EditVacation => ({ type: 'EDIT-VACATION' });

export interface CompleteVacation {
    type: 'COMPLETE-VACATION';
}

export const completeVacation = (): CompleteVacation => ({ type: 'COMPLETE-VACATION' });

export interface ProlongVacation {
    type: 'PROLONG-VACATION';
}

export const prolongVacation = (): ProlongVacation => ({ type: 'PROLONG-VACATION' });

export interface ConfirmProlongVacation {
    type: 'CONFIRM-PROLONG-VACATION';
}

export const confirmProlongVacation = (): ConfirmProlongVacation => ({ type: 'CONFIRM-PROLONG-VACATION' });

export type VacationActions = ClaimVacation | ConfirmClaimVacation | EditVacation | CompleteVacation | ProlongVacation | ConfirmProlongVacation;